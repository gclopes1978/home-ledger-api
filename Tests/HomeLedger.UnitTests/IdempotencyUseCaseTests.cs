using FluentAssertions;
using HomeLedger.Application.UseCases;
using HomeLedger.Domain.BusinessModels;
using HomeLedger.Domain.Interfaces.Repositories;
using Moq;
using Xunit;

namespace HomeLedger.UnitTests;

public sealed class IdempotencyUseCaseTests
{
    [Fact]
    public async Task TryBeginAsync_GivenANewKey_ShouldReturnRepositoryResult()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<IIdempotencyRepository>();
        var record = CreateRecord();
        repository.Setup(item => item.TryBeginAsync(record, cancellationToken))
            .ReturnsAsync(true);
        var useCase = new IdempotencyUseCase(repository.Object);

        (await useCase.TryBeginAsync(record, cancellationToken)).Should().BeTrue();

        repository.Verify(item => item.TryBeginAsync(record, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetByKeyAsync_GivenAnExistingKey_ShouldReturnStoredRecord()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<IIdempotencyRepository>();
        var record = CreateRecord();
        repository.Setup(item => item.GetByKeyAsync(record.IdempotencyKey, cancellationToken))
            .ReturnsAsync(record);
        var useCase = new IdempotencyUseCase(repository.Object);

        (await useCase.GetByKeyAsync(record.IdempotencyKey, cancellationToken))
            .Should().BeSameAs(record);

        repository.Verify(
            item => item.GetByKeyAsync(record.IdempotencyKey, cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task CompleteAsync_GivenAStoredKey_ShouldForwardTheResponse()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<IIdempotencyRepository>();
        repository.Setup(item => item.CompleteAsync(
                "key-1",
                201,
                "application/json",
                "/api/categories/2",
                "{\"id\":2}",
                cancellationToken))
            .Returns(Task.CompletedTask);
        var useCase = new IdempotencyUseCase(repository.Object);

        await useCase.CompleteAsync(
            "key-1",
            201,
            "application/json",
            "/api/categories/2",
            "{\"id\":2}",
            cancellationToken);

        repository.Verify(
            item => item.CompleteAsync(
                "key-1",
                201,
                "application/json",
                "/api/categories/2",
                "{\"id\":2}",
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_GivenAnIncompleteKey_ShouldForwardTheDelete()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<IIdempotencyRepository>();
        repository.Setup(item => item.DeleteAsync("key-1", cancellationToken))
            .Returns(Task.CompletedTask);
        var useCase = new IdempotencyUseCase(repository.Object);

        await useCase.DeleteAsync("key-1", cancellationToken);

        repository.Verify(item => item.DeleteAsync("key-1", cancellationToken), Times.Once);
    }

    private static IdempotencyRecord CreateRecord()
    {
        return new IdempotencyRecord
        {
            IdempotencyKey = "key-1",
            RequestHash = "request-hash",
            CreatedAtUtc = DateTimeOffset.UtcNow
        };
    }
}