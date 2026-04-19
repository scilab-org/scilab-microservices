namespace Management.Domain.Tests.Abstractions;

public sealed class IDomainEventTests
{
    private sealed class TestDomainEvent : IDomainEvent
    {
    }

    [Fact]
    public void EventId_ShouldReturnNewGuid()
    {
        // Arrange
        IDomainEvent domainEvent = new TestDomainEvent();

        // Act
        var eventId = domainEvent.EventId;

        // Assert
        eventId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void EventId_ShouldReturnDifferentValueOnEachAccess()
    {
        // Arrange
        IDomainEvent domainEvent = new TestDomainEvent();

        // Act
        var id1 = domainEvent.EventId;
        var id2 = domainEvent.EventId;

        // Assert
        id1.Should().NotBe(id2);
    }

    [Fact]
    public void OccurredOn_ShouldReturnCurrentDateTime()
    {
        // Arrange
        IDomainEvent domainEvent = new TestDomainEvent();

        // Act
        var occurredOn = domainEvent.OccurredOn;

        // Assert
        occurredOn.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void EventType_ShouldReturnAssemblyQualifiedName()
    {
        // Arrange
        IDomainEvent domainEvent = new TestDomainEvent();

        // Act
        var eventType = domainEvent.EventType;

        // Assert
        eventType.Should().NotBeNullOrEmpty();
        eventType.Should().Contain("TestDomainEvent");
    }
}
