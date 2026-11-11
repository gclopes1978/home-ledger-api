using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HomeLedger.ApiContract.DTOs;
using Xunit;

namespace HomeLedger.IntegrationTests;

public sealed class IncomesControllerTests
{
    [Fact]
    public async Task GetAll_GivenSeededIncomes_ShouldReturnIncomes()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.GetAsync("/api/incomes", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var incomes = await response.Content.ReadFromJsonAsync<List<IncomeDto>>(cancellationToken: TestContext.Current.CancellationToken);
        incomes.Should().Contain(income => income.Id == 1);
    }

    [Fact]
    public async Task GetAll_GivenAnInclusiveDateRange_ShouldReturnIncomesWithinRange()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.GetAsync(
            "/api/incomes?startDate=2026-01-01&endDate=2026-01-31",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var incomes = await response.Content.ReadFromJsonAsync<List<IncomeDto>>(
            cancellationToken: TestContext.Current.CancellationToken);

        incomes.Should().NotBeNull();
        incomes!.Select(income => income.Id).Should().BeEquivalentTo([1, 3]);
    }

    [Fact]
    public async Task Get_GivenExistingAndUnknownIds_ShouldReturnIncomeAndNotFound()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.GetAsync("/api/incomes/1", TestContext.Current.CancellationToken);
        var income = await response.Content.ReadFromJsonAsync<IncomeDto>(cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        income.Should().NotBeNull();
        income!.Description.Should().Be("Monthly salary");
        (await context.Client.GetAsync("/api/incomes/999", TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_GivenAValidIncomeDto_ShouldReturnCreatedIncome()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.PostAsJsonAsync(
            "/api/incomes",
            new IncomeDto(0, new DateOnly(2026, 2, 1), "Freelance work", 450m),
            cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var income = await response.Content.ReadFromJsonAsync<IncomeDto>(cancellationToken: TestContext.Current.CancellationToken);
        income!.Description.Should().Be("Freelance work");
    }

    [Fact]
    public async Task Update_GivenExistingAndUnknownIds_ShouldReturnNoContentAndNotFound()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.PutAsJsonAsync(
            "/api/incomes/1",
            new IncomeDto(1, new DateOnly(2026, 1, 1), "Updated salary", 2700m),
            cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await context.Client.PutAsJsonAsync(
            "/api/incomes/999",
            new IncomeDto(999, new DateOnly(2026, 1, 1), "Missing", 1m), cancellationToken: TestContext.Current.CancellationToken))
            .StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_GivenExistingAndDeletedIds_ShouldReturnNoContentAndNotFound()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.DeleteAsync("/api/incomes/1", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await context.Client.DeleteAsync("/api/incomes/1", TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
