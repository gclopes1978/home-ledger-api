using HomeLedger.Application.UseCases;
using HomeLedger.Domain.BusinessModels;
using HomeLedger.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace HomeLedger.UnitTests;

public sealed class CategoryUseCaseTests
{
    [Fact]
    public async Task GetAllAsync_GivenCategoriesExist_ShouldReturnAllCategories()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<ICategoryRepository>();
        var model = new Category
        {
            Id = 1,
            Name = "Food",
            GroupingId = 2,
            Grouping = new Grouping { Id = 2, Name = "Home" }
        };
        IReadOnlyList<Category> list = [model];
        repository.Setup(x => x.GetAllAsync(cancellationToken)).ReturnsAsync(list);
        var useCase = new CategoryUseCase(repository.Object);

        var result = await useCase.GetAllAsync(cancellationToken);
        result.Should().BeSameAs(list);
        result[0].Grouping.Should().BeEquivalentTo(model.Grouping);
        repository.Verify(x => x.GetAllAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetAsync_GivenAnExistingId_ShouldReturnCategoryWithGivenId()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<ICategoryRepository>();
        var model = new Category
        {
            Id = 1,
            Name = "Food",
            GroupingId = 2,
            Grouping = new Grouping { Id = 2, Name = "Home" }
        };
        repository.Setup(x => x.GetAsync(1, cancellationToken)).ReturnsAsync(model);
        var useCase = new CategoryUseCase(repository.Object);

        (await useCase.GetAsync(1, cancellationToken)).Should().BeSameAs(model);
        model.Grouping.Should().NotBeNull();
        repository.Verify(x => x.GetAsync(1, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_GivenAValidCategory_ShouldReturnInsertedCategory()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<ICategoryRepository>();
        var model = new Category
        {
            Id = 1,
            Name = "Food",
            GroupingId = 2,
            Grouping = new Grouping { Id = 2, Name = "Home" }
        };
        repository.Setup(x => x.InsertAsync(model, cancellationToken)).ReturnsAsync(model);
        var useCase = new CategoryUseCase(repository.Object);

        var result = await useCase.InsertAsync(model, cancellationToken);
        result.Should().BeSameAs(model);
        result.Grouping.Should().BeEquivalentTo(model.Grouping);
        repository.Verify(x => x.InsertAsync(model, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_GivenAnExistingId_ShouldReturnTrue()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<ICategoryRepository>();
        var model = new Category
        {
            Id = 1,
            Name = "Food",
            GroupingId = 2,
            Grouping = new Grouping { Id = 2, Name = "Home" }
        };
        repository.Setup(x => x.UpdateAsync(1, model, cancellationToken)).ReturnsAsync(true);
        var useCase = new CategoryUseCase(repository.Object);

        (await useCase.UpdateAsync(1, model, cancellationToken)).Should().BeTrue();
        repository.Verify(x => x.UpdateAsync(1, model, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_GivenAnExistingId_ShouldReturnTrue()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<ICategoryRepository>();
        repository.Setup(x => x.DeleteAsync(1, cancellationToken)).ReturnsAsync(true);
        var useCase = new CategoryUseCase(repository.Object);

        (await useCase.DeleteAsync(1, cancellationToken)).Should().BeTrue();
        repository.Verify(x => x.DeleteAsync(1, cancellationToken), Times.Once);
    }
}
