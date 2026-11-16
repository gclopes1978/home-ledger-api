using HomeLedger.Application.UseCases;
using HomeLedger.Domain.BusinessModels;
using HomeLedger.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace HomeLedger.UnitTests;

public sealed class SourceUseCaseTests
{
    [Fact]
    public async Task GetAllAsync_GivenSourcesExist_ShouldReturnAllSources()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<ISourceRepository>();
        var model = new Source { Id = 1, Name = "Market", Description = "Shop" };
        IReadOnlyList<Source> list = [model];
        repository.Setup(x => x.GetAllAsync(cancellationToken)).ReturnsAsync(list);
        var useCase = new SourceUseCase(repository.Object);

        (await useCase.GetAllAsync(cancellationToken)).Should().BeSameAs(list);
        repository.Verify(x => x.GetAllAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetAsync_GivenAnExistingId_ShouldReturnSourceWithGivenId()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<ISourceRepository>();
        var model = new Source { Id = 1, Name = "Market", Description = "Shop" };
        repository.Setup(x => x.GetAsync(1, cancellationToken)).ReturnsAsync(model);
        var useCase = new SourceUseCase(repository.Object);

        (await useCase.GetAsync(1, cancellationToken)).Should().BeSameAs(model);
        repository.Verify(x => x.GetAsync(1, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_GivenAValidSource_ShouldReturnInsertedSource()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<ISourceRepository>();
        var model = new Source { Id = 1, Name = "Market", Description = "Shop" };
        repository.Setup(x => x.InsertAsync(model, cancellationToken)).ReturnsAsync(model);
        var useCase = new SourceUseCase(repository.Object);

        (await useCase.InsertAsync(model, cancellationToken)).Should().BeSameAs(model);
        repository.Verify(x => x.InsertAsync(model, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_GivenAnExistingId_ShouldReturnTrue()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<ISourceRepository>();
        var model = new Source { Id = 1, Name = "Market", Description = "Shop" };
        repository.Setup(x => x.UpdateAsync(1, model, cancellationToken)).ReturnsAsync(true);
        var useCase = new SourceUseCase(repository.Object);

        (await useCase.UpdateAsync(1, model, cancellationToken)).Should().BeTrue();
        repository.Verify(x => x.UpdateAsync(1, model, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_GivenAnExistingId_ShouldReturnTrue()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<ISourceRepository>();
        repository.Setup(x => x.DeleteAsync(1, cancellationToken)).ReturnsAsync(true);
        var useCase = new SourceUseCase(repository.Object);

        (await useCase.DeleteAsync(1, cancellationToken)).Should().BeTrue();
        repository.Verify(x => x.DeleteAsync(1, cancellationToken), Times.Once);
    }
}
