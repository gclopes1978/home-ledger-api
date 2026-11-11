using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HomeLedger.ApiContract.DTOs;
using Xunit;

namespace HomeLedger.IntegrationTests;

public sealed class ExpensesControllerTests
{
    [Fact]
    public async Task GetAll_GivenSeededExpenses_ShouldReturnExpenses()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.GetAsync("/api/expenses", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var expenses = await response.Content.ReadFromJsonAsync<List<ExpenseDto>>(cancellationToken: TestContext.Current.CancellationToken);
        var expense = expenses.Should().ContainSingle(item => item.Id == 1).Subject;
        expense.Category.Should().BeEquivalentTo(
            new CategoryDto(1, "Groceries", 1, new GroupingDto(1, "Home")));
        expense.Location.Should().BeEquivalentTo(
            new LocationDto(1, "Local Market", "Integration test location"));
        expense.PaymentType.Should().BeEquivalentTo(new PaymentTypeDto(1, "Debit card"));
    }

    [Fact]
    public async Task GetAll_GivenAnInclusiveDateRange_ShouldReturnExpensesWithinRange()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.GetAsync(
            "/api/expenses?startDate=2026-01-01&endDate=2026-01-31",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var expenses = await response.Content.ReadFromJsonAsync<List<ExpenseDto>>(
            cancellationToken: TestContext.Current.CancellationToken);

        expenses.Should().NotBeNull();
        expenses!.Select(expense => expense.Id).Should().BeEquivalentTo([1, 3]);
    }

    [Fact]
    public async Task Get_GivenExistingAndUnknownIds_ShouldReturnExpenseAndNotFound()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.GetAsync("/api/expenses/1", TestContext.Current.CancellationToken);
        var expense = await response.Content.ReadFromJsonAsync<ExpenseDto>(cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        expense.Should().NotBeNull();
        expense!.Total.Should().Be(20m);
        expense.Category.Should().BeEquivalentTo(
            new CategoryDto(1, "Groceries", 1, new GroupingDto(1, "Home")));
        expense.Location.Should().BeEquivalentTo(
            new LocationDto(1, "Local Market", "Integration test location"));
        expense.PaymentType.Should().BeEquivalentTo(new PaymentTypeDto(1, "Debit card"));
        (await context.Client.GetAsync("/api/expenses/999", TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_GivenAValidExpenseDto_ShouldReturnCreatedExpense()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.PostAsJsonAsync(
            "/api/expenses",
            new ExpenseDto(0, new DateOnly(2026, 2, 1), 1, 1, 1, 18m, 3m, 15m),
            cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var expense = await response.Content.ReadFromJsonAsync<ExpenseDto>(cancellationToken: TestContext.Current.CancellationToken);
        expense!.Total.Should().Be(15m);
        expense.Category.Should().BeEquivalentTo(
            new CategoryDto(1, "Groceries", 1, new GroupingDto(1, "Home")));
        expense.Location.Should().BeEquivalentTo(
            new LocationDto(1, "Local Market", "Integration test location"));
        expense.PaymentType.Should().BeEquivalentTo(new PaymentTypeDto(1, "Debit card"));
    }

    [Fact]
    public async Task Update_GivenExistingAndUnknownIds_ShouldReturnNoContentAndNotFound()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.PutAsJsonAsync(
            "/api/expenses/1",
            new ExpenseDto(1, new DateOnly(2026, 1, 1), 1, 1, 1, 30m, 5m, 25m),
            cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await context.Client.PutAsJsonAsync(
            "/api/expenses/999",
            new ExpenseDto(999, new DateOnly(2026, 1, 1), 1, 1, 1, 1m, 0m, 1m), cancellationToken: TestContext.Current.CancellationToken))
            .StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_GivenExistingAndDeletedIds_ShouldReturnNoContentAndNotFound()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.DeleteAsync("/api/expenses/1", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await context.Client.DeleteAsync("/api/expenses/1", TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
