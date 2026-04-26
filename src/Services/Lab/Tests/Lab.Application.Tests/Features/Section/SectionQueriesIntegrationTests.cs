using Common.Constants;
using Lab.Application.Dtos.Sections;
using Lab.Application.Features.Section.Commands.UpsertSection;
using Lab.Application.Features.Section.Queries.GetReferenceBySectionId;
using Lab.Application.Tests.Common;
using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Lab.Domain.Models;

namespace Lab.Application.Tests.Features.Section;

public class SectionQueriesIntegrationTests : MartenTestBase
{
    protected override string SchemaName => "section_queries_tests";

    private readonly Mock<IManagementApiService> _mockMgmt = new();

    // ─── Helpers ──────────────────────────────────────────────────────────────

    private SectionEntity SeedMainSection(Guid? paperId = null, string title = "Introduction")
    {
        var section = SectionEntity.Create(
            id: Guid.NewGuid(),
            content: "content",
            paperId: paperId ?? Guid.NewGuid(),
            displayOrder: 1,
            status: SectionStatus.NotStarted,
            isMainSection: true,
            version: "Version Initial",
            title: title);
        Session.Store(section);
        return section;
    }

    private PaperContributorEntity SeedContributor(
        Guid paperId, Guid memberId, Guid sectionId, Guid markSectionId,
        string role = AuthorizeConstants.SectionEdit)
    {
        var c = PaperContributorEntity.Create(Guid.NewGuid(), role, paperId, sectionId, memberId, markSectionId);
        Session.Store(c);
        return c;
    }

    // ─── GetReferenceBySectionIdQueryHandler ─────────────────────────────────

    [Fact]
    public async Task GetReferenceBySectionId_WithNonExistentSection_ShouldThrowNotFoundException()
    {
        var handler = new GetReferenceBySectionIdQueryHandler(Session);

        var act = () => handler.Handle(
            new GetReferenceBySectionIdQuery(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetReferenceBySectionId_WhenPaperNotFound_ShouldThrowNotFoundException()
    {
        // Seed a section but NO paper with matching PaperId
        var section = SectionEntity.Create(
            id: Guid.NewGuid(),
            content: "content",
            paperId: Guid.NewGuid(), // paper not in DB
            displayOrder: 1,
            status: SectionStatus.NotStarted,
            isMainSection: true,
            title: "Intro");
        Session.Store(section);
        await Session.SaveChangesAsync();

        var handler = new GetReferenceBySectionIdQueryHandler(Session);

        var act = () => handler.Handle(
            new GetReferenceBySectionIdQuery(section.Id), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetReferenceBySectionId_WithSectionAndPaper_NoReferences_ShouldReturnEmpty()
    {
        var paperId = Guid.NewGuid();
        var paper = PaperEntity.Create(paperId, "Test Paper");
        Session.Store(paper);

        var section = SectionEntity.Create(
            id: Guid.NewGuid(),
            content: "content",
            paperId: paperId,
            displayOrder: 1,
            status: SectionStatus.NotStarted,
            isMainSection: true,
            title: "Introduction");
        Session.Store(section);
        await Session.SaveChangesAsync();

        var handler = new GetReferenceBySectionIdQueryHandler(Session);
        var result = await handler.Handle(
            new GetReferenceBySectionIdQuery(section.Id), CancellationToken.None);

        result.Should().NotBeNull();
        result.InUse.Should().BeEmpty();
        result.OtherReference.Should().BeEmpty();
    }

    [Fact]
    public async Task GetReferenceBySectionId_WithInUsePaperBankReferences_ShouldReturnInUse()
    {
        var paperId = Guid.NewGuid();
        var paperBankId = Guid.NewGuid();

        var paperBank = PaperBankEntity.Create(paperBankId, "Referenced Paper");
        Session.Store(paperBank);

        var paper = PaperEntity.Create(paperId, "Test Paper");
        Session.Store(paper);

        var section = SectionEntity.Create(
            id: Guid.NewGuid(),
            content: "content",
            paperId: paperId,
            displayOrder: 1,
            status: SectionStatus.NotStarted,
            isMainSection: true,
            title: "Introduction",
            references: new List<Guid> { paperBankId });
        Session.Store(section);
        await Session.SaveChangesAsync();

        var handler = new GetReferenceBySectionIdQueryHandler(Session);
        var result = await handler.Handle(
            new GetReferenceBySectionIdQuery(section.Id), CancellationToken.None);

        result.InUse.Should().HaveCount(1);
        result.InUse[0].Title.Should().Be("Referenced Paper");
    }

    [Fact]
    public async Task GetReferenceBySectionId_WithOtherPaperReferences_ShouldReturnOtherReference()
    {
        var paperId = Guid.NewGuid();
        var sectionId = Guid.NewGuid();
        var otherPaperBankId = Guid.NewGuid();
        var otherSectionId = Guid.NewGuid();

        var otherPaperBank = PaperBankEntity.Create(otherPaperBankId, "Other Referenced Paper");
        Session.Store(otherPaperBank);

        // The paper has references to another paper bank, linked through a different section
        var paper = PaperEntity.Create(paperId, "Test Paper", references: new List<Reference>
        {
            new() { PaperId = otherPaperBankId, SectionIds = new List<Guid> { otherSectionId } }
        });
        Session.Store(paper);

        var section = SectionEntity.Create(
            id: sectionId,
            content: "content",
            paperId: paperId,
            displayOrder: 1,
            status: SectionStatus.NotStarted,
            isMainSection: true,
            title: "Introduction");
        Session.Store(section);
        await Session.SaveChangesAsync();

        var handler = new GetReferenceBySectionIdQueryHandler(Session);
        var result = await handler.Handle(
            new GetReferenceBySectionIdQuery(sectionId), CancellationToken.None);

        result.OtherReference.Should().HaveCount(1);
        result.OtherReference[0].PaperBank.Title.Should().Be("Other Referenced Paper");
    }

    // ─── UpsertSectionCommandHandler (main section creates new version) ───────

    [Fact]
    public async Task UpsertSection_MainSectionWithNoExistingVersion_ShouldCreateNewVersion()
    {
        var paperId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var section = SectionEntity.Create(
            id: Guid.NewGuid(),
            content: "original content",
            paperId: paperId,
            displayOrder: 1,
            status: SectionStatus.NotStarted,
            isMainSection: true,
            version: "Version Initial",
            title: "Introduction");
        Session.Store(section);

        var contributor = SeedContributor(paperId, memberId, section.Id, section.Id, AuthorizeConstants.SectionEdit);
        await Session.SaveChangesAsync();

        var handler = new UpsertSectionCommandHandler(Session);
        var dto = new UpsertSectionDto { MemberId = memberId, Content = "new content", Title = "Updated Title" };

        var result = await handler.Handle(
            new UpsertSectionCommand(dto, section.Id, "author"),
            CancellationToken.None);

        result.Should().NotBeEmpty();
        result.Should().NotBe(section.Id); // A NEW section was created

        var newVersion = await Session.LoadAsync<SectionEntity>(result);
        newVersion.Should().NotBeNull();
        newVersion!.Content.Should().Be("new content");
        newVersion.IsMainSection.Should().BeFalse();
        newVersion.PreviousVersionSectionId.Should().Be(section.Id);
    }
}
