namespace Lab.Domain.Tests.Entities;

public sealed class TagEntityTests
{
    [Fact]
    public void Create_ShouldInitializeCorrectly()
    {
        var id = Guid.NewGuid();
        var entity = TagEntity.Create(id, "Machine Learning");

        entity.Id.Should().Be(id);
        entity.Name.Should().Be("Machine Learning");
        entity.CreatedOnUtc.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Update_ShouldUpdateName()
    {
        var entity = TagEntity.Create(Guid.NewGuid(), "Old Tag");
        entity.Update("New Tag");
        entity.Name.Should().Be("New Tag");
    }

    [Fact]
    public void Update_ShouldKeepExisting_WhenNameIsNull()
    {
        var entity = TagEntity.Create(Guid.NewGuid(), "Tag");
        entity.Update(null);
        entity.Name.Should().Be("Tag");
    }
}
