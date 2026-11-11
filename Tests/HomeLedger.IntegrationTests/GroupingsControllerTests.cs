using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HomeLedger.ApiContract.DTOs;
using Xunit;

namespace HomeLedger.IntegrationTests;

public sealed class GroupingsControllerTests
{
    [Fact]
    public async Task GetAll_GivenSeededGroupings_ShouldReturnGroupings()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.GetAsync("/api/groupings", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var groupings = await response.Content.ReadFromJsonAsync<List<GroupingDto>>(cancellationToken: TestContext.Current.CancellationToken);
        groupings.Should().Contain(grouping => grouping.Id == 1);
    }

    [Fact]
    public async Task Get_GivenExistingAndUnknownIds_ShouldReturnGroupingAndNotFound()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.GetAsync("/api/groupings/1", TestContext.Current.CancellationToken);
        var grouping = await response.Content.ReadFromJsonAsync<GroupingDto>(cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        grouping.Should().NotBeNull();
        grouping!.Name.Should().Be("Home");
        (await context.Client.GetAsync("/api/groupings/999", TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_GivenAValidGroupingDto_ShouldReturnCreatedGrouping()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.PostAsJsonAsync(
            "/api/groupings",
            new GroupingDto(0, "Transport"),
            cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var grouping = await response.Content.ReadFromJsonAsync<GroupingDto>(cancellationToken: TestContext.Current.CancellationToken);
        grouping!.Name.Should().Be("Transport");
    }

    [Fact]
    public async Task Update_GivenExistingAndUnknownIds_ShouldReturnNoContentAndNotFound()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.PutAsJsonAsync(
            "/api/groupings/1",
            new GroupingDto(1, "Household"),
            cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await context.Client.PutAsJsonAsync("/api/groupings/999", new GroupingDto(999, "Missing"), cancellationToken: TestContext.Current.CancellationToken))
            .StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_GivenExistingAndDeletedIds_ShouldReturnNoContentAndNotFound()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.DeleteAsync("/api/groupings/1", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await context.Client.DeleteAsync("/api/groupings/1", TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
