namespace Lab.Domain.Tests.Entities;

public sealed class SectionEntityTests
{
    [Fact]
    public void Create_ShouldInitializeWithCorrectValues()
    {
        var id = Guid.NewGuid();
        var paperId = Guid.NewGuid();
        var prevId = Guid.NewGuid();
        var refs = new List<Guid> { Guid.NewGuid() };

        var entity = SectionEntity.Create(id, "content", paperId, 1.0f,
            SectionStatus.InProgress, isMainSection: true, isOldMainSection: false,
            version: "1.0", title: "Introduction", sectionSumary: "summary",
            description: "desc", mainIdea: "idea", rule: "rule",
            previousVersionSectionId: prevId, createdBy: "admin",
            references: refs, paperRule: "prule", projectRule: "projrule",
            sectionRule: "srule", files: new List<string> { "f1" },
            packages: new List<string> { "pkg1" },
            sectionContext: "sctx", projectContext: "pctx", paperContext: "ppctx");

        entity.Id.Should().Be(id);
        entity.Content.Should().Be("content");
        entity.PaperId.Should().Be(paperId);
        entity.DisplayOrder.Should().Be(1.0f);
        entity.Status.Should().Be(SectionStatus.InProgress);
        entity.IsMainSection.Should().BeTrue();
        entity.IsOldMainSection.Should().BeFalse();
        entity.Version.Should().Be("1.0");
        entity.Title.Should().Be("Introduction");
        entity.SectionSumary.Should().Be("summary");
        entity.Description.Should().Be("desc");
        entity.MainIdea.Should().Be("idea");
        entity.Rule.Should().Be("rule");
        entity.PreviousVersionSectionId.Should().Be(prevId);
        entity.CreatedBy.Should().Be("admin");
        entity.References.Should().HaveCount(1);
        entity.PaperRule.Should().Be("prule");
        entity.ProjectRule.Should().Be("projrule");
        entity.SectionRule.Should().Be("srule");
        entity.SectionContext.Should().Be("sctx");
        entity.ProjectContext.Should().Be("pctx");
        entity.PaperContext.Should().Be("ppctx");
        entity.Files.Should().ContainSingle("f1");
        entity.Packages.Should().ContainSingle("pkg1");
    }

    [Fact]
    public void Update_ShouldUpdateProvidedValues()
    {
        var entity = SectionEntity.Create(Guid.NewGuid(), "old", Guid.NewGuid(), 1.0f, SectionStatus.NotStarted);
        var newPaperId = Guid.NewGuid();

        entity.Update(content: "new", title: "NewTitle", displayOrder: 2.0f,
            sectionSumary: "newSum", description: "newDesc",
            status: SectionStatus.Completed, mainIdea: "newIdea",
            rule: "newRule", isMainSection: true, isOldMainSection: true,
            version: "2.0", paperId: newPaperId,
            references: new List<Guid>(), paperRule: "pr",
            projectRule: "projr", sectionRule: "sr",
            files: new List<string> { "f2" }, packages: new List<string> { "p2" },
            lastModifiedBy: "editor",
            sectionContext: "sc", projectContext: "pc", paperContext: "ppctx");

        entity.Content.Should().Be("new");
        entity.Title.Should().Be("NewTitle");
        entity.DisplayOrder.Should().Be(2.0f);
        entity.Status.Should().Be(SectionStatus.Completed);
        entity.IsMainSection.Should().BeTrue();
        entity.LastModifiedBy.Should().Be("editor");
        entity.PaperId.Should().Be(newPaperId);
    }

    [Fact]
    public void Update_ShouldKeepExistingValues_WhenNullsPassed()
    {
        var entity = SectionEntity.Create(Guid.NewGuid(), "content", Guid.NewGuid(), 1.0f, SectionStatus.InProgress);
        entity.Update();
        entity.Content.Should().Be("content");
        entity.Status.Should().Be(SectionStatus.InProgress);
    }

    [Fact]
    public void Update_ShouldKeepExistingVersionLinks_WhenNullVersionIdsPassed()
    {
        var previousVersionSectionId = Guid.NewGuid();
        var nextVersionSectionId = Guid.NewGuid();
        var entity = SectionEntity.Create(
            Guid.NewGuid(),
            "content",
            Guid.NewGuid(),
            1.0f,
            SectionStatus.InProgress,
            previousVersionSectionId: previousVersionSectionId);

        entity.NextVersionSectionId = nextVersionSectionId;

        entity.Update(previousVersionSectionId: null, nextVersionSectionId: null);

        entity.PreviousVersionSectionId.Should().Be(previousVersionSectionId);
        entity.NextVersionSectionId.Should().Be(nextVersionSectionId);
    }

    [Fact]
    public void UpdateFilePath_ShouldAddToFiles_WhenUrlValid()
    {
        var entity = SectionEntity.Create(Guid.NewGuid(), "c", Guid.NewGuid(), 1.0f, SectionStatus.NotStarted);
        entity.UpdateFilePath("https://example.com/file.txt");
        entity.Files.Should().ContainSingle("https://example.com/file.txt");
    }

    [Fact]
    public void UpdateFilePath_ShouldInitializeFilesList_WhenNull()
    {
        var entity = SectionEntity.Create(Guid.NewGuid(), "c", Guid.NewGuid(), 1.0f, SectionStatus.NotStarted);
        entity.Files = null;
        entity.UpdateFilePath("https://example.com/file.txt");
        entity.Files.Should().ContainSingle("https://example.com/file.txt");
    }

    [Fact]
    public void UpdateFilePath_ShouldNotUpdate_WhenUrlIsNull()
    {
        var entity = SectionEntity.Create(Guid.NewGuid(), "c", Guid.NewGuid(), 1.0f, SectionStatus.NotStarted);
        entity.UpdateFilePath(null);
        entity.Files.Should().BeNull();
    }

    [Fact]
    public void Update_ShouldUpdateVersionLinks_WhenNonNullVersionIdsPassed()
    {
        var entity = SectionEntity.Create(Guid.NewGuid(), "content", Guid.NewGuid(), 1.0f, SectionStatus.InProgress);
        var newPrevId = Guid.NewGuid();
        var newNextId = Guid.NewGuid();

        entity.Update(previousVersionSectionId: newPrevId, nextVersionSectionId: newNextId);

        entity.PreviousVersionSectionId.Should().Be(newPrevId);
        entity.NextVersionSectionId.Should().Be(newNextId);
    }

    [Fact]
    public void UpdateFilePath_ShouldNotUpdate_WhenUrlIsEmpty()
    {
        var entity = SectionEntity.Create(Guid.NewGuid(), "c", Guid.NewGuid(), 1.0f, SectionStatus.NotStarted);
        entity.UpdateFilePath("");
        entity.Files.Should().BeNull();
    }

    [Fact]
    public void UpdateFilePath_ShouldNotUpdate_WhenUrlIsWhitespace()
    {
        var entity = SectionEntity.Create(Guid.NewGuid(), "c", Guid.NewGuid(), 1.0f, SectionStatus.NotStarted);
        entity.UpdateFilePath("   ");
        entity.Files.Should().BeNull();
    }
}
