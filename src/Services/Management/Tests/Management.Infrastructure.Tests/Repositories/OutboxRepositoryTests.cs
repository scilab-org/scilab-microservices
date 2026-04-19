using Management.Infrastructure.Repositories;

namespace Management.Infrastructure.Tests.Repositories;

public sealed class OutboxRepositoryTests
{
    private readonly Mock<IDocumentSession> _sessionMock = new();
    private readonly Mock<ILogger<OutboxRepository>> _loggerMock = new();
    private readonly OutboxRepository _repository;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public OutboxRepositoryTests()
    {
        _repository = new OutboxRepository(_sessionMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task AddMessageAsync_ShouldStoreAndSave_WhenMessageIsValid()
    {
        // Arrange
        var message = OutboxMessageEntity.Create(Guid.NewGuid(), "TestEvent", "payload", DateTimeOffset.UtcNow);

        // Act
        var result = await _repository.AddMessageAsync(message, _cancellationToken);

        // Assert
        result.Should().BeTrue();
        _sessionMock.Verify(s => s.Store(message), Times.Once);
        _sessionMock.Verify(s => s.SaveChangesAsync(_cancellationToken), Times.Once);
    }

    [Fact]
    public async Task UpdateMessagesAsync_ShouldStoreAllAndSave_WhenMessagesProvided()
    {
        // Arrange
        var messages = new List<OutboxMessageEntity>
        {
            OutboxMessageEntity.Create(Guid.NewGuid(), "Event1", "payload1", DateTimeOffset.UtcNow),
            OutboxMessageEntity.Create(Guid.NewGuid(), "Event2", "payload2", DateTimeOffset.UtcNow)
        };

        // Act
        var result = await _repository.UpdateMessagesAsync(messages, _cancellationToken);

        // Assert
        result.Should().BeTrue();
        _sessionMock.Verify(s => s.Store(It.IsAny<OutboxMessageEntity>()), Times.Exactly(2));
        _sessionMock.Verify(s => s.SaveChangesAsync(_cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ReleaseClaimsAsync_ShouldClearClaimedAndSave_WhenMessagesProvided()
    {
        // Arrange
        var message = OutboxMessageEntity.Create(Guid.NewGuid(), "TestEvent", "payload", DateTimeOffset.UtcNow);
        message.Claim(DateTimeOffset.UtcNow);
        var messages = new List<OutboxMessageEntity> { message };

        // Act
        var result = await _repository.ReleaseClaimsAsync(messages, _cancellationToken);

        // Assert
        result.Should().BeTrue();
        message.ClaimedOnUtc.Should().BeNull();
        _sessionMock.Verify(s => s.Store(message), Times.Once);
        _sessionMock.Verify(s => s.SaveChangesAsync(_cancellationToken), Times.Once);
    }
}
