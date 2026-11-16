using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HomeLedger.ApiContract.DTOs;
using Xunit;

namespace HomeLedger.IntegrationTests;

public sealed class SourcesControllerTests
{
    [Fact]
    public async Task GetAll_GivenSeededSources_ShouldReturnSources()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.GetAsync("/api/sources", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var sources = await response.Content.ReadFromJsonAsync<List<SourceDto>>(cancellationToken: TestContext.Current.CancellationToken);
        sources.Should().Contain(source => source.Id == 1);
    }

    [Fact]
    public async Task Get_GivenExistingAndUnknownIds_ShouldReturnSourceAndNotFound()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.GetAsync("/api/sources/1", TestContext.Current.CancellationToken);
        var source = await response.Content.ReadFromJsonAsync<SourceDto>(cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        source.Should().NotBeNull();
        source!.Name.Should().Be("Local Market");
        (await context.Client.GetAsync("/api/sources/999", TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_GivenAValidSourceDto_ShouldReturnCreatedSource()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.PostAsJsonWithIdempotencyKeyAsync(
            "/api/sources",
            new SourceDto(0, "Pharmacy", "Town center"),
            cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var source = await response.Content.ReadFromJsonAsync<SourceDto>(cancellationToken: TestContext.Current.CancellationToken);
        source!.Name.Should().Be("Pharmacy");
    }

    [Fact]
    public async Task Update_GivenExistingAndUnknownIds_ShouldReturnNoContentAndNotFound()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.PutAsJsonAsync(
            "/api/sources/1",
            new SourceDto(1, "Updated Market", "New description"),
            cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await context.Client.PutAsJsonAsync("/api/sources/999", new SourceDto(999, "Missing", null), cancellationToken: TestContext.Current.CancellationToken))
            .StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_GivenExistingAndDeletedIds_ShouldReturnNoContentAndNotFound()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.DeleteAsync("/api/sources/1", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await context.Client.DeleteAsync("/api/sources/1", TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
