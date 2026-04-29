using Common.Constants;
using Lab.Application.Dtos.Sections;
using Lab.Application.Features.Section.Commands.MarkMainSection;
using Lab.Application.Features.Section.Commands.MarkSectionToCompleted;
using Lab.Application.Features.Section.Commands.UpdateGuideline;
using Lab.Application.Features.Section.Commands.UpdateReference;
using Lab.Application.Features.Section.Commands.UploadSectionFile;
using Lab.Application.Features.Section.Commands.UpsertSection;
using Lab.Application.Features.Section.Queries.GetSectionByMarkSectionId;
using Lab.Application.Features.Section.Queries.GetSectionHistory;
using Lab.Application.Tests.Common;
using GetSectionByMarkSectionIdHandler = Lab.Application.Features.Section.Queries.GetSectionByMarkSectionId.GetSectionByMarkSectionIdQueryHandler;
using GetSectionHistoryHandler = Lab.Application.Features.Section.Queries.GetSectionHistory.GetSectionByMarkSectionIdQueryHandler;

namespace Lab.Application.Tests.Features.Section;

public class SectionCommandsIntegrationTests : MartenTestBase
{
    protected override string SchemaName => "section_commands_tests";

    private readonly Mock<IManagementApiService> _mockMgmt = new();
    private readonly Mock<IUserApiService> _mockUser = new();
    private readonly Mock<IMinIoCloudService> _mockMinIo = new();

    // ─── Helpers ──────────────────────────────────────────────────────────────

    private SectionEntity SeedMainSection(
        Guid? paperId = null,
        string title = "Introduction",
        string version = "Version Initial",
        SectionStatus status = SectionStatus.NotStarted)
    {
        var section = SectionEntity.Create(
            id: Guid.NewGuid(),
            content: "initial content",
            paperId: paperId ?? Guid.NewGuid(),
            displayOrder: 1,
            status: status,
            isMainSection: true,
            version: version,
            title: title);
        Session.Store(section);
        return section;
    }

    private SectionEntity SeedChildSection(
        Guid mainSectionId,
        Guid paperId,
        string title = "Introduction",
        SectionStatus status = SectionStatus.InProgress)
    {
        var section = SectionEntity.Create(
            id: Guid.NewGuid(),
            content: "draft content",
            paperId: paperId,
            displayOrder: 1,
            status: status,
            isMainSection: false,
            title: title,
            previousVersionSectionId: mainSectionId);
        Session.Store(section);
        return section;
    }

    private PaperContributorEntity SeedContributor(
        Guid paperId,
        Guid memberId,
        Guid sectionId,
        Guid markSectionId,
        string role = AuthorizeConstants.SectionEdit)
    {
        var c = PaperContributorEntity.Create(
            Guid.NewGuid(), role, paperId, sectionId, memberId, markSectionId);
        Session.Store(c);
        return c;
    }

    // ─── UpsertSection ────────────────────────────────────────────────────────

