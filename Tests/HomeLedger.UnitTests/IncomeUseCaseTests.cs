using HomeLedger.Application.UseCases;
using HomeLedger.Domain.BusinessModels;
using HomeLedger.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace HomeLedger.UnitTests;

public sealed class IncomeUseCaseTests
{
    [Fact]
    public async Task GetAllAsync_GivenADateRange_ShouldReturnIncomesWithinRange()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<IIncomeRepository>();
        var startDate = new DateOnly(2026, 1, 1);
        var endDate = new DateOnly(2026, 1, 31);
        var model = new Income { Id = 1, Date = new(2026, 1, 1), Description = "Salary", Amount = 100m };
        IReadOnlyList<Income> list = [model];
        repository.Setup(x => x.GetAllAsync(startDate, endDate, cancellationToken)).ReturnsAsync(list);
        var useCase = new IncomeUseCase(repository.Object);

        (await useCase.GetAllAsync(startDate, endDate, cancellationToken)).Should().BeSameAs(list);
        repository.Verify(x => x.GetAllAsync(startDate, endDate, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetAsync_GivenAnExistingId_ShouldReturnIncomeWithGivenId()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<IIncomeRepository>();
        var model = new Income { Id = 1, Date = new(2026, 1, 1), Description = "Salary", Amount = 100m };
        repository.Setup(x => x.GetAsync(1, cancellationToken)).ReturnsAsync(model);
        var useCase = new IncomeUseCase(repository.Object);

        (await useCase.GetAsync(1, cancellationToken)).Should().BeSameAs(model);
        repository.Verify(x => x.GetAsync(1, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_GivenAValidIncome_ShouldReturnInsertedIncome()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<IIncomeRepository>();
        var model = new Income { Id = 1, Date = new(2026, 1, 1), Description = "Salary", Amount = 100m };
        repository.Setup(x => x.InsertAsync(model, cancellationToken)).ReturnsAsync(model);
        var useCase = new IncomeUseCase(repository.Object);

        (await useCase.InsertAsync(model, cancellationToken)).Should().BeSameAs(model);
        repository.Verify(x => x.InsertAsync(model, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_GivenAnExistingId_ShouldReturnTrue()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<IIncomeRepository>();
        var model = new Income { Id = 1, Date = new(2026, 1, 1), Description = "Salary", Amount = 100m };
        repository.Setup(x => x.UpdateAsync(1, model, cancellationToken)).ReturnsAsync(true);
        var useCase = new IncomeUseCase(repository.Object);

        (await useCase.UpdateAsync(1, model, cancellationToken)).Should().BeTrue();
        repository.Verify(x => x.UpdateAsync(1, model, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_GivenAnExistingId_ShouldReturnTrue()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<IIncomeRepository>();
        repository.Setup(x => x.DeleteAsync(1, cancellationToken)).ReturnsAsync(true);
        var useCase = new IncomeUseCase(repository.Object);

        (await useCase.DeleteAsync(1, cancellationToken)).Should().BeTrue();
        repository.Verify(x => x.DeleteAsync(1, cancellationToken), Times.Once);
    }
}
