using HomeLedger.Application.UseCases;
using HomeLedger.Domain.BusinessModels;
using HomeLedger.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace HomeLedger.UnitTests;

public sealed class PaymentTypeUseCaseTests
{
    [Fact]
    public async Task GetAllAsync_GivenPaymentTypesExist_ShouldReturnAllPaymentTypes()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<IPaymentTypeRepository>();
        var model = new PaymentType { Id = 1, Description = "Card" };
        IReadOnlyList<PaymentType> list = [model];
        repository.Setup(x => x.GetAllAsync(cancellationToken)).ReturnsAsync(list);
        var useCase = new PaymentTypeUseCase(repository.Object);

        (await useCase.GetAllAsync(cancellationToken)).Should().BeSameAs(list);
        repository.Verify(x => x.GetAllAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetAsync_GivenAnExistingId_ShouldReturnPaymentTypeWithGivenId()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<IPaymentTypeRepository>();
        var model = new PaymentType { Id = 1, Description = "Card" };
        repository.Setup(x => x.GetAsync(1, cancellationToken)).ReturnsAsync(model);
        var useCase = new PaymentTypeUseCase(repository.Object);

        (await useCase.GetAsync(1, cancellationToken)).Should().BeSameAs(model);
        repository.Verify(x => x.GetAsync(1, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_GivenAValidPaymentType_ShouldReturnInsertedPaymentType()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<IPaymentTypeRepository>();
        var model = new PaymentType { Id = 1, Description = "Card" };
        repository.Setup(x => x.InsertAsync(model, cancellationToken)).ReturnsAsync(model);
        var useCase = new PaymentTypeUseCase(repository.Object);

        (await useCase.InsertAsync(model, cancellationToken)).Should().BeSameAs(model);
        repository.Verify(x => x.InsertAsync(model, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_GivenAnExistingId_ShouldReturnTrue()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<IPaymentTypeRepository>();
        var model = new PaymentType { Id = 1, Description = "Card" };
        repository.Setup(x => x.UpdateAsync(1, model, cancellationToken)).ReturnsAsync(true);
        var useCase = new PaymentTypeUseCase(repository.Object);

        (await useCase.UpdateAsync(1, model, cancellationToken)).Should().BeTrue();
        repository.Verify(x => x.UpdateAsync(1, model, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_GivenAnExistingId_ShouldReturnTrue()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<IPaymentTypeRepository>();
        repository.Setup(x => x.DeleteAsync(1, cancellationToken)).ReturnsAsync(true);
        var useCase = new PaymentTypeUseCase(repository.Object);

        (await useCase.DeleteAsync(1, cancellationToken)).Should().BeTrue();
        repository.Verify(x => x.DeleteAsync(1, cancellationToken), Times.Once);
    }
}
