using Microsoft.AspNetCore.Mvc;

namespace HomeLedger.Api.Middleware;

public sealed class ExceptionsHandlerMiddleware(
    RequestDelegate next,
    ILogger<ExceptionsHandlerMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            if (context.Response.HasStarted)
            {
                throw;
            }

            var statusCode = GetStatusCode(exception);

            LogException(exception, context, statusCode);

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";

            var problemDetails = CreateProblemDetails(statusCode, context.Request.Path);

            problemDetails.Extensions["traceId"] = context.TraceIdentifier;

            await context.Response.WriteAsJsonAsync(
                problemDetails,
                context.RequestAborted);
        }
    }

    private static int GetStatusCode(Exception exception)
    {
        if (exception is BadHttpRequestException badRequest)
        {
            return badRequest.StatusCode;
        }

        return StatusCodes.Status500InternalServerError;
    }

    private void LogException(
        Exception exception,
        HttpContext context,
        int statusCode)
    {
        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(
                exception,
                "An unhandled exception occurred while processing request {TraceId}",
                context.TraceIdentifier);

            return;
        }

        logger.LogWarning(
            "Request failed with HTTP {StatusCode}. TraceId: {TraceId}",
            statusCode,
            context.TraceIdentifier);
    }

    private static ProblemDetails CreateProblemDetails(
        int statusCode,
        PathString requestPath)
    {
        var isPayloadTooLarge =
            statusCode == StatusCodes.Status413PayloadTooLarge;

        var isServerError =
            statusCode >= StatusCodes.Status500InternalServerError;

        var title = GetProblemTitle(isPayloadTooLarge, isServerError);
        var detail = GetProblemDetail(isPayloadTooLarge, isServerError);

        return new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = requestPath
        };
    }

    private static string GetProblemTitle(bool isPayloadTooLarge, bool isServerError)
    {
        if (isPayloadTooLarge)
        {
            return "Request body too large.";
        }

        if (isServerError)
        {
            return "An unexpected error occurred.";
        }

        return "Invalid request.";
    }

    private static string GetProblemDetail(bool isPayloadTooLarge, bool isServerError)
    {
        if (isPayloadTooLarge)
        {
            return "The request body exceeds the maximum allowed size.";
        }

        if (isServerError)
        {
            return "The server encountered an unexpected error. Please try again later.";
        }

        return "The request could not be processed.";
    }
}