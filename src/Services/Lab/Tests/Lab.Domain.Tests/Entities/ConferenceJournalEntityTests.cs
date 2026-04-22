namespace Lab.Domain.Tests.Entities;

public sealed class ConferenceJournalEntityTests
{
    [Fact]
    public void Create_ShouldInitializeCorrectly()
    {
        var id = Guid.NewGuid();
        var templateId = Guid.NewGuid();
        var projectIds = new List<Guid> { Guid.NewGuid() };
        var paperIds = new List<Guid> { Guid.NewGuid() };

        var entity = ConferenceJournalEntity.Create(id, "ICSE 2024", "A*",
            "https://icse.org", null, "IEEE", ConferenceJournalType.Journal, new List<Guid> { templateId },
            "tex.tex", "pdf.pdf",
            projectIds, paperIds, "admin");

        entity.Id.Should().Be(id);
        entity.Name.Should().Be("ICSE 2024");
        entity.Ranking.Should().Be("A*");
        entity.Url.Should().Be("https://icse.org");
        entity.Style.Should().Be("IEEE");
        entity.TemplateIds.Should().ContainSingle().Which.Should().Be(templateId);
        entity.TexFile.Should().Be("tex.tex");
        entity.PdfFile.Should().Be("pdf.pdf");
        entity.ProjectIds.Should().HaveCount(1);
        entity.PaperIds.Should().HaveCount(1);
        entity.CreatedBy.Should().Be("admin");
    }

    [Fact]
    public void Create_ShouldDefaultListsToEmpty()
    {
        var entity = ConferenceJournalEntity.Create(Guid.NewGuid(), "Name", null, null, null, null, ConferenceJournalType.Journal, [], null, null);
        entity.ProjectIds.Should().BeEmpty();
        entity.PaperIds.Should().BeEmpty();
    }

    [Fact]
    public void Update_ShouldUpdateProvidedValues()
    {
        var entity = ConferenceJournalEntity.Create(Guid.NewGuid(), "Old", null, null, null, null, ConferenceJournalType.Journal, [], null, null);
        var newTemplateId = Guid.NewGuid();

        entity.Update(name: "New", ranking: "B", url: "https://new.org",
            projectIds: new List<Guid>(), paperIds: new List<Guid>(),
            style: "ACM", templateIds: new List<Guid> { newTemplateId },
            texFile: "new.tex", pdfFile: "new.pdf", lastModifiedBy: "editor");

        entity.Name.Should().Be("New");
        entity.Ranking.Should().Be("B");
        entity.Url.Should().Be("https://new.org");
        entity.Style.Should().Be("ACM");
        entity.TemplateIds.Should().ContainSingle().Which.Should().Be(newTemplateId);
        entity.TexFile.Should().Be("new.tex");
        entity.PdfFile.Should().Be("new.pdf");
        entity.LastModifiedBy.Should().Be("editor");
    }

    [Fact]
    public void Update_ShouldKeepExisting_WhenNullsPassed()
    {
        var entity = ConferenceJournalEntity.Create(Guid.NewGuid(), "Name", "A", null, null, null, ConferenceJournalType.Journal, [], null, null);
        entity.Update();
        entity.Name.Should().Be("Name");
        entity.Ranking.Should().Be("A");
    }

    [Fact]
    public void UpdateFilePath_ShouldSetBothPaths_WhenBothProvided()
    {
        var entity = ConferenceJournalEntity.Create(Guid.NewGuid(), "N", null, null, null, null, ConferenceJournalType.Journal, [], null, null);
        entity.UpdateFilePath("https://tex.url", "https://pdf.url");
        entity.TexFile.Should().Be("https://tex.url");
        entity.PdfFile.Should().Be("https://pdf.url");
    }

    [Fact]
    public void UpdateFilePath_ShouldUpdate_WhenOnlyTexProvided()
    {
        var entity = ConferenceJournalEntity.Create(Guid.NewGuid(), "N", null, null, null, null, ConferenceJournalType.Journal, [], null, null);
        entity.UpdateFilePath("https://tex.url", null);
        entity.TexFile.Should().Be("https://tex.url");
        entity.PdfFile.Should().BeNull();
    }

    [Fact]
    public void UpdateFilePath_ShouldUpdate_WhenOnlyPdfProvided()
    {
        var entity = ConferenceJournalEntity.Create(Guid.NewGuid(), "N", null, null, null, null, ConferenceJournalType.Journal, [], null, null);
        entity.UpdateFilePath(null, "https://pdf.url");
        entity.TexFile.Should().BeNull();
        entity.PdfFile.Should().Be("https://pdf.url");
    }

    [Fact]
    public void UpdateFilePath_ShouldNotUpdate_WhenBothNullOrWhitespace()
    {
        var entity = ConferenceJournalEntity.Create(Guid.NewGuid(), "N", null, null, null, null, ConferenceJournalType.Journal, [], "old.tex", "old.pdf");
        entity.UpdateFilePath(null, null);
        entity.TexFile.Should().Be("old.tex");
        entity.PdfFile.Should().Be("old.pdf");
    }

    [Fact]
    public void UpdateFilePath_ShouldNotUpdate_WhenBothEmpty()
    {
        var entity = ConferenceJournalEntity.Create(Guid.NewGuid(), "N", null, null, null, null, ConferenceJournalType.Journal, [], "old.tex", "old.pdf");
        entity.UpdateFilePath("", "");
        entity.TexFile.Should().Be("old.tex");
        entity.PdfFile.Should().Be("old.pdf");
    }

    [Fact]
    public void UpdateFilePath_ShouldNotUpdate_WhenBothWhitespace()
    {
        var entity = ConferenceJournalEntity.Create(Guid.NewGuid(), "N", null, null, null, null, ConferenceJournalType.Journal, [], "old.tex", "old.pdf");
        entity.UpdateFilePath("  ", "  ");
        entity.TexFile.Should().Be("old.tex");
        entity.PdfFile.Should().Be("old.pdf");
    }
}
