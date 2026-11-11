namespace HomeLedger.IntegrationTests;

public sealed class ApiTestContext : IAsyncDisposable
{
    private readonly ApiFactory _factory;

    private ApiTestContext(ApiFactory factory, HttpClient client)
    {
        _factory = factory;
        Client = client;
    }

    public HttpClient Client { get; }

    public static async Task<ApiTestContext> CreateAsync(CancellationToken cancellationToken)
    {
        var factory = new ApiFactory();
        var client = factory.CreateClient();
        await factory.SeedAsync(cancellationToken);

        return new ApiTestContext(factory, client);
    }

    public async ValueTask DisposeAsync()
    {
        Client.Dispose();
        await _factory.DisposeAsync();
    }
}
