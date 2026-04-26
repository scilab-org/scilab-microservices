using Common.Constants;
using Lab.Application.Dtos.Papers;
using Lab.Application.Dtos.Sections;
using Lab.Application.Features.Paper.Commands.CreatePaper;
using Lab.Application.Services;
using Lab.Application.Tests.Common;
using Lab.Domain.Entities;
using Lab.Domain.Enums;

namespace Lab.Application.Tests.Features.Paper;

public class CreatePaperCommandsIntegrationTests : MartenTestBase
{
    protected override string SchemaName => "paper_create_tests";

    private readonly Mock<IManagementApiService> _mockMgmt = new();

    private static ManagementProjectInfo FakeProject(Guid id) =>
        new(id, "Test Project", "TP", "Desc", "Active", null, null, "Context", "AI", "KP");

    private async Task<ConferenceJournalEntity> SeedJournalAsync()
    {
        var journal = ConferenceJournalEntity.Create(
            Guid.NewGuid(), "Test Journal", "Q1", "https://j.com", "1234-5678",
            "IEEE", ConferenceJournalType.Journal, [], null, null);
        Session.Store(journal);
        await Session.SaveChangesAsync();
        return journal;
    }

    private async Task<GapTypeEntity> SeedGapTypeAsync(string name = "Empirical Gap")
    {
        var gapType = GapTypeEntity.Create(Guid.NewGuid(), name);
        Session.Store(gapType);
        await Session.SaveChangesAsync();
        return gapType;
    }

    private static CreatePaperDto BuildDto(Guid projectId, Guid journalId, List<Guid>? gapTypeIds = null,
        List<CreateSectionDto>? sections = null)
        => new()
        {
            ProjectId = projectId,
            Title = "Test Paper",
            Template = "IMRAD",
            Context = "AI Research Context",
            Abstract = "Abstract text",
            ResearchGap = "Research gap description",
            GapTypeIds = gapTypeIds ?? new List<Guid>(),
            MainContribution = "Main contribution",
            ResearchAim = "Research aim",
            ConferenceJournalId = journalId,
            ConferenceJournalName = "Test Journal",
            Sections = sections
        };

    #region Success paths

