using HomeLedger.Application.UseCases;
using HomeLedger.Domain.BusinessModels;
using HomeLedger.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace HomeLedger.UnitTests;

public sealed class ExpenseUseCaseTests
{
    [Fact]
    public async Task GetAllAsync_GivenADateRange_ShouldReturnExpensesWithinRange()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<IExpenseRepository>();
        var startDate = new DateOnly(2026, 1, 1);
        var endDate = new DateOnly(2026, 1, 31);
        var model = CreateModel();
        IReadOnlyList<Expense> list = [model];
        repository.Setup(x => x.GetAllAsync(startDate, endDate, cancellationToken)).ReturnsAsync(list);
        var useCase = new ExpenseUseCase(repository.Object);

        var result = await useCase.GetAllAsync(startDate, endDate, cancellationToken);

        result.Should().BeSameAs(list);
        result.Should().ContainSingle();
        result[0].Category.Should().BeEquivalentTo(model.Category);
        result[0].Location.Should().BeEquivalentTo(model.Location);
        result[0].PaymentType.Should().BeEquivalentTo(model.PaymentType);
        repository.Verify(x => x.GetAllAsync(startDate, endDate, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetAsync_GivenAnExistingId_ShouldReturnExpenseWithRelatedModels()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<IExpenseRepository>();
        var model = CreateModel();
        repository.Setup(x => x.GetAsync(1, cancellationToken)).ReturnsAsync(model);
        var useCase = new ExpenseUseCase(repository.Object);

        var result = await useCase.GetAsync(1, cancellationToken);

        result.Should().BeSameAs(model);
        result.Category.Should().BeEquivalentTo(model.Category);
        result.Location.Should().BeEquivalentTo(model.Location);
        result.PaymentType.Should().BeEquivalentTo(model.PaymentType);
        repository.Verify(x => x.GetAsync(1, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_GivenAValidExpense_ShouldReturnInsertedExpenseWithRelatedModels()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<IExpenseRepository>();
        var model = CreateModel();
        repository.Setup(x => x.InsertAsync(model, cancellationToken)).ReturnsAsync(model);
        var useCase = new ExpenseUseCase(repository.Object);

        var result = await useCase.InsertAsync(model, cancellationToken);

        result.Should().BeSameAs(model);
        result.Category.Should().BeEquivalentTo(model.Category);
        result.Location.Should().BeEquivalentTo(model.Location);
        result.PaymentType.Should().BeEquivalentTo(model.PaymentType);
        repository.Verify(x => x.InsertAsync(model, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_GivenAnExistingId_ShouldReturnTrue()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<IExpenseRepository>();
        var model = CreateModel();
        repository.Setup(x => x.UpdateAsync(1, model, cancellationToken)).ReturnsAsync(true);
        var useCase = new ExpenseUseCase(repository.Object);

        (await useCase.UpdateAsync(1, model, cancellationToken)).Should().BeTrue();
        repository.Verify(x => x.UpdateAsync(1, model, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_GivenAnExistingId_ShouldReturnTrue()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<IExpenseRepository>();
        repository.Setup(x => x.DeleteAsync(1, cancellationToken)).ReturnsAsync(true);
        var useCase = new ExpenseUseCase(repository.Object);

        (await useCase.DeleteAsync(1, cancellationToken)).Should().BeTrue();
        repository.Verify(x => x.DeleteAsync(1, cancellationToken), Times.Once);
    }

    private static Expense CreateModel()
    {
        return new Expense
        {
            Id = 1,
            Date = new(2026, 1, 1),
            LocationId = 2,
            CategoryId = 3,
            PaymentTypeId = 5,
            PurchaseAmount = 10m,
            DiscountAmount = 1m,
            Total = 9m,
            Category = new Category
            {
                Id = 3,
                Name = "Groceries",
                GroupingId = 4,
                Grouping = new Grouping { Id = 4, Name = "Home" }
            },
            Location = new Location
            {
                Id = 2,
                Name = "Local Market",
                Description = "Nearby store"
            },
            PaymentType = new PaymentType
            {
                Id = 5,
                Description = "Debit card"
            }
        };
    }
}