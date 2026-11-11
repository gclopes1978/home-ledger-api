using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HomeLedger.ApiContract.DTOs;
using Xunit;

namespace HomeLedger.IntegrationTests;

public sealed class LocationsControllerTests
{
    [Fact]
    public async Task GetAll_GivenSeededLocations_ShouldReturnLocations()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.GetAsync("/api/locations", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var locations = await response.Content.ReadFromJsonAsync<List<LocationDto>>(cancellationToken: TestContext.Current.CancellationToken);
        locations.Should().Contain(location => location.Id == 1);
    }

    [Fact]
    public async Task Get_GivenExistingAndUnknownIds_ShouldReturnLocationAndNotFound()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.GetAsync("/api/locations/1", TestContext.Current.CancellationToken);
        var location = await response.Content.ReadFromJsonAsync<LocationDto>(cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        location.Should().NotBeNull();
        location!.Name.Should().Be("Local Market");
        (await context.Client.GetAsync("/api/locations/999", TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_GivenAValidLocationDto_ShouldReturnCreatedLocation()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.PostAsJsonAsync(
            "/api/locations",
            new LocationDto(0, "Pharmacy", "Town center"),
            cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var location = await response.Content.ReadFromJsonAsync<LocationDto>(cancellationToken: TestContext.Current.CancellationToken);
        location!.Name.Should().Be("Pharmacy");
    }

    [Fact]
    public async Task Update_GivenExistingAndUnknownIds_ShouldReturnNoContentAndNotFound()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.PutAsJsonAsync(
            "/api/locations/1",
            new LocationDto(1, "Updated Market", "New description"),
            cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await context.Client.PutAsJsonAsync("/api/locations/999", new LocationDto(999, "Missing", null), cancellationToken: TestContext.Current.CancellationToken))
            .StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_GivenExistingAndDeletedIds_ShouldReturnNoContentAndNotFound()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.DeleteAsync("/api/locations/1", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await context.Client.DeleteAsync("/api/locations/1", TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
