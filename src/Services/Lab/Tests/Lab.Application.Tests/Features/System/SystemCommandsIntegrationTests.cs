using Lab.Application.Dtos.Projects;
using Lab.Application.Features.System;
using Lab.Application.Tests.Common;
using Lab.Domain.Entities;
using Lab.Domain.Enums;

namespace Lab.Application.Tests.Features.System;

public class SystemCommandsIntegrationTests : MartenTestBase
{
    protected override string SchemaName => "system_cmd_tests";

    private async Task<ConferenceJournalEntity> SeedJournalAsync()
    {
        var journal = ConferenceJournalEntity.Create(
            Guid.NewGuid(), "Test Journal", "Q1", "https://j.com", "0000-0001",
            "IEEE", ConferenceJournalType.Journal, [], null, null);
        Session.Store(journal);
        await Session.SaveChangesAsync();
        return journal;
    }

    private async Task<PaperEntity> SeedPaperAsync(Guid? journalId = null)
    {
        var paper = PaperEntity.Create(Guid.NewGuid(), "Test Paper",
            conferenceJournalId: journalId);
        Session.Store(paper);
        await Session.SaveChangesAsync();
        return paper;
    }

    private async Task<SectionEntity> SeedSectionAsync(Guid paperId, string title = "Introduction")
    {
        var section = SectionEntity.Create(
            Guid.NewGuid(), "", paperId, 1.0f, SectionStatus.NotStarted,
            isMainSection: true, title: title);
        Session.Store(section);
        await Session.SaveChangesAsync();
        return section;
    }

    [Fact]
    public async Task UpdateProjectRules_EmptyPaperIds_ShouldReturnTrue()
    {
        var dto = new UpdateProjectRulesDto { PaperIds = [], Context = "ctx", Domain = "dom" };
        var handler = new UpdateProjectRulesCommandHandler(Session);

        var result = await handler.Handle(new UpdateProjectRulesCommand(dto), CancellationToken.None);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateProjectRules_PaperIdsNotInDb_ShouldReturnTrue()
    {
        var dto = new UpdateProjectRulesDto { PaperIds = [Guid.NewGuid()], Context = "ctx" };
        var handler = new UpdateProjectRulesCommandHandler(Session);

        var result = await handler.Handle(new UpdateProjectRulesCommand(dto), CancellationToken.None);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateProjectRules_PapersExistButNoSections_ShouldReturnTrue()
    {
        var journal = await SeedJournalAsync();
        var paper = await SeedPaperAsync(journal.Id);

        var dto = new UpdateProjectRulesDto { PaperIds = [paper.Id], Context = "ctx", Domain = "dom" };
        var handler = new UpdateProjectRulesCommandHandler(Session);

        var result = await handler.Handle(new UpdateProjectRulesCommand(dto), CancellationToken.None);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateProjectRules_WithPapersAndSections_ShouldUpdateSectionsAndReturnTrue()
    {
        var journal = await SeedJournalAsync();
        var paper = await SeedPaperAsync(journal.Id);
        var section = await SeedSectionAsync(paper.Id, "Introduction");

        var dto = new UpdateProjectRulesDto
        {
            PaperIds = [paper.Id],
            Context = "Test context",
            Domain = "Computer Science",
            Keypoint = "Key contributions"
        };
        var handler = new UpdateProjectRulesCommandHandler(Session);

        var result = await handler.Handle(new UpdateProjectRulesCommand(dto), CancellationToken.None);

        result.Should().BeTrue();
        var updated = await Session.LoadAsync<SectionEntity>(section.Id);
        updated.Should().NotBeNull();
        updated!.ProjectRule.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task UpdateProjectRules_PaperMissingJournal_ShouldThrowNotFoundException()
    {
        // Paper has a journalId that doesn't exist in DB
        var paper = await SeedPaperAsync(Guid.NewGuid());
        var section = await SeedSectionAsync(paper.Id, "Methods");

        var dto = new UpdateProjectRulesDto { PaperIds = [paper.Id], Context = "ctx" };
        var handler = new UpdateProjectRulesCommandHandler(Session);

        var act = () => handler.Handle(new UpdateProjectRulesCommand(dto), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateProjectRules_OnlyEmptyGuidInPaperIds_ShouldReturnTrue()
    {
        var dto = new UpdateProjectRulesDto { PaperIds = [Guid.Empty], Context = "ctx" };
        var handler = new UpdateProjectRulesCommandHandler(Session);

        var result = await handler.Handle(new UpdateProjectRulesCommand(dto), CancellationToken.None);

        result.Should().BeTrue();
    }
}
