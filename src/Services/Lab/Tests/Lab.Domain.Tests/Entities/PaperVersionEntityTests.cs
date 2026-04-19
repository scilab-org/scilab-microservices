namespace Lab.Domain.Tests.Entities;

public sealed class PaperVersionEntityTests
{
    [Fact]
    public void Create_ShouldInitializeCorrectly()
    {
        var id = Guid.NewGuid();
        var paperId = Guid.NewGuid();
        var refs = new List<Guid> { Guid.NewGuid() };
        var files = new List<string> { "f1.pdf" };

        var entity = PaperVersionEntity.Create(id, paperId, "v1", "content", refs, files, "admin");

        entity.Id.Should().Be(id);
        entity.PaperId.Should().Be(paperId);
        entity.Name.Should().Be("v1");
        entity.Content.Should().Be("content");
        entity.References.Should().HaveCount(1);
        entity.Files.Should().ContainSingle("f1.pdf");
        entity.CreatedBy.Should().Be("admin");
        entity.LastModifiedBy.Should().Be("admin");
    }

    [Fact]
    public void Create_ShouldUseDefaults()
    {
        var entity = PaperVersionEntity.Create(Guid.NewGuid(), Guid.NewGuid(), "v", "c");
        entity.References.Should().BeNull();
        entity.Files.Should().BeNull();
        entity.CreatedBy.Should().BeNull();
    }

    [Fact]
    public void Update_ShouldUpdateProvidedValues()
    {
        var entity = PaperVersionEntity.Create(Guid.NewGuid(), Guid.NewGuid(), "v", "old");
        var newPaperId = Guid.NewGuid();
        var newRefs = new List<Guid> { Guid.NewGuid() };
        var newFiles = new List<string> { "new.pdf" };

        entity.Update(content: "new", references: newRefs, files: newFiles,
            lastModifiedBy: "editor", paperId: newPaperId);

        entity.Content.Should().Be("new");
        entity.References.Should().HaveCount(1);
        entity.Files.Should().ContainSingle("new.pdf");
        entity.LastModifiedBy.Should().Be("editor");
        entity.PaperId.Should().Be(newPaperId);
    }

    [Fact]
    public void Update_ShouldKeepExisting_WhenNullsPassed()
    {
        var entity = PaperVersionEntity.Create(Guid.NewGuid(), Guid.NewGuid(), "v", "content");
        entity.Update();
        entity.Content.Should().Be("content");
    }
}