    [Fact]
    public async Task CreatePaper_WithValidData_ShouldStoreAndReturnId()
    {
        var journal = await SeedJournalAsync();
        var gapType = await SeedGapTypeAsync();
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var subProjectId = Guid.NewGuid();

        _mockMgmt.Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthorizeConstants.ProjectAuthor);
        _mockMgmt.Setup(x => x.GetProjectByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeProject(projectId));
        _mockMgmt.Setup(x => x.CreateSubProjectAsync(projectId, It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(subProjectId);
        _mockMgmt.Setup(x => x.AddSubProjectMembersAsync(subProjectId, It.IsAny<IEnumerable<(Guid, string)>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockMgmt.Setup(x => x.AddProjectConferenceJournalsAsync(projectId, journal.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var dto = BuildDto(projectId, journal.Id, new List<Guid> { gapType.Id });
        var handler = new CreatePaperCommandHandler(Session, _mockMgmt.Object);

        var result = await handler.Handle(new CreatePaperCommand(dto, userId, "user1"), CancellationToken.None);

        result.Should().NotBeEmpty();
        var stored = await Session.LoadAsync<PaperEntity>(result);
        stored.Should().NotBeNull();
        stored!.Title.Should().Be("Test Paper");
        stored.Template.Should().Be("IMRAD");
        stored.Status.Should().Be(PaperStatus.Draft);
        stored.ConferenceJournalId.Should().Be(journal.Id);
    }

    [Fact]
    public async Task CreatePaper_WithSections_ShouldStoreSectionsAlso()
    {
        var journal = await SeedJournalAsync();
        var gapType = await SeedGapTypeAsync();
        var projectId = Guid.NewGuid();

        _mockMgmt.Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthorizeConstants.ProjectAuthor);
        _mockMgmt.Setup(x => x.GetProjectByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeProject(projectId));
        _mockMgmt.Setup(x => x.CreateSubProjectAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null);
        _mockMgmt.Setup(x => x.AddProjectConferenceJournalsAsync(projectId, journal.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var sections = new List<CreateSectionDto>
        {
            new() { Title = "Introduction", DisplayOrder = 1, SectionRule = "Rule1", MainIdea = "Intro idea" },
            new() { Title = "Conclusion", DisplayOrder = 2, SectionRule = "Rule2", MainIdea = "Conclusion idea" },
        };

        var dto = BuildDto(projectId, journal.Id, new List<Guid> { gapType.Id }, sections);
        var handler = new CreatePaperCommandHandler(Session, _mockMgmt.Object);

        var paperId = await handler.Handle(new CreatePaperCommand(dto, Guid.NewGuid(), "user1"), CancellationToken.None);

        var storedSections = await Session.Query<SectionEntity>()
            .Where(s => s.PaperId == paperId)
            .ToListAsync();
        storedSections.Should().HaveCount(2);
        storedSections.Should().Contain(s => s.Title == "Introduction");
        storedSections.Should().Contain(s => s.Title == "Conclusion");
    }

    [Fact]
    public async Task CreatePaper_WithEmptyProjectId_ShouldSkipPostSaveManagementCalls()
    {
        var journal = await SeedJournalAsync();
        var gapType = await SeedGapTypeAsync();
        var emptyProjectId = Guid.Empty;

        _mockMgmt.Setup(x => x.GetMyProjectRoleAsync(emptyProjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);
        _mockMgmt.Setup(x => x.GetProjectByIdAsync(emptyProjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeProject(emptyProjectId));

        var dto = BuildDto(emptyProjectId, journal.Id, new List<Guid> { gapType.Id });
        var handler = new CreatePaperCommandHandler(Session, _mockMgmt.Object);

        var result = await handler.Handle(new CreatePaperCommand(dto, Guid.NewGuid(), "user1"), CancellationToken.None);

        result.Should().NotBeEmpty();
        _mockMgmt.Verify(x => x.CreateSubProjectAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockMgmt.Verify(x => x.AddProjectConferenceJournalsAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreatePaper_WithEmptyGapTypeIds_ShouldSucceed()
    {
        var journal = await SeedJournalAsync();
        var projectId = Guid.NewGuid();

        _mockMgmt.Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthorizeConstants.ProjectAuthor);
        _mockMgmt.Setup(x => x.GetProjectByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeProject(projectId));
        _mockMgmt.Setup(x => x.CreateSubProjectAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null);
        _mockMgmt.Setup(x => x.AddProjectConferenceJournalsAsync(projectId, journal.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var dto = BuildDto(projectId, journal.Id, new List<Guid>());
        var handler = new CreatePaperCommandHandler(Session, _mockMgmt.Object);

        var result = await handler.Handle(new CreatePaperCommand(dto, Guid.NewGuid(), "user"), CancellationToken.None);

        result.Should().NotBeEmpty();
    }

    #endregion

    #region Failure paths

    [Fact]
    public async Task CreatePaper_WithNonExistentJournal_ShouldThrowNotFoundException()
    {
        var projectId = Guid.NewGuid();

        _mockMgmt.Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthorizeConstants.ProjectAuthor);
        _mockMgmt.Setup(x => x.GetProjectByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeProject(projectId));

        var dto = BuildDto(projectId, Guid.NewGuid()); // journal not in DB
        var handler = new CreatePaperCommandHandler(Session, _mockMgmt.Object);

        var act = () => handler.Handle(new CreatePaperCommand(dto, Guid.NewGuid(), "user"), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreatePaper_WithMissingGapType_ShouldThrowNotFoundException()
    {
        var journal = await SeedJournalAsync();
        var projectId = Guid.NewGuid();

        _mockMgmt.Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthorizeConstants.ProjectAuthor);
        _mockMgmt.Setup(x => x.GetProjectByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeProject(projectId));

        var dto = BuildDto(projectId, journal.Id, new List<Guid> { Guid.NewGuid() }); // gapType not in DB
        var handler = new CreatePaperCommandHandler(Session, _mockMgmt.Object);

        var act = () => handler.Handle(new CreatePaperCommand(dto, Guid.NewGuid(), "user"), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreatePaper_WithUnauthorizedRole_ShouldThrowNoPermissionException()
    {
        var projectId = Guid.NewGuid();

        _mockMgmt.Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync("ProjectMember"); // not author role

        var dto = BuildDto(projectId, Guid.NewGuid());
        var handler = new CreatePaperCommandHandler(Session, _mockMgmt.Object);

        var act = () => handler.Handle(new CreatePaperCommand(dto, Guid.NewGuid(), "user"), CancellationToken.None);

        await act.Should().ThrowAsync<NoPermissionException>();
    }

    [Fact]
    public async Task CreatePaper_WhenAddProjectJournalFails_ShouldThrowNotFoundException()
    {
        var journal = await SeedJournalAsync();
        var gapType = await SeedGapTypeAsync();
        var projectId = Guid.NewGuid();

        _mockMgmt.Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthorizeConstants.ProjectAuthor);
        _mockMgmt.Setup(x => x.GetProjectByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeProject(projectId));
        _mockMgmt.Setup(x => x.CreateSubProjectAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null);
        _mockMgmt.Setup(x => x.AddProjectConferenceJournalsAsync(projectId, journal.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false); // management call fails

        var dto = BuildDto(projectId, journal.Id, new List<Guid> { gapType.Id });
        var handler = new CreatePaperCommandHandler(Session, _mockMgmt.Object);

        var act = () => handler.Handle(new CreatePaperCommand(dto, Guid.NewGuid(), "user"), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion
}
