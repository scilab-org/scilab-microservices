namespace Lab.Domain.Tests.Abstractions;

public sealed class IDomainEventTests
{
    private sealed class TestDomainEvent : IDomainEvent { }

    [Fact]
    public void EventId_ShouldReturnNewGuid()
    {
        IDomainEvent domainEvent = new TestDomainEvent();
        domainEvent.EventId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void EventId_ShouldReturnDifferentValueOnEachAccess()
    {
        IDomainEvent domainEvent = new TestDomainEvent();
        var id1 = domainEvent.EventId;
        var id2 = domainEvent.EventId;
        id1.Should().NotBe(id2);
    }

    [Fact]
    public void OccurredOn_ShouldReturnCurrentDateTime()
    {
        IDomainEvent domainEvent = new TestDomainEvent();
        domainEvent.OccurredOn.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void EventType_ShouldReturnAssemblyQualifiedName()
    {
        IDomainEvent domainEvent = new TestDomainEvent();
        var eventType = domainEvent.EventType;
        eventType.Should().NotBeNullOrEmpty();
        eventType.Should().Contain("TestDomainEvent");
    }
}
