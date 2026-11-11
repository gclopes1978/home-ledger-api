using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HomeLedger.ApiContract.DTOs;
using Xunit;

namespace HomeLedger.IntegrationTests;

public sealed class CategoriesControllerTests
{
    [Fact]
    public async Task GetAll_GivenSeededCategories_ShouldReturnCategories()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.GetAsync("/api/categories", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var categories = await response.Content.ReadFromJsonAsync<List<CategoryDto>>(cancellationToken: TestContext.Current.CancellationToken);
        var category = categories.Should().ContainSingle(item => item.Id == 1).Subject;
        category.Grouping.Should().BeEquivalentTo(new GroupingDto(1, "Home"));
    }

    [Fact]
    public async Task Get_GivenExistingAndUnknownIds_ShouldReturnCategoryAndNotFound()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.GetAsync("/api/categories/1", TestContext.Current.CancellationToken);
        var category = await response.Content.ReadFromJsonAsync<CategoryDto>(cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        category.Should().NotBeNull();
        category.Name.Should().Be("Groceries");
        category.Grouping.Should().BeEquivalentTo(new GroupingDto(1, "Home"));
        (await context.Client.GetAsync("/api/categories/999", TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_GivenAValidCategoryDto_ShouldReturnCreatedCategory()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.PostAsJsonAsync("/api/categories", new CategoryDto(0, "Utilities", 1),
            cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var category = await response.Content.ReadFromJsonAsync<CategoryDto>(cancellationToken: TestContext.Current.CancellationToken);
        category!.Name.Should().Be("Utilities");
        category.Grouping.Should().BeEquivalentTo(new GroupingDto(1, "Home"));
    }

    [Fact]
    public async Task Update_GivenExistingAndUnknownIds_ShouldReturnNoContentAndNotFound()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.PutAsJsonAsync("/api/categories/1", new CategoryDto(1, "Food", 1),
            cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await context.Client.PutAsJsonAsync("/api/categories/999", new CategoryDto(999, "Missing", 1), cancellationToken: TestContext.Current.CancellationToken))
            .StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_GivenExistingAndDeletedIds_ShouldReturnNoContentAndNotFound()
    {
        await using var context = await ApiTestContext.CreateAsync(TestContext.Current.CancellationToken);
        var response = await context.Client.DeleteAsync("/api/categories/1", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await context.Client.DeleteAsync("/api/categories/1", TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
