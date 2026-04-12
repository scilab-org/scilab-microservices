namespace User.Domain.Tests.Abstractions;

public sealed class AggregateTests
{
    private sealed class TestDomainEvent : IDomainEvent { }

    private sealed class TestAggregate : Aggregate<Guid>
    {
        public TestAggregate(Guid id)
        {
            Id = id;
        }
    }

    [Fact]
    public void DomainEvents_ShouldBeEmpty_WhenAggregateIsCreated()
    {
        // Arrange & Act
        var aggregate = new TestAggregate(Guid.NewGuid());

        // Assert
        aggregate.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void AddDomainEvent_ShouldAppendEventToList()
    {
        // Arrange
        var aggregate = new TestAggregate(Guid.NewGuid());
        var evt = new TestDomainEvent();

        // Act
        aggregate.AddDomainEvent(evt);

        // Assert
        aggregate.DomainEvents.Should().ContainSingle()
            .Which.Should().Be(evt);
    }

    [Fact]
    public void AddDomainEvent_ShouldSupportMultipleEvents()
    {
        // Arrange
        var aggregate = new TestAggregate(Guid.NewGuid());
        var evt1 = new TestDomainEvent();
        var evt2 = new TestDomainEvent();

        // Act
        aggregate.AddDomainEvent(evt1);
        aggregate.AddDomainEvent(evt2);

        // Assert
        aggregate.DomainEvents.Should().HaveCount(2);
    }

    [Fact]
    public void ClearDomainEvents_ShouldReturnAllEventsAndClearList()
    {
        // Arrange
        var aggregate = new TestAggregate(Guid.NewGuid());
        var evt1 = new TestDomainEvent();
        var evt2 = new TestDomainEvent();
        aggregate.AddDomainEvent(evt1);
        aggregate.AddDomainEvent(evt2);

        // Act
        var dequeued = aggregate.ClearDomainEvents();

        // Assert
        dequeued.Should().HaveCount(2);
        dequeued.Should().Contain(evt1);
        dequeued.Should().Contain(evt2);
        aggregate.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void ClearDomainEvents_ShouldReturnEmpty_WhenNoEventsExist()
    {
        // Arrange
        var aggregate = new TestAggregate(Guid.NewGuid());

        // Act
        var dequeued = aggregate.ClearDomainEvents();

        // Assert
        dequeued.Should().BeEmpty();
        aggregate.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void DomainEvents_ShouldBeReadOnly_AfterReturn()
    {
        // Arrange
        var aggregate = new TestAggregate(Guid.NewGuid());
        aggregate.AddDomainEvent(new TestDomainEvent());
        var events = aggregate.DomainEvents;

        // Act & Assert — list returned is read-only
        var act = () => ((System.Collections.Generic.IList<IDomainEvent>)events).Add(new TestDomainEvent());
        act.Should().Throw<NotSupportedException>();
    }
}
