namespace Lab.Domain.Tests.Entities;

public sealed class PaperVersionFileEntityTests
{
    [Fact]
    public void Create_ShouldInitializeCorrectly()
    {
        var pvId = Guid.NewGuid();
        var entity = PaperVersionFileEntity.Create(pvId, "file.pdf", "https://url.com/file.pdf", "note1", "admin");

        entity.Id.Should().NotBe(Guid.Empty);
        entity.PaperVersionId.Should().Be(pvId);
        entity.FileName.Should().Be("file.pdf");
        entity.FileUrl.Should().Be("https://url.com/file.pdf");
        entity.Note.Should().Be("note1");
        entity.CreatedBy.Should().Be("admin");
        entity.LastModifiedBy.Should().Be("admin");
        entity.CreatedOnUtc.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Create_ShouldGenerateNewId()
    {
        var entity1 = PaperVersionFileEntity.Create(Guid.NewGuid(), "f1", "u1");
        var entity2 = PaperVersionFileEntity.Create(Guid.NewGuid(), "f2", "u2");
        entity1.Id.Should().NotBe(entity2.Id);
    }

    [Fact]
    public void Create_ShouldUseDefaults_WhenOptionalOmitted()
    {
        var entity = PaperVersionFileEntity.Create(Guid.NewGuid(), "f", "u");
        entity.Note.Should().BeNull();
        entity.CreatedBy.Should().BeNull();
    }
}
