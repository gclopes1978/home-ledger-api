using Microsoft.AspNetCore.Mvc;

namespace HomeLedger.Api.Middleware
{
    public class ExceptionsHandlerMiddleware(
        RequestDelegate next,
        ILogger<ExceptionsHandlerMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionsHandlerMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "An unhandled exception occurred while processing request {TraceId}",
                    context.TraceIdentifier);

                if (context.Response.HasStarted)
                {
                    throw;
                }

                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/problem+json";

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An unexpected error occurred.",
                    Detail = "The server encountered an unexpected error. Please try again later.",
                    Instance = context.Request.Path
                };

                problemDetails.Extensions["traceId"] = context.TraceIdentifier;

                await context.Response.WriteAsJsonAsync(problemDetails, context.RequestAborted);
            }
        }
    }
}