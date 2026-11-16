using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HomeLedger.ApiContract.DTOs;
using Xunit;

namespace HomeLedger.IntegrationTests;

public sealed class IdempotencyControllerTests
{
    [Fact]
    public async Task Create_GivenTheSameKeyAndPayloadForEachEntity_ShouldReplayEachCreatedResponse()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var cancellationToken = TestContext.Current.CancellationToken;

        await AssertResponseIsReplayedAsync(
            context.Client,
            "/api/categories",
            new CategoryDto(0, "Idempotent category", 1),
            "category-create",
            cancellationToken);
        await AssertResponseIsReplayedAsync(
            context.Client,
            "/api/groupings",
            new GroupingDto(0, "Idempotent grouping"),
            "grouping-create",
            cancellationToken);
        await AssertResponseIsReplayedAsync(
            context.Client,
            "/api/sources",
            new SourceDto(0, "Idempotent source", "Created once"),
            "source-create",
            cancellationToken);
        await AssertResponseIsReplayedAsync(
            context.Client,
            "/api/expenses",
            new ExpenseDto(0, new DateOnly(2026, 3, 1), 1, 1, 1, 10m, 0m, 10m),
            "expense-create",
            cancellationToken);
        await AssertResponseIsReplayedAsync(
            context.Client,
            "/api/incomes",
            new IncomeDto(0, new DateOnly(2026, 3, 1), "Idempotent income", 10m),
            "income-create",
            cancellationToken);
        await AssertResponseIsReplayedAsync(
            context.Client,
            "/api/paymenttypes",
            new PaymentTypeDto(0, "Idempotent payment type"),
            "payment-type-create",
            cancellationToken);
    }

    [Fact]
    public async Task Create_GivenAKeyReusedWithDifferentPayload_ShouldReturnConflict()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var cancellationToken = TestContext.Current.CancellationToken;

        using var first = await context.Client.PostAsJsonWithIdempotencyKeyAsync(
            "/api/categories",
            new CategoryDto(0, "First category", 1),
            "same-category-key",
            cancellationToken);
        using var second = await context.Client.PostAsJsonWithIdempotencyKeyAsync(
            "/api/categories",
            new CategoryDto(0, "Different category", 1),
            "same-category-key",
            cancellationToken);

        first.StatusCode.Should().Be(HttpStatusCode.Created);
        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Create_GivenNoIdempotencyKey_ShouldReturnBadRequest()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);

        using var response = await context.Client.PostAsJsonAsync(
            "/api/categories",
            new CategoryDto(0, "Missing key category", 1),
            cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private static async Task AssertResponseIsReplayedAsync<T>(
        HttpClient client,
        string requestUri,
        T payload,
        string idempotencyKey,
        CancellationToken cancellationToken)
    {
        using var first = await client.PostAsJsonWithIdempotencyKeyAsync(
            requestUri,
            payload,
            idempotencyKey,
            cancellationToken);
        var firstBody = await first.Content.ReadAsStringAsync(cancellationToken);

        using var retry = await client.PostAsJsonWithIdempotencyKeyAsync(
            requestUri,
            payload,
            idempotencyKey,
            cancellationToken);
        var retryBody = await retry.Content.ReadAsStringAsync(cancellationToken);

        first.StatusCode.Should().Be(HttpStatusCode.Created);
        retry.StatusCode.Should().Be(first.StatusCode);
        retryBody.Should().Be(firstBody);
        retry.Headers.Location.Should().Be(first.Headers.Location);
    }
}