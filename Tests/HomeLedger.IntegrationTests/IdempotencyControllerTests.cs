using FluentAssertions;
using HomeLedger.ApiContract.DTOs;
using System.Net;
using System.Net.Http.Json;
using System.Text;
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

    [Fact]
    public async Task Create_GivenConcurrentRequestsWithSameKeyAndPayload_ShouldCreateOnlyOnceAndReplayResponse()
    {
        await using var context = await ApiTestContext.CreateAsync(
            TestContext.Current.CancellationToken);

        var cancellationToken = TestContext.Current.CancellationToken;

        using var startSignal = new ManualResetEventSlim(false);

        var payload = new CategoryDto(
            0,
            "Concurrent idempotent category",
            1);

        const string idempotencyKey = "concurrent-category-create";

        var firstTask = SendRequestAsync();
        var secondTask = SendRequestAsync();

        startSignal.Set();

        using var first = await firstTask;
        using var second = await secondTask;

        var firstBody = await first.Content.ReadAsStringAsync(cancellationToken);
        var secondBody = await second.Content.ReadAsStringAsync(cancellationToken);

        first.StatusCode.Should().Be(HttpStatusCode.Created);
        second.StatusCode.Should().Be(HttpStatusCode.Created);

        secondBody.Should().Be(firstBody);
        second.Headers.Location.Should().Be(first.Headers.Location);

        var categoryCount = await context.CountCategoriesByNameAsync(
            "Concurrent idempotent category",
            cancellationToken);

        categoryCount.Should().Be(1);

        async Task<HttpResponseMessage> SendRequestAsync()
        {
            await Task.Run(
                () => startSignal.Wait(cancellationToken),
                cancellationToken);

            return await context.Client.PostAsJsonWithIdempotencyKeyAsync(
                "/api/categories",
                payload,
                idempotencyKey,
                cancellationToken);
        }
    }

    [Fact]
    public async Task Create_GivenConcurrentRequestsWithSameKeyAndDifferentPayloads_ShouldCreateOnlyOnceAndReturnConflict()
    {
        await using var context = await ApiTestContext.CreateAsync(
            TestContext.Current.CancellationToken);

        var cancellationToken = TestContext.Current.CancellationToken;

        using var startSignal = new ManualResetEventSlim(false);

        const string idempotencyKey = "concurrent-different-payloads";

        var firstPayload = new CategoryDto(
            0,
            "Concurrent category A",
            1);

        var secondPayload = new CategoryDto(
            0,
            "Concurrent category B",
            1);

        var firstTask = SendRequestAsync(firstPayload);
        var secondTask = SendRequestAsync(secondPayload);

        startSignal.Set();

        using var first = await firstTask;
        using var second = await secondTask;

        var statuses = new[]
        {
            first.StatusCode,
            second.StatusCode
        };

        statuses.Should().Contain(HttpStatusCode.Created);
        statuses.Should().Contain(HttpStatusCode.Conflict);

        var categoryCountA = await context.CountCategoriesByNameAsync(
            "Concurrent category A",
            cancellationToken);

        var categoryCountB = await context.CountCategoriesByNameAsync(
            "Concurrent category B",
            cancellationToken);

        (categoryCountA + categoryCountB).Should().Be(1);

        async Task<HttpResponseMessage> SendRequestAsync(CategoryDto payload)
        {
            await Task.Run(
                () => startSignal.Wait(cancellationToken),
                cancellationToken);

            return await context.Client.PostAsJsonWithIdempotencyKeyAsync(
                "/api/categories",
                payload,
                idempotencyKey,
                cancellationToken);
        }
    }

    [Fact]
    public async Task Create_GivenRequestFailsWithServerError_ShouldAllowRetryWithSameKey()
    {
        await using var context = await ApiTestContext.CreateAsync(
            TestContext.Current.CancellationToken);

        var cancellationToken = TestContext.Current.CancellationToken;

        const string idempotencyKey = "category-retry-after-server-error";

        var invalidPayload = new CategoryDto(
            0,
            "Retry after server error",
            int.MaxValue);

        using var failedResponse =
            await context.Client.PostAsJsonWithIdempotencyKeyAsync(
                "/api/categories",
                invalidPayload,
                idempotencyKey,
                cancellationToken);

        failedResponse.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var validPayload = invalidPayload with { GroupingId = 1 };

        using var successfulResponse =
            await context.Client.PostAsJsonWithIdempotencyKeyAsync(
                "/api/categories",
                validPayload,
                idempotencyKey,
                cancellationToken);

        successfulResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var successfulBody = await successfulResponse.Content
            .ReadAsStringAsync(cancellationToken);

        using var replayResponse =
            await context.Client.PostAsJsonWithIdempotencyKeyAsync(
                "/api/categories",
                validPayload,
                idempotencyKey,
                cancellationToken);

        replayResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var replayBody = await replayResponse.Content
            .ReadAsStringAsync(cancellationToken);

        replayBody.Should().Be(successfulBody);
        replayResponse.Headers.Location.Should()
            .Be(successfulResponse.Headers.Location);

        var categoryCount = await context.CountCategoriesByNameAsync(
            "Retry after server error",
            cancellationToken);

        categoryCount.Should().Be(1);
    }

    [Fact]
    public async Task Create_GivenConcurrentRequestsWithSameKeyAndOneInvalidPayload_ShouldAllowSuccessfulRequest()
    {
        await using var context = await ApiTestContext.CreateAsync(
            TestContext.Current.CancellationToken);

        var cancellationToken = TestContext.Current.CancellationToken;

        using var startSignal = new ManualResetEventSlim(false);

        const string idempotencyKey = "concurrent-failure-and-success";

        var invalidPayload = new CategoryDto(
            0,
            "Concurrent recovery category",
            int.MaxValue);

        var validPayload = invalidPayload with { GroupingId = 1 };

        var invalidTask = SendRequestAsync(invalidPayload);
        var validTask = SendRequestAsync(validPayload);

        startSignal.Set();

        using var invalidResponse = await invalidTask;
        using var validResponse = await validTask;

        validResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        invalidResponse.StatusCode.Should().BeOneOf(
            HttpStatusCode.InternalServerError,
            HttpStatusCode.Conflict);

        var successfulBody = await validResponse.Content
            .ReadAsStringAsync(cancellationToken);

        using var retryResponse =
            await context.Client.PostAsJsonWithIdempotencyKeyAsync(
                "/api/categories",
                validPayload,
                idempotencyKey,
                cancellationToken);

        retryResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var retryBody = await retryResponse.Content
            .ReadAsStringAsync(cancellationToken);

        retryBody.Should().Be(successfulBody);
        retryResponse.Headers.Location.Should().Be(validResponse.Headers.Location);

        var categoryCount = await context.CountCategoriesByNameAsync(
            "Concurrent recovery category",
            cancellationToken);

        categoryCount.Should().Be(1);

        async Task<HttpResponseMessage> SendRequestAsync(CategoryDto payload)
        {
            await Task.Run(
                () => startSignal.Wait(cancellationToken),
                cancellationToken);

            return await context.Client.PostAsJsonWithIdempotencyKeyAsync(
                "/api/categories",
                payload,
                idempotencyKey,
                cancellationToken);
        }
    }

    [Fact]
    public async Task Create_GivenRequestBodyExceedsMaximumSize_ShouldReturnPayloadTooLarge()
    {
        await using var context = await ApiTestContext.CreateAsync(
            TestContext.Current.CancellationToken);

        var cancellationToken = TestContext.Current.CancellationToken;

        var oversizedPayload = new string('A', 1_048_577);

        using var content = new StringContent(
            oversizedPayload,
            Encoding.UTF8,
            "application/json");

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/categories")
        {
            Content = content
        };

        request.Headers.Add("Idempotency-Key", "oversized-request");

        using var response = await context.Client.SendAsync(
            request,
            cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.RequestEntityTooLarge);
    }
}