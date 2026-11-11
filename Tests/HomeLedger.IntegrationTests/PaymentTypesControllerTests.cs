using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HomeLedger.ApiContract.DTOs;
using Xunit;

namespace HomeLedger.IntegrationTests;

public sealed class PaymentTypesControllerTests
{
    [Fact]
    public async Task GetAll_GivenSeededPaymentTypes_ShouldReturnPaymentTypes()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.GetAsync("/api/paymenttypes", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var paymentTypes = await response.Content.ReadFromJsonAsync<List<PaymentTypeDto>>(cancellationToken: TestContext.Current.CancellationToken);
        paymentTypes.Should().Contain(paymentType => paymentType.Id == 1);
    }

    [Fact]
    public async Task Get_GivenExistingAndUnknownIds_ShouldReturnPaymentTypeAndNotFound()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.GetAsync("/api/paymenttypes/1", TestContext.Current.CancellationToken);
        var paymentType = await response.Content.ReadFromJsonAsync<PaymentTypeDto>(cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        paymentType.Should().NotBeNull();
        paymentType!.Description.Should().Be("Debit card");
        (await context.Client.GetAsync("/api/paymenttypes/999", TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_GivenAValidPaymentTypeDto_ShouldReturnCreatedPaymentType()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.PostAsJsonAsync(
            "/api/paymenttypes",
            new PaymentTypeDto(0, "Cash"),
            cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var paymentType = await response.Content.ReadFromJsonAsync<PaymentTypeDto>(cancellationToken: TestContext.Current.CancellationToken);
        paymentType!.Description.Should().Be("Cash");
    }

    [Fact]
    public async Task Update_GivenExistingAndUnknownIds_ShouldReturnNoContentAndNotFound()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.PutAsJsonAsync(
            "/api/paymenttypes/1",
            new PaymentTypeDto(1, "Credit card"),
            cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await context.Client.PutAsJsonAsync("/api/paymenttypes/999", new PaymentTypeDto(999, "Missing"), cancellationToken: TestContext.Current.CancellationToken))
            .StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_GivenExistingAndDeletedIds_ShouldReturnNoContentAndNotFound()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.DeleteAsync("/api/paymenttypes/1", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await context.Client.DeleteAsync("/api/paymenttypes/1", TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
