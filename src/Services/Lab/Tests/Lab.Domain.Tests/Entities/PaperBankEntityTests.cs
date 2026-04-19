namespace Lab.Domain.Tests.Entities;

public sealed class PaperBankEntityTests
{
    [Fact]
    public void Create_ShouldInitializeWithCorrectValues()
    {
        var id = Guid.NewGuid();
        var entity = PaperBankEntity.Create(id, "Test Paper",
            authors: "Author1", publisher: "Publisher1", ranking: "A",
            abstractText: "Abstract", doi: "10.1234", parsedText: "parsed",
            isIngested: true, isAutoTagged: true,
            publicationDate: DateTimeOffset.UtcNow, paperType: "journal",
            journalName: "Journal1", pages: "1-10", number: "1",
            volume: "5", conferenceName: "Conf1", referenceContent: "ref",
            tagNames: new List<string> { "tag1" }, ingestStatus: IngestStatus.Success);

        entity.Id.Should().Be(id);
        entity.Title.Should().Be("Test Paper");
        entity.Authors.Should().Be("Author1");
        entity.Publisher.Should().Be("Publisher1");
        entity.Ranking.Should().Be("A");
        entity.Abstract.Should().Be("Abstract");
        entity.Doi.Should().Be("10.1234");
        entity.ParsedText.Should().Be("parsed");
        entity.IsIngested.Should().BeTrue();
        entity.IsAutoTagged.Should().BeTrue();
        entity.PaperType.Should().Be("journal");
        entity.JournalName.Should().Be("Journal1");
        entity.Pages.Should().Be("1-10");
        entity.Number.Should().Be("1");
        entity.Volume.Should().Be("5");
        entity.ConferenceName.Should().Be("Conf1");
        entity.ReferenceContent.Should().Be("ref");
        entity.TagNames.Should().ContainSingle("tag1");
        entity.IngestStatus.Should().Be(IngestStatus.Success);
        entity.CreatedOnUtc.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Create_ShouldUseDefaults_WhenOptionalParametersOmitted()
    {
        var entity = PaperBankEntity.Create(Guid.NewGuid(), "Title");

        entity.ParsedText.Should().Be(string.Empty);
        entity.IsIngested.Should().BeFalse();
        entity.IsAutoTagged.Should().BeFalse();
        entity.TagNames.Should().BeEmpty();
        entity.IngestStatus.Should().Be(IngestStatus.Pending);
    }

    [Fact]
    public void Update_ShouldUpdateProvidedValues()
    {
        var entity = PaperBankEntity.Create(Guid.NewGuid(), "Old Title");

        entity.Update(title: "New Title", authors: "New Author", publisher: "New Pub",
            ranking: "B", abstractText: "New Abstract", doi: "10.5678",
            isIngested: true, isAutoTagged: true,
            publicationDate: DateTimeOffset.UtcNow, paperType: "conf",
            journalName: "J2", pages: "11-20", number: "2",
            volume: "10", conferenceName: "C2", referenceContent: "ref2",
            ingestStatus: IngestStatus.Failed, tagNames: new List<string> { "t2" });

        entity.Title.Should().Be("New Title");
        entity.Authors.Should().Be("New Author");
        entity.Publisher.Should().Be("New Pub");
        entity.Ranking.Should().Be("B");
        entity.IngestStatus.Should().Be(IngestStatus.Failed);
        entity.TagNames.Should().ContainSingle("t2");
    }

    [Fact]
    public void Update_ShouldKeepExistingValues_WhenNullsPassed()
    {
        var entity = PaperBankEntity.Create(Guid.NewGuid(), "Title", authors: "A1");
        entity.Update();
        entity.Title.Should().Be("Title");
        entity.Authors.Should().Be("A1");
    }

    [Fact]
    public void UpdateIngestionStatus_ShouldUpdateFields()
    {
        var entity = PaperBankEntity.Create(Guid.NewGuid(), "Title");

        entity.UpdateIngestionStatus(true, IngestStatus.Success);

        entity.IsIngested.Should().BeTrue();
        entity.IngestStatus.Should().Be(IngestStatus.Success);
        entity.LastModifiedOnUtc.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void UpdateFilePath_ShouldSetFilePath_WhenUrlValid()
    {
        var entity = PaperBankEntity.Create(Guid.NewGuid(), "Title");
        entity.UpdateFilePath("https://example.com/file.pdf");
        entity.FilePath.Should().Be("https://example.com/file.pdf");
    }

    [Fact]
    public void UpdateFilePath_ShouldNotUpdate_WhenUrlIsNull()
    {
        var entity = PaperBankEntity.Create(Guid.NewGuid(), "Title");
        entity.UpdateFilePath(null);
        entity.FilePath.Should().BeNull();
    }

    [Fact]
    public void UpdateFilePath_ShouldNotUpdate_WhenUrlIsEmpty()
    {
        var entity = PaperBankEntity.Create(Guid.NewGuid(), "Title");
        entity.UpdateFilePath("");
        entity.FilePath.Should().BeNull();
    }

    [Fact]
    public void UpdateFilePath_ShouldNotUpdate_WhenUrlIsWhitespace()
    {
        var entity = PaperBankEntity.Create(Guid.NewGuid(), "Title");
        entity.UpdateFilePath("   ");
        entity.FilePath.Should().BeNull();
    }
}
