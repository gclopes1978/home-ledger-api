using HomeLedger.Application.UseCases;
using HomeLedger.Domain.BusinessModels;
using HomeLedger.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace HomeLedger.UnitTests;

public sealed class GroupingUseCaseTests
{
    [Fact]
    public async Task GetAllAsync_GivenGroupingsExist_ShouldReturnAllGroupings()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<IGroupingRepository>();
        var model = new Grouping { Id = 1, Name = "Home" };
        IReadOnlyList<Grouping> list = [model];
        repository.Setup(x => x.GetAllAsync(cancellationToken)).ReturnsAsync(list);
        var useCase = new GroupingUseCase(repository.Object);

        (await useCase.GetAllAsync(cancellationToken)).Should().BeSameAs(list);
        repository.Verify(x => x.GetAllAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetAsync_GivenAnExistingId_ShouldReturnGroupingWithGivenId()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<IGroupingRepository>();
        var model = new Grouping { Id = 1, Name = "Home" };
        repository.Setup(x => x.GetAsync(1, cancellationToken)).ReturnsAsync(model);
        var useCase = new GroupingUseCase(repository.Object);

        (await useCase.GetAsync(1, cancellationToken)).Should().BeSameAs(model);
        repository.Verify(x => x.GetAsync(1, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_GivenAValidGrouping_ShouldReturnInsertedGrouping()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<IGroupingRepository>();
        var model = new Grouping { Id = 1, Name = "Home" };
        repository.Setup(x => x.InsertAsync(model, cancellationToken)).ReturnsAsync(model);
        var useCase = new GroupingUseCase(repository.Object);

        (await useCase.InsertAsync(model, cancellationToken)).Should().BeSameAs(model);
        repository.Verify(x => x.InsertAsync(model, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_GivenAnExistingId_ShouldReturnTrue()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<IGroupingRepository>();
        var model = new Grouping { Id = 1, Name = "Home" };
        repository.Setup(x => x.UpdateAsync(1, model, cancellationToken)).ReturnsAsync(true);
        var useCase = new GroupingUseCase(repository.Object);

        (await useCase.UpdateAsync(1, model, cancellationToken)).Should().BeTrue();
        repository.Verify(x => x.UpdateAsync(1, model, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_GivenAnExistingId_ShouldReturnTrue()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<IGroupingRepository>();
        repository.Setup(x => x.DeleteAsync(1, cancellationToken)).ReturnsAsync(true);
        var useCase = new GroupingUseCase(repository.Object);

        (await useCase.DeleteAsync(1, cancellationToken)).Should().BeTrue();
        repository.Verify(x => x.DeleteAsync(1, cancellationToken), Times.Once);
    }
}
