namespace Lab.Domain.Tests.Abstractions;

public sealed class AggregateTests
{
    private sealed class TestAggregate : Aggregate<Guid> { }
    private sealed class TestDomainEvent : IDomainEvent { }

    [Fact]
    public void DomainEvents_ShouldBeEmpty_WhenNewlyCreated()
    {
        var aggregate = new TestAggregate();
        aggregate.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void AddDomainEvent_ShouldAddEventToList()
    {
        var aggregate = new TestAggregate();
        var domainEvent = new TestDomainEvent();

        aggregate.AddDomainEvent(domainEvent);

        aggregate.DomainEvents.Should().ContainSingle().Which.Should().Be(domainEvent);
    }

    [Fact]
    public void AddDomainEvent_ShouldAddMultipleEvents()
    {
        var aggregate = new TestAggregate();
        var event1 = new TestDomainEvent();
        var event2 = new TestDomainEvent();

        aggregate.AddDomainEvent(event1);
        aggregate.AddDomainEvent(event2);

        aggregate.DomainEvents.Should().HaveCount(2);
    }

    [Fact]
    public void ClearDomainEvents_ShouldReturnAllEventsAndClearList()
    {
        var aggregate = new TestAggregate();
        var event1 = new TestDomainEvent();
        var event2 = new TestDomainEvent();
        aggregate.AddDomainEvent(event1);
        aggregate.AddDomainEvent(event2);

        var cleared = aggregate.ClearDomainEvents();

        cleared.Should().HaveCount(2);
        cleared.Should().Contain(event1);
        cleared.Should().Contain(event2);
        aggregate.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void ClearDomainEvents_ShouldReturnEmptyArray_WhenNoEvents()
    {
        var aggregate = new TestAggregate();
        var cleared = aggregate.ClearDomainEvents();
        cleared.Should().BeEmpty();
    }

    [Fact]
    public void DomainEvents_ShouldBeReadOnly()
    {
        var aggregate = new TestAggregate();
        aggregate.DomainEvents.Should().BeAssignableTo<IReadOnlyList<IDomainEvent>>();
    }
}