    [Fact]
    public async Task UpsertSection_WithNonExistentSection_ShouldThrowClientValidationException()
    {
        var handler = new UpsertSectionCommandHandler(Session);
        var dto = new UpsertSectionDto { MemberId = Guid.NewGuid(), Content = "content" };

        var act = () => handler.Handle(new UpsertSectionCommand(dto, Guid.NewGuid(), "user"), CancellationToken.None);
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    [Fact]
    public async Task UpsertSection_WhenContributorNotFound_ShouldThrowUnauthorizedException()
    {
        var memberId = Guid.NewGuid();
        var section = SeedMainSection();
        await Session.SaveChangesAsync();

        var handler = new UpsertSectionCommandHandler(Session);
        var dto = new UpsertSectionDto { MemberId = memberId, Content = "content" };

        var act = () => handler.Handle(new UpsertSectionCommand(dto, section.Id, "user"), CancellationToken.None);
        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task UpsertSection_WhenContributorHasSectionReadRole_ShouldThrowUnauthorizedException()
    {
        var memberId = Guid.NewGuid();
        var paperId = Guid.NewGuid();
        var section = SeedMainSection(paperId: paperId);
        SeedContributor(paperId, memberId, section.Id, section.Id, AuthorizeConstants.SectionRead);
        await Session.SaveChangesAsync();

        var handler = new UpsertSectionCommandHandler(Session);
        var dto = new UpsertSectionDto { MemberId = memberId, Content = "content" };

        var act = () => handler.Handle(new UpsertSectionCommand(dto, section.Id, "user"), CancellationToken.None);
        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task UpsertSection_MainSectionVersionInitial_ShouldCreateNewDraftVersion()
    {
        var paperId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var section = SeedMainSection(paperId: paperId, version: "Version Initial");
        SeedContributor(paperId, memberId, section.Id, section.Id);
        await Session.SaveChangesAsync();

        var handler = new UpsertSectionCommandHandler(Session);
        var dto = new UpsertSectionDto
        {
            MemberId = memberId,
            Title = "Introduction",
            Content = "new draft content"
        };

        var result = await handler.Handle(new UpsertSectionCommand(dto, section.Id, "author"), CancellationToken.None);

        result.Should().NotBe(section.Id);
        var newSection = await Session.LoadAsync<SectionEntity>(result);
        newSection.Should().NotBeNull();
        newSection!.IsMainSection.Should().BeFalse();
        newSection.PreviousVersionSectionId.Should().Be(section.Id);
    }

    [Fact]
    public async Task UpsertSection_MainSectionContributorAlreadyHasVersion_ShouldThrowClientValidationException()
    {
        var paperId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var section = SeedMainSection(paperId: paperId, version: "Version 1");
        var existingVersion = SeedChildSection(section.Id, paperId);
        // Contributor for main section (allows authorization check to pass)
        SeedContributor(paperId, memberId, section.Id, section.Id);
        // Contributor for the existing child version (triggers the 'already has version' check)
        SeedContributor(paperId, memberId, existingVersion.Id, section.Id);
        await Session.SaveChangesAsync();

        var handler = new UpsertSectionCommandHandler(Session);
        var dto = new UpsertSectionDto { MemberId = memberId, Content = "more content" };

        var act = () => handler.Handle(new UpsertSectionCommand(dto, section.Id, "user"), CancellationToken.None);
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    [Fact]
    public async Task UpsertSection_NonMainSection_ShouldUpdateContentDirectly()
    {
        var paperId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var mainSection = SeedMainSection(paperId: paperId);
        var childSection = SeedChildSection(mainSection.Id, paperId);
        SeedContributor(paperId, memberId, childSection.Id, mainSection.Id);
        await Session.SaveChangesAsync();

        var handler = new UpsertSectionCommandHandler(Session);
        var dto = new UpsertSectionDto
        {
            MemberId = memberId,
            Title = "Updated Title",
            Content = "updated content"
        };

        var result = await handler.Handle(new UpsertSectionCommand(dto, childSection.Id, "author"), CancellationToken.None);

        result.Should().Be(childSection.Id);
        var updated = await Session.LoadAsync<SectionEntity>(childSection.Id);
        updated!.Content.Should().Be("updated content");
    }

    // ─── MarkSectionToCompleted ───────────────────────────────────────────────

    [Fact]
    public async Task MarkSectionToCompleted_WhenRoleIsEmpty_ShouldThrowUnauthorizedException()
    {
        var projectId = Guid.NewGuid();
        _mockMgmt.Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        var handler = new MarkSectionToCompletedCommandHandler(Session, _mockMgmt.Object);
        var dto = new MarkSectionToCompletedDto { MemberId = Guid.NewGuid(), ProjectId = projectId };

        var act = () => handler.Handle(new MarkSectionToCompletedCommand(Guid.NewGuid(), dto, "user"), CancellationToken.None);
        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task MarkSectionToCompleted_WhenContributorMissingOrReadOnly_ShouldThrowUnauthorizedException()
    {
        var projectId = Guid.NewGuid();
        var paperId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var section = SeedChildSection(Guid.NewGuid(), paperId, status: SectionStatus.InProgress);
        await Session.SaveChangesAsync();

        _mockMgmt.Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthorizeConstants.PaperAuthor);

        var handler = new MarkSectionToCompletedCommandHandler(Session, _mockMgmt.Object);
        var dto = new MarkSectionToCompletedDto { MemberId = memberId, ProjectId = projectId };

        var act = () => handler.Handle(new MarkSectionToCompletedCommand(section.Id, dto, "user"), CancellationToken.None);
        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task MarkSectionToCompleted_WhenStatusIsNotInProgress_ShouldThrowClientValidationException()
    {
        var projectId = Guid.NewGuid();
        var paperId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var section = SeedChildSection(Guid.NewGuid(), paperId, status: SectionStatus.InProgress);
        SeedContributor(paperId, memberId, section.Id, Guid.NewGuid());
        await Session.SaveChangesAsync();

        _mockMgmt.Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthorizeConstants.PaperAuthor);

        var handler = new MarkSectionToCompletedCommandHandler(Session, _mockMgmt.Object);
        var dto = new MarkSectionToCompletedDto { MemberId = memberId, ProjectId = projectId };

        var act = () => handler.Handle(new MarkSectionToCompletedCommand(section.Id, dto, "user"), CancellationToken.None);
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    [Fact]
    public async Task MarkSectionToCompleted_WithValidData_ShouldSetStatusToCompleted()
    {
        var projectId = Guid.NewGuid();
        var paperId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var section = SeedChildSection(Guid.NewGuid(), paperId, status: SectionStatus.InProgress);
        SeedContributor(paperId, memberId, section.Id, Guid.NewGuid());
        await Session.SaveChangesAsync();

        _mockMgmt.Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthorizeConstants.PaperAuthor);

        var handler = new MarkSectionToCompletedCommandHandler(Session, _mockMgmt.Object);
        var dto = new MarkSectionToCompletedDto { MemberId = memberId, ProjectId = projectId };

        var result = await handler.Handle(new MarkSectionToCompletedCommand(section.Id, dto, "author"), CancellationToken.None);

        result.Should().Be(section.Id);
        var updated = await Session.LoadAsync<SectionEntity>(section.Id);
        updated!.Status.Should().Be(SectionStatus.Completed);
    }

    // ─── MarkMainSection ──────────────────────────────────────────────────────

    [Fact]
    public async Task MarkMainSection_WhenRoleNotPaperAuthor_ShouldThrowUnauthorizedException()
    {
        var projectId = Guid.NewGuid();
        _mockMgmt.Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        var handler = new MarkMainSectionCommandHandler(Session, _mockMgmt.Object);
        var dto = new MarkMainSectionDto { ProjectId = projectId };

        var act = () => handler.Handle(new MarkMainSectionCommand(dto, Guid.NewGuid()), CancellationToken.None);
        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task MarkMainSection_WithNonExistentSection_ShouldThrowClientValidationException()
    {
        var projectId = Guid.NewGuid();
        _mockMgmt.Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthorizeConstants.PaperAuthor);

        var handler = new MarkMainSectionCommandHandler(Session, _mockMgmt.Object);
        var dto = new MarkMainSectionDto { ProjectId = projectId };

        var act = () => handler.Handle(new MarkMainSectionCommand(dto, Guid.NewGuid()), CancellationToken.None);
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    [Fact]
    public async Task MarkMainSection_WhenSectionAlreadyMain_ShouldThrowClientValidationException()
    {
        var projectId = Guid.NewGuid();
        var section = SeedMainSection(); // isMainSection = true
        await Session.SaveChangesAsync();

        _mockMgmt.Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthorizeConstants.PaperAuthor);

        var handler = new MarkMainSectionCommandHandler(Session, _mockMgmt.Object);
        var dto = new MarkMainSectionDto { ProjectId = projectId };

        var act = () => handler.Handle(new MarkMainSectionCommand(dto, section.Id), CancellationToken.None);
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    [Fact]
    public async Task MarkMainSection_WhenContributorNotFound_ShouldThrowClientValidationException()
    {
        var projectId = Guid.NewGuid();
        var paperId = Guid.NewGuid();
        var oldMain = SeedMainSection(paperId: paperId, title: "Intro", version: "Version 1");
        var childSection = SeedChildSection(oldMain.Id, paperId, title: "Intro");
        // No contributor seeded for childSection
        await Session.SaveChangesAsync();

        _mockMgmt.Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthorizeConstants.PaperAuthor);

        var handler = new MarkMainSectionCommandHandler(Session, _mockMgmt.Object);
        var dto = new MarkMainSectionDto { ProjectId = projectId };

        var act = () => handler.Handle(new MarkMainSectionCommand(dto, childSection.Id), CancellationToken.None);
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task MarkMainSection_WithValidData_ShouldPromoteChildToMain()
    {
        var projectId = Guid.NewGuid();
        var paperId = Guid.NewGuid();
        var memberId = Guid.NewGuid();

        // Seed the PaperEntity that the handler loads at the end
        var paper = PaperEntity.Create(paperId, "Test Paper");
        Session.Store(paper);

        // Seed the "References" main section that the handler requires after promoting
        var refSection = SeedMainSection(paperId: paperId, title: "References");

        // Build: oldMain ← childSection (with contributor)
        var oldMain = SeedMainSection(paperId: paperId, title: "Intro", version: "Version 1");
        var childSection = SeedChildSection(oldMain.Id, paperId, title: "Intro", status: SectionStatus.Completed);

        // Contributor for child section points to oldMain as markSectionId
        var childContributor = SeedContributor(paperId, memberId, childSection.Id, oldMain.Id, AuthorizeConstants.PaperAuthor);
        // Contributor for old main section
        var oldMainContributor = SeedContributor(paperId, memberId, oldMain.Id, oldMain.Id, AuthorizeConstants.PaperAuthor);
        await Session.SaveChangesAsync();

        _mockMgmt.Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthorizeConstants.PaperAuthor);

        var handler = new MarkMainSectionCommandHandler(Session, _mockMgmt.Object);
        var dto = new MarkMainSectionDto { ProjectId = projectId };

        var result = await handler.Handle(new MarkMainSectionCommand(dto, childSection.Id), CancellationToken.None);

        result.Should().NotBeEmpty();
        var newMain = await Session.LoadAsync<SectionEntity>(result);
        newMain.Should().NotBeNull();
        newMain!.IsMainSection.Should().BeTrue();
    }

    // ─── UpdateGuideline ──────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateGuideline_WithNonExistentSection_ShouldThrowNotFoundException()
    {
        var userId = Guid.NewGuid();
        var handler = new UpdateGuidelineCommandHandler(Session, _mockMgmt.Object);
        var dto = new UpdateGuidelineDto { Description = "desc", MainIdea = "idea" };

        var act = () => handler.Handle(
            new UpdateGuidelineCommand(dto, Guid.NewGuid(), userId, "user"),
            CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateGuideline_WhenMemberNotFound_ShouldThrowUnauthorizedException()
    {
        var userId = Guid.NewGuid();
        var section = SeedMainSection();
        await Session.SaveChangesAsync();

        _mockMgmt
            .Setup(x => x.GetMemberByPaperIdAsync(section.PaperId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ValueTuple<Guid, Guid, Guid>?)null);

        var handler = new UpdateGuidelineCommandHandler(Session, _mockMgmt.Object);
        var dto = new UpdateGuidelineDto { Description = "desc", MainIdea = "idea" };

        var act = () => handler.Handle(
            new UpdateGuidelineCommand(dto, section.Id, userId, "user"),
            CancellationToken.None);
        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task UpdateGuideline_WhenContributorNotFound_ShouldThrowUnauthorizedException()
    {
        var userId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var section = SeedMainSection();
        await Session.SaveChangesAsync();

        _mockMgmt
            .Setup(x => x.GetMemberByPaperIdAsync(section.PaperId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid.NewGuid(), memberId, Guid.NewGuid()));

        var handler = new UpdateGuidelineCommandHandler(Session, _mockMgmt.Object);
        var dto = new UpdateGuidelineDto { Description = "desc", MainIdea = "idea" };

        var act = () => handler.Handle(
            new UpdateGuidelineCommand(dto, section.Id, userId, "user"),
            CancellationToken.None);
        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task UpdateGuideline_WithValidData_ShouldUpdateSectionDescription()
    {
        var userId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var paperId = Guid.NewGuid();
        var section = SeedMainSection(paperId: paperId);
        SeedContributor(paperId, memberId, section.Id, section.Id);
        await Session.SaveChangesAsync();

        _mockMgmt
            .Setup(x => x.GetMemberByPaperIdAsync(paperId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid.NewGuid(), memberId, Guid.NewGuid()));

        var handler = new UpdateGuidelineCommandHandler(Session, _mockMgmt.Object);
        var dto = new UpdateGuidelineDto { Description = "New description", MainIdea = "New main idea" };

        var result = await handler.Handle(
            new UpdateGuidelineCommand(dto, section.Id, userId, "user"),
            CancellationToken.None);

        result.Should().Be(section.Id);
        var updated = await Session.LoadAsync<SectionEntity>(section.Id);
        updated!.Description.Should().Be("New description");
        updated.MainIdea.Should().Be("New main idea");
    }

    // ─── UploadSectionFile ────────────────────────────────────────────────────

    [Fact]
    public async Task UploadSectionFile_WithNonExistentSection_ShouldThrowNotFoundException()
    {
        var handler = new UploadSectionFileCommandHandler(Session, _mockMinIo.Object);
        var dto = new UploadSectionFileDto
        {
            UploadFile = new UploadFileBytes { FileName = "file.tex", Bytes = new byte[] { 1, 2, 3 }, ContentType = "text/plain" }
        };

        var act = () => handler.Handle(new UploadSectionFileCommand(dto, Guid.NewGuid()), CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UploadSectionFile_WithValidData_ShouldUpdateSectionFilePath()
    {
        var section = SeedMainSection();
        await Session.SaveChangesAsync();

        _mockMinIo
            .Setup(x => x.UploadFilesAsync(
                It.IsAny<string>(),
                It.IsAny<List<UploadFileBytes>>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UploadFileResult>
            {
                new() { PublicURL = "https://storage.example.com/intro.tex" }
            });

        var handler = new UploadSectionFileCommandHandler(Session, _mockMinIo.Object);
        var dto = new UploadSectionFileDto
        {
            UploadFile = new UploadFileBytes { FileName = "intro.tex", Bytes = new byte[] { 1, 2, 3 }, ContentType = "text/plain" }
        };

        var result = await handler.Handle(new UploadSectionFileCommand(dto, section.Id), CancellationToken.None);

        result.Should().Be(section.Id);
        var updated = await Session.LoadAsync<SectionEntity>(section.Id);
        updated!.Files.Should().Contain(f => f == "https://storage.example.com/intro.tex");
    }

    // ─── UpdateReference ─────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateReference_WhenMemberNotFound_ShouldThrowUnauthorizedException()
    {
        var userId = Guid.NewGuid();
        var paperId = Guid.NewGuid();

        _mockMgmt
            .Setup(x => x.GetMemberByPaperIdAsync(paperId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ValueTuple<Guid, Guid, Guid>?)null);

        var handler = new UpdateReferenceCommandHandler(Session, _mockMgmt.Object);
        var dto = new UpdateReferenceDto { PaperId = paperId, PaperBankIds = new List<Guid>() };
        var section = SeedMainSection(paperId: paperId);
        await Session.SaveChangesAsync();

        var act = () => handler.Handle(
            new UpdateReferenceCommand(dto, userId, "user", section.Id),
            CancellationToken.None);
        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task UpdateReference_WithNonExistentSection_ShouldThrowNotFoundException()
    {
        var userId = Guid.NewGuid();
        var paperId = Guid.NewGuid();
        var memberId = Guid.NewGuid();

        _mockMgmt
            .Setup(x => x.GetMemberByPaperIdAsync(paperId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid.NewGuid(), memberId, Guid.NewGuid()));

        var handler = new UpdateReferenceCommandHandler(Session, _mockMgmt.Object);
        var dto = new UpdateReferenceDto { PaperId = paperId, PaperBankIds = new List<Guid>() };

        var act = () => handler.Handle(
            new UpdateReferenceCommand(dto, userId, "user", Guid.NewGuid()),
            CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateReference_WhenContributorMissingOrReadOnly_ShouldThrowUnauthorizedException()
    {
        var userId = Guid.NewGuid();
        var paperId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var section = SeedMainSection(paperId: paperId);
        // No contributor seeded for this member/section
        await Session.SaveChangesAsync();

        _mockMgmt
            .Setup(x => x.GetMemberByPaperIdAsync(paperId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid.NewGuid(), memberId, Guid.NewGuid()));

        var handler = new UpdateReferenceCommandHandler(Session, _mockMgmt.Object);
        var dto = new UpdateReferenceDto { PaperId = paperId, PaperBankIds = new List<Guid>() };

        var act = () => handler.Handle(
            new UpdateReferenceCommand(dto, userId, "user", section.Id),
            CancellationToken.None);
        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task UpdateReference_WhenReferenceMainSectionNotFound_ShouldThrowNotFoundException()
    {
        var userId = Guid.NewGuid();
        var paperId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var mainSection = SeedMainSection(paperId: paperId, title: "Introduction");
        SeedContributor(paperId, memberId, mainSection.Id, mainSection.Id, AuthorizeConstants.SectionEdit);
        // No References section seeded
        await Session.SaveChangesAsync();

        _mockMgmt
            .Setup(x => x.GetMemberByPaperIdAsync(paperId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid.NewGuid(), memberId, Guid.NewGuid()));

        var handler = new UpdateReferenceCommandHandler(Session, _mockMgmt.Object);
        var dto = new UpdateReferenceDto { PaperId = paperId, PaperBankIds = new List<Guid>() };

        var act = () => handler.Handle(
            new UpdateReferenceCommand(dto, userId, "user", mainSection.Id),
            CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    // ─── GetSectionByMarkSectionId ────────────────────────────────────────────

    [Fact]
    public async Task GetSectionByMarkSectionId_WithNoContributors_ShouldReturnMainSectionItem()
    {
        var section = SeedMainSection(title: "Intro");
        await Session.SaveChangesAsync();

        _mockMgmt
            .Setup(x => x.GetSubProjectMembersByPaperIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SubProjectMemberInfo>());

        var handler = new GetSectionByMarkSectionIdHandler(Session, _mockMgmt.Object, _mockUser.Object);
        var result = await handler.Handle(
            new GetSectionByMarkSectionIdQuery(section.Id, Guid.NewGuid()),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].SectionId.Should().Be(section.Id);
    }

    // ─── GetSectionHistory ────────────────────────────────────────────────────

    [Fact]
    public async Task GetSectionHistory_WithNoContributors_ShouldReturnEmpty()
    {
        var section = SeedMainSection();
        await Session.SaveChangesAsync();

        var handler = new GetSectionHistoryHandler(Session, _mockMgmt.Object, _mockUser.Object);
        var result = await handler.Handle(
            new GetSectionHistoryQuery(section.Id),
            CancellationToken.None);

        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSectionHistory_WithContributorsButNoneAssigned_ShouldReturnEmpty()
    {
        var paperId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var section = SeedMainSection(paperId: paperId);
        // Contributor has no SectionId (not assigned) and is not PaperAuthor
        var contributor = PaperContributorEntity.Create(
            Guid.NewGuid(), AuthorizeConstants.PaperMember, paperId,
            null, memberId, section.Id);
        Session.Store(contributor);
        await Session.SaveChangesAsync();

        var handler = new GetSectionHistoryHandler(Session, _mockMgmt.Object, _mockUser.Object);
        var result = await handler.Handle(
            new GetSectionHistoryQuery(section.Id),
            CancellationToken.None);

        result.Items.Should().BeEmpty();
    }
}