using System.Security.Cryptography;
using System.Text;
using HomeLedger.Domain.BusinessModels;
using HomeLedger.Domain.Interfaces.Application;
using HomeLedger.Infrastructure.Data;

namespace HomeLedger.Api.Middleware;

public sealed class IdempotencyMiddleware(
    RequestDelegate next,
    ILogger<IdempotencyMiddleware> logger)
{
    private const long MaximumRequestBodySize = 1_048_576;
    private const int BufferSize = 81_920;

    private const string HeaderName = "Idempotency-Key";
    private const int MaximumKeyLength = 128;

    public async Task InvokeAsync(
        HttpContext context,
        IIdempotencyUseCase idempotencyUseCase,
        HomeLedgerDbContext db)
    {
        if (!HttpMethods.IsPost(context.Request.Method)
            || !context.Request.Path.StartsWithSegments("/api"))
        {
            await next(context);
            return;
        }

        if (!TryGetKey(context.Request, out var idempotencyKey))
        {
            await Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "A valid Idempotency-Key header is required.")
                .ExecuteAsync(context);
            return;
        }

        var requestHash = await ComputeRequestHashAsync(
            context.Request,
            context.RequestAborted);

        var record = new IdempotencyRecord
        {
            IdempotencyKey = idempotencyKey,
            RequestHash = requestHash,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        var originalResponseBody = context.Response.Body;
        await using var responseBuffer = new MemoryStream();

        var keyReserved = false;

        await using (var transaction =
            await db.Database.BeginTransactionAsync(context.RequestAborted))
        {
            try
            {
                keyReserved = await idempotencyUseCase.TryBeginAsync(
                    record,
                    context.RequestAborted);

                if (!keyReserved)
                {
                    await transaction.RollbackAsync(CancellationToken.None);
                }
                else
                {
                    context.Response.Body = responseBuffer;

                    await next(context);

                    responseBuffer.Position = 0;

                    using var reader = new StreamReader(
                        responseBuffer,
                        Encoding.UTF8,
                        detectEncodingFromByteOrderMarks: true,
                        bufferSize: 1024,
                        leaveOpen: true);

                    var responseBody = await reader.ReadToEndAsync(
                        context.RequestAborted);

                    var responseLocation =
                        context.Response.Headers.Location.ToString();

                    if (context.Response.StatusCode >= StatusCodes.Status500InternalServerError)
                    {
                        context.Response.Clear();

                        throw new InvalidOperationException(
                            $"The endpoint returned HTTP {context.Response.StatusCode}.");
                    }

                    await idempotencyUseCase.CompleteAsync(
                        idempotencyKey,
                        context.Response.StatusCode,
                        context.Response.ContentType,
                        string.IsNullOrEmpty(responseLocation)
                            ? null
                            : responseLocation,
                        responseBody,
                        context.RequestAborted);


                    await transaction.CommitAsync(context.RequestAborted);
                }
            }
            catch
            {
                try
                {
                    await transaction.RollbackAsync(CancellationToken.None);
                }
                catch (Exception rollbackException)
                {
                    logger.LogError(
                        rollbackException,
                        "Failed to roll back the transaction for idempotency key {IdempotencyKey}.",
                        idempotencyKey);
                }

                throw;
            }
            finally
            {
                context.Response.Body = originalResponseBody;
            }
        }

        if (!keyReserved)
        {
            await ReplayOrRejectAsync(
                context,
                idempotencyUseCase,
                idempotencyKey,
                requestHash);

            return;
        }

        responseBuffer.Position = 0;

        await responseBuffer.CopyToAsync(
            originalResponseBody,
            context.RequestAborted);
    }

    private static bool TryGetKey(
        HttpRequest request,
        out string idempotencyKey)
    {
        idempotencyKey = string.Empty;

        if (!request.Headers.TryGetValue(HeaderName, out var values)
            || values.Count != 1)
        {
            return false;
        }

        var candidate = values[0]?.Trim();

        if (string.IsNullOrWhiteSpace(candidate)
            || candidate.Length > MaximumKeyLength)
        {
            return false;
        }

        idempotencyKey = candidate;
        return true;
    }

    private static async Task<string> ComputeRequestHashAsync(
        HttpRequest request,
        CancellationToken ct)
    {
        if (request.ContentLength > MaximumRequestBodySize)
        {
            throw new BadHttpRequestException(
                "The request body exceeds the maximum allowed size.",
                StatusCodes.Status413PayloadTooLarge);
        }

        request.EnableBuffering(
            bufferThreshold: BufferSize,
            bufferLimit: MaximumRequestBodySize);

        using var hash = IncrementalHash.CreateHash(
            HashAlgorithmName.SHA256);

        var requestMetadata = Encoding.UTF8.GetBytes(
            $"{request.Method}\n{request.Path}\n{request.QueryString}\n{request.ContentType}\n");

        hash.AppendData(requestMetadata);

        var buffer = new byte[BufferSize];
        long totalBytesRead = 0;

        try
        {
            int bytesRead;

            while ((bytesRead = await request.Body.ReadAsync(buffer, ct)) > 0)
            {
                totalBytesRead += bytesRead;

                if (totalBytesRead > MaximumRequestBodySize)
                {
                    throw new BadHttpRequestException(
                        "The request body exceeds the maximum allowed size.",
                        StatusCodes.Status413PayloadTooLarge);
                }

                hash.AppendData(buffer, 0, bytesRead);
            }
        }
        finally
        {
            request.Body.Position = 0;
        }

        return Convert.ToHexString(hash.GetHashAndReset());
    }

    private static async Task ReplayOrRejectAsync(
        HttpContext context,
        IIdempotencyUseCase idempotencyUseCase,
        string idempotencyKey,
        string requestHash)
    {
        var existing = await idempotencyUseCase.GetByKeyAsync(
            idempotencyKey,
            context.RequestAborted);

        if (existing is null)
        {
            await WriteConflictAsync(
                context,
                "A request with this Idempotency-Key is already being processed.");

            return;
        }

        if (!string.Equals(
            existing.RequestHash,
            requestHash,
            StringComparison.Ordinal))
        {
            await WriteConflictAsync(
                context,
                "This Idempotency-Key has already been used with a different request.");

            return;
        }

        if (existing.ResponseStatusCode is null)
        {
            await WriteConflictAsync(
                context,
                "A request with this Idempotency-Key is still being processed.");

            return;
        }

        context.Response.StatusCode = existing.ResponseStatusCode.Value;

        if (existing.ResponseContentType is not null)
        {
            context.Response.ContentType = existing.ResponseContentType;
        }

        if (existing.ResponseLocation is not null)
        {
            context.Response.Headers.Location = existing.ResponseLocation;
        }

        await context.Response.WriteAsync(
            existing.ResponseBody ?? string.Empty,
            context.RequestAborted);
    }

    private static Task WriteConflictAsync(
        HttpContext context,
        string detail)
    {
        return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Idempotency conflict.",
                detail: detail)
            .ExecuteAsync(context);
    }
}