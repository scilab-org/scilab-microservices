namespace Lab.Domain.Tests.Abstractions;

public sealed class EntityTests
{
    private sealed class TestEntity : Entity<Guid> { }

    [Fact]
    public void Entity_ShouldHaveDefaultValues()
    {
        var entity = new TestEntity();
        entity.Id.Should().Be(Guid.Empty);
        entity.CreatedBy.Should().BeNull();
        entity.LastModifiedBy.Should().BeNull();
    }

    [Fact]
    public void Entity_ShouldSetAndGetProperties()
    {
        var id = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        var entity = new TestEntity
        {
            Id = id,
            CreatedOnUtc = now,
            CreatedBy = "user1",
            LastModifiedOnUtc = now,
            LastModifiedBy = "user2"
        };

        entity.Id.Should().Be(id);
        entity.CreatedOnUtc.Should().Be(now);
        entity.CreatedBy.Should().Be("user1");
        entity.LastModifiedOnUtc.Should().Be(now);
        entity.LastModifiedBy.Should().Be("user2");
    }
}

public sealed class EntityIdTests
{
    private sealed class TestEntityId : EntityId<Guid> { }

    [Fact]
    public void EntityId_ShouldSetAndGetId()
    {
        var id = Guid.NewGuid();
        var entity = new TestEntityId { Id = id };
        entity.Id.Should().Be(id);
    }
}
