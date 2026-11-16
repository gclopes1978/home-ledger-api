namespace HomeLedger.Api.Middleware;

public sealed class SecurityHeadersMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            var headers = context.Response.Headers;

            headers.XContentTypeOptions = "nosniff";
            headers.XFrameOptions = "DENY";
            headers["Referrer-Policy"] = "no-referrer";
            headers.ContentSecurityPolicy =
                "default-src 'none'; frame-ancestors 'none';";

            return Task.CompletedTask;
        });

        await next(context);
    }
}