namespace Lab.Domain.Tests.Entities;

public sealed class PaperEntityTests
{
    [Fact]
    public void Create_ShouldInitializeWithCorrectValues()
    {
        var id = Guid.NewGuid();
        var journalId = Guid.NewGuid();
        var refs = new List<Reference> { new() { PaperId = Guid.NewGuid() } };

        var gapTypeIds = new List<Guid> { Guid.NewGuid() };

        var entity = PaperEntity.Create(id, "Test Paper",
            template: "IMRAD", context: "AI Research",
            abstractText: "Abstract text", researchGap: "Gap",
            mainContribution: "Contribution", researchAim: "Aim",
            rule: "Rule", gapTypeIds: gapTypeIds,
            conferenceJournalName: "Journal", conferenceJournalId: journalId,
            conferenceJournalStartAt: DateTimeOffset.UtcNow,
            conferenceJournalEndAt: DateTimeOffset.UtcNow.AddDays(3),
            status: PaperStatus.Draft, references: refs,
            createdBy: "admin");

        entity.Id.Should().Be(id);
        entity.Title.Should().Be("Test Paper");
        entity.Template.Should().Be("IMRAD");
        entity.Context.Should().Be("AI Research");
        entity.Abstract.Should().Be("Abstract text");
        entity.ResearchGap.Should().Be("Gap");
        entity.MainContribution.Should().Be("Contribution");
        entity.ResearchAim.Should().Be("Aim");
        entity.Rule.Should().Be("Rule");
        entity.GapTypeIds.Should().BeEquivalentTo(gapTypeIds);
        entity.ConferenceJournalName.Should().Be("Journal");
        entity.ConferenceJournalId.Should().Be(journalId);
        entity.Status.Should().Be(PaperStatus.Draft);
        entity.References.Should().HaveCount(1);
        entity.CreatedBy.Should().Be("admin");
    }

    [Fact]
    public void Create_ShouldUseDefaults_WhenOptionalParametersOmitted()
    {
        var entity = PaperEntity.Create(Guid.NewGuid(), "Title");

        entity.Status.Should().Be(PaperStatus.Processing);
        entity.References.Should().BeEmpty();
        entity.CreatedBy.Should().BeNull();
    }

    [Fact]
    public void Update_ShouldUpdateProvidedValues()
    {
        var entity = PaperEntity.Create(Guid.NewGuid(), "Old Title");
        var journalId = Guid.NewGuid();

        entity.Update(title: "New Title", template: "IEEE",
            context: "New Context", abstractText: "New Abstract",
            researchGap: "New Gap", mainContribution: "New Contribution",
            researchAim: "New Aim", rule: "New Rule",
            conferenceJournalName: "New Journal", conferenceJournalId: journalId,
            conferenceJournalStartAt: DateTimeOffset.UtcNow,
            conferenceJournalEndAt: DateTimeOffset.UtcNow.AddDays(5),
            status: PaperStatus.Released, gapTypeIds: new List<Guid> { Guid.NewGuid() },
            references: new List<Reference>(), lastModifiedBy: "editor");

        entity.Title.Should().Be("New Title");
        entity.Template.Should().Be("IEEE");
        entity.GapTypeIds.Should().ContainSingle();
        entity.Status.Should().Be(PaperStatus.Released);
        entity.LastModifiedBy.Should().Be("editor");
    }

    [Fact]
    public void Update_ShouldKeepExistingValues_WhenNullsPassed()
    {
        var entity = PaperEntity.Create(Guid.NewGuid(), "Title", template: "IMRAD");
        entity.Update();
        entity.Title.Should().Be("Title");
        entity.Template.Should().Be("IMRAD");
    }

    [Fact]
    public void UpdateFilePath_ShouldSetFilePath_WhenUrlValid()
    {
        var entity = PaperEntity.Create(Guid.NewGuid(), "Title");
        entity.UpdateFilePath("https://example.com/paper.pdf");
        entity.FilePath.Should().Be("https://example.com/paper.pdf");
    }

    [Fact]
    public void UpdateFilePath_ShouldNotUpdate_WhenUrlIsNull()
    {
        var entity = PaperEntity.Create(Guid.NewGuid(), "Title");
        entity.UpdateFilePath(null);
        entity.FilePath.Should().BeNull();
    }

    [Fact]
    public void UpdateFilePath_ShouldNotUpdate_WhenUrlIsEmpty()
    {
        var entity = PaperEntity.Create(Guid.NewGuid(), "Title");
        entity.UpdateFilePath("");
        entity.FilePath.Should().BeNull();
    }

    [Fact]
    public void UpdateFilePath_ShouldNotUpdate_WhenUrlIsWhitespace()
    {
        var entity = PaperEntity.Create(Guid.NewGuid(), "Title");
        entity.UpdateFilePath("   ");
        entity.FilePath.Should().BeNull();
    }
}
