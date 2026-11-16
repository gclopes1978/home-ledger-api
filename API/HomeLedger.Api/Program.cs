using HomeLedger.Api.Mapping;
using HomeLedger.Api.Middleware;
using HomeLedger.Application.Extensions;
using HomeLedger.Infrastructure.Data;
using HomeLedger.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendOnly", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE")
            .WithHeaders(
                "Content-Type",
                "Authorization",
                "Idempotency-Key")
            .WithExposedHeaders("Location");
    });
});

builder.Services.AddOpenApi();

builder.Services.AddAutoMapper(_ => { }, typeof(ApiMappingProfile));

builder.Services.AddHomeLedgerApplication();

builder.Services.AddHomeLedgerInfrastructure(builder.Configuration);

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.GlobalLimiter =
        PartitionedRateLimiter.Create<HttpContext, string>(
            httpContext =>
            {
                var ipAddress =
                    httpContext.Connection.RemoteIpAddress?.ToString()
                    ?? "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(
                    ipAddress,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    });
            });
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.AddServerHeader = false;

    // Limite máximo do request body: 1 MB
    options.Limits.MaxRequestBodySize = 1_048_576;

    // Timeout para receber os headers
    options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(15);
});

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HomeLedgerDbContext>();

    if (db.Database.IsRelational())
    {
        await db.Database.MigrateAsync();
    }
    else
    {
        await db.Database.EnsureCreatedAsync();
    }
}

app.UseMiddleware<ExceptionsHandlerMiddleware>();

app.UseMiddleware<SecurityHeadersMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseHsts();
}

app.UseCors("FrontendOnly");

app.UseRateLimiter();

app.UseMiddleware<IdempotencyMiddleware>();

app.MapControllers();

await app.RunAsync();