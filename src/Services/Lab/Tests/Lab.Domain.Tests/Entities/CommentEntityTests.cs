namespace Lab.Domain.Tests.Entities;

public sealed class CommentEntityTests
{
    [Fact]
    public void Create_ShouldInitializeCorrectly()
    {
        var id = Guid.NewGuid();
        var sectionId = Guid.NewGuid();

        var entity = CommentEntity.Create(id, sectionId, "Great work!", "user1");

        entity.Id.Should().Be(id);
        entity.SectionId.Should().Be(sectionId);
        entity.Content.Should().Be("Great work!");
        entity.UserName.Should().Be("user1");
        entity.CreatedOnUtc.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Update_ShouldUpdateContent()
    {
        var entity = CommentEntity.Create(Guid.NewGuid(), Guid.NewGuid(), "old", "user1");

        entity.Update("new content");

        entity.Content.Should().Be("new content");
        entity.LastModifiedOnUtc.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
    }
}
