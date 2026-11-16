using System.Net.Http.Json;

namespace HomeLedger.IntegrationTests;

internal static class HttpClientIdempotencyExtensions
{
    public static Task<HttpResponseMessage> PostAsJsonWithIdempotencyKeyAsync<T>(
        this HttpClient client,
        string requestUri,
        T value,
        CancellationToken cancellationToken = default)
    {
        return PostAsJsonWithIdempotencyKeyAsync(
            client,
            requestUri,
            value,
            Guid.NewGuid().ToString("N"),
            cancellationToken);
    }

    public static async Task<HttpResponseMessage> PostAsJsonWithIdempotencyKeyAsync<T>(
        this HttpClient client,
        string requestUri,
        T value,
        string idempotencyKey,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = JsonContent.Create(value)
        };
        request.Headers.Add("Idempotency-Key", idempotencyKey);

        return await client.SendAsync(request, cancellationToken);
    }
}