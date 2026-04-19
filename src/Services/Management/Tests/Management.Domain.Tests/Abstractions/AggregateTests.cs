namespace Management.Domain.Tests.Abstractions;

public sealed class AggregateTests
{
    private sealed class TestAggregate : Aggregate<Guid>
    {
    }

    private sealed class TestDomainEvent : IDomainEvent
    {
    }

    [Fact]
    public void DomainEvents_ShouldBeEmpty_WhenNewlyCreated()
    {
        // Arrange & Act
        var aggregate = new TestAggregate();

        // Assert
        aggregate.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void AddDomainEvent_ShouldAddEventToList()
    {
        // Arrange
        var aggregate = new TestAggregate();
        var domainEvent = new TestDomainEvent();

        // Act
        aggregate.AddDomainEvent(domainEvent);

        // Assert
        aggregate.DomainEvents.Should().ContainSingle();
        aggregate.DomainEvents.Should().Contain(domainEvent);
    }

    [Fact]
    public void AddDomainEvent_ShouldAddMultipleEvents()
    {
        // Arrange
        var aggregate = new TestAggregate();
        var event1 = new TestDomainEvent();
        var event2 = new TestDomainEvent();

        // Act
        aggregate.AddDomainEvent(event1);
        aggregate.AddDomainEvent(event2);

        // Assert
        aggregate.DomainEvents.Should().HaveCount(2);
    }

    [Fact]
    public void ClearDomainEvents_ShouldReturnAllEventsAndClearList()
    {
        // Arrange
        var aggregate = new TestAggregate();
        var event1 = new TestDomainEvent();
        var event2 = new TestDomainEvent();
        aggregate.AddDomainEvent(event1);
        aggregate.AddDomainEvent(event2);

        // Act
        var cleared = aggregate.ClearDomainEvents();

        // Assert
        cleared.Should().HaveCount(2);
        cleared.Should().Contain(event1);
        cleared.Should().Contain(event2);
        aggregate.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void ClearDomainEvents_ShouldReturnEmptyArray_WhenNoEvents()
    {
        // Arrange
        var aggregate = new TestAggregate();

        // Act
        var cleared = aggregate.ClearDomainEvents();

        // Assert
        cleared.Should().BeEmpty();
    }

    [Fact]
    public void DomainEvents_ShouldBeReadOnly()
    {
        // Arrange
        var aggregate = new TestAggregate();

        // Act & Assert
        aggregate.DomainEvents.Should().BeAssignableTo<IReadOnlyList<IDomainEvent>>();
    }
}
