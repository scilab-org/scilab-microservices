using Common.Constants;
using Common.Models;
using Lab.Application.Dtos.Papers;
using Lab.Application.Features.Paper.Commands.CreatePaperVersionFile;
using Lab.Application.Features.Paper.Commands.TransitionPaperStatus;
using Lab.Application.Features.Paper.Commands.UpdateCombinePaper;
using Lab.Application.Features.Paper.Commands.UpdatePaper;
using Lab.Application.Features.Paper.Queries.GetPaperById;
using Lab.Application.Tests.Common;

namespace Lab.Application.Tests.Features.Paper;

public class PaperCommandsIntegrationTests : MartenTestBase
{
    protected override string SchemaName => "paper_commands_tests";

    private readonly Mock<IManagementApiService> _mockMgmt = new();
    private readonly Mock<IMinIoCloudService> _mockMinIo = new();

    // ─── Helpers ──────────────────────────────────────────────────────────────

    private (PaperEntity paper, ConferenceJournalEntity journal) SeedPaperWithJournal(string title = "Test Paper")
    {
        var journal = ConferenceJournalEntity.Create(Guid.NewGuid(), "Test Journal",
            null, null, null, null, Lab.Domain.Enums.ConferenceJournalType.Journal,
            new List<Guid>(), null, null);
        var paper = PaperEntity.Create(Guid.NewGuid(), title, conferenceJournalId: journal.Id);
        Session.Store(journal);
        Session.Store(paper);
        return (paper, journal);
    }

    // ─── UpdatePaper ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdatePaper_WithNonExistentId_ShouldThrowNotFoundException()
    {
        var handler = new UpdatePaperCommandHandler(Session);
        var act = () => handler.Handle(
            new UpdatePaperCommand(new UpdatePaperDto { Context = "ctx" }, Guid.NewGuid(), "user"),
            CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdatePaper_WithMissingGapTypes_ShouldThrowNotFoundException()
    {
        var (paper, _) = SeedPaperWithJournal();
        await Session.SaveChangesAsync();

        var missingGapTypeId = Guid.NewGuid();
        var handler = new UpdatePaperCommandHandler(Session);
        var act = () => handler.Handle(
            new UpdatePaperCommand(
                new UpdatePaperDto
                {
                    Context = "ctx",
                    GapTypeIds = new List<Guid> { missingGapTypeId }
                },
                paper.Id, "user"),
            CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdatePaper_WithMissingJournal_ShouldThrow()
    {
        // Paper has a ConferenceJournalId that doesn't exist in DB
        var paper = PaperEntity.Create(Guid.NewGuid(), "Paper", conferenceJournalId: Guid.NewGuid());
        Session.Store(paper);
        await Session.SaveChangesAsync();

        var handler = new UpdatePaperCommandHandler(Session);
        var act = () => handler.Handle(
            new UpdatePaperCommand(new UpdatePaperDto { Context = "ctx" }, paper.Id, "user"),
            CancellationToken.None);
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task UpdatePaper_WithValidData_ShouldUpdateAndReturn()
    {
        var (paper, _) = SeedPaperWithJournal();
        await Session.SaveChangesAsync();

        var handler = new UpdatePaperCommandHandler(Session);
        var result = await handler.Handle(
            new UpdatePaperCommand(
                new UpdatePaperDto
                {
                    Context = "Updated context",
                    Abstract = "Abstract text",
                    ResearchGap = "Some gap",
                    GapTypeIds = new List<Guid>()
                },
                paper.Id, "editor"),
            CancellationToken.None);

        result.Should().Be(paper.Id);
        var updated = await Session.LoadAsync<PaperEntity>(paper.Id);
        updated!.Context.Should().Be("Updated context");
    }

    [Fact]
    public async Task UpdatePaper_WithGapTypes_ShouldUpdateGapTypeIds()
    {
        var (paper, _) = SeedPaperWithJournal();
        var gapType = new GapTypeEntity { Id = Guid.NewGuid(), Name = "Empirical" };
        Session.Store(gapType);
        await Session.SaveChangesAsync();

        var handler = new UpdatePaperCommandHandler(Session);
        await handler.Handle(
            new UpdatePaperCommand(
                new UpdatePaperDto
                {
                    Context = "ctx",
                    GapTypeIds = new List<Guid> { gapType.Id }
                },
                paper.Id, "user"),
            CancellationToken.None);

        var updated = await Session.LoadAsync<PaperEntity>(paper.Id);
        updated!.GapTypeIds.Should().Contain(gapType.Id);
    }

    // ─── TransitionPaperStatus ────────────────────────────────────────────────

    [Fact]
    public async Task TransitionPaperStatus_WithNonExistentPaper_ShouldThrowNotFoundException()
    {
        var handler = new TransitionPaperStatusCommandHandler(Session, _mockMgmt.Object);
        var dto = new TransitionPaperStatusDto
        {
            ProjectId = Guid.NewGuid(),
            TargetStatus = SubmissionStatus.Submitted
        };

        var act = () => handler.Handle(
            new TransitionPaperStatusCommand(Guid.NewGuid(), dto, Guid.NewGuid(), "user"),
            CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task TransitionPaperStatus_DuplicateStatus_ShouldThrowClientValidationException()
    {
        var paper = PaperEntity.Create(Guid.NewGuid(), "P");
        Session.Store(paper);
        Session.Store(new PaperStatusHistoryEntity
        {
            Id = Guid.NewGuid(),
            PaperId = paper.Id,
            Status = SubmissionStatus.Draft,
            CreatedOnUtc = DateTimeOffset.UtcNow
        });
        await Session.SaveChangesAsync();

        var handler = new TransitionPaperStatusCommandHandler(Session, _mockMgmt.Object);
        var dto = new TransitionPaperStatusDto
        {
            ProjectId = Guid.NewGuid(),
            TargetStatus = SubmissionStatus.Draft
        };

        var act = () => handler.Handle(
            new TransitionPaperStatusCommand(paper.Id, dto, Guid.NewGuid(), "user"),
            CancellationToken.None);
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    [Fact]
    public async Task TransitionPaperStatus_InvalidTransition_ShouldThrowClientValidationException()
    {
        // Draft → Published is not allowed
        var paper = PaperEntity.Create(Guid.NewGuid(), "P");
        Session.Store(paper);
        await Session.SaveChangesAsync();

        var handler = new TransitionPaperStatusCommandHandler(Session, _mockMgmt.Object);
        var dto = new TransitionPaperStatusDto
        {
            ProjectId = Guid.NewGuid(),
            TargetStatus = SubmissionStatus.Published
        };

        var act = () => handler.Handle(
            new TransitionPaperStatusCommand(paper.Id, dto, Guid.NewGuid(), "user"),
            CancellationToken.None);
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    [Fact]
    public async Task TransitionPaperStatus_RequiresPdfButNoneProvided_ShouldThrowClientValidationException()
    {
        var paper = PaperEntity.Create(Guid.NewGuid(), "P");
        Session.Store(paper);
        await Session.SaveChangesAsync();

        var handler = new TransitionPaperStatusCommandHandler(Session, _mockMgmt.Object);
        var dto = new TransitionPaperStatusDto
        {
            ProjectId = Guid.NewGuid(),
            TargetStatus = SubmissionStatus.Submitted,
            PdfFileId = null
        };

        var act = () => handler.Handle(
            new TransitionPaperStatusCommand(paper.Id, dto, Guid.NewGuid(), "user"),
            CancellationToken.None);
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    [Fact]
    public async Task TransitionPaperStatus_RequiresPdfButFileNotFound_ShouldThrowNotFoundException()
    {
        var paper = PaperEntity.Create(Guid.NewGuid(), "P");
        Session.Store(paper);
        await Session.SaveChangesAsync();

        var handler = new TransitionPaperStatusCommandHandler(Session, _mockMgmt.Object);
        var dto = new TransitionPaperStatusDto
        {
            ProjectId = Guid.NewGuid(),
            TargetStatus = SubmissionStatus.Submitted,
            PdfFileId = Guid.NewGuid()
        };

        var act = () => handler.Handle(
            new TransitionPaperStatusCommand(paper.Id, dto, Guid.NewGuid(), "user"),
            CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task TransitionPaperStatus_PdfBelongsToDifferentPaper_ShouldThrowClientValidationException()
    {
        var paper = PaperEntity.Create(Guid.NewGuid(), "P");
        Session.Store(paper);
        var version = new PaperVersionEntity
        {
            Id = Guid.NewGuid(),
            PaperId = Guid.NewGuid(), // Different paper
            Name = "v1",
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow
        };
        Session.Store(version);
        var pdfFile = new PaperVersionFileEntity
        {
            Id = Guid.NewGuid(),
            PaperVersionId = version.Id,
            FileName = "doc.pdf",
            FileUrl = "https://example.com/doc.pdf",
            CreatedOnUtc = DateTimeOffset.UtcNow
        };
        Session.Store(pdfFile);
        await Session.SaveChangesAsync();

        var handler = new TransitionPaperStatusCommandHandler(Session, _mockMgmt.Object);
        var dto = new TransitionPaperStatusDto
        {
            ProjectId = Guid.NewGuid(),
            TargetStatus = SubmissionStatus.Submitted,
            PdfFileId = pdfFile.Id
        };

        var act = () => handler.Handle(
            new TransitionPaperStatusCommand(paper.Id, dto, Guid.NewGuid(), "user"),
            CancellationToken.None);
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    [Fact]
    public async Task TransitionPaperStatus_SubmittedWithValidPdf_AsAuthor_ShouldStoreHistory()
    {
        var paper = PaperEntity.Create(Guid.NewGuid(), "P");
        Session.Store(paper);
        var version = new PaperVersionEntity
        {
            Id = Guid.NewGuid(), PaperId = paper.Id, Name = "v1",
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow
        };
        Session.Store(version);
        var pdfFile = new PaperVersionFileEntity
        {
            Id = Guid.NewGuid(), PaperVersionId = version.Id,
            FileName = "paper.pdf", FileUrl = "https://example.com/paper.pdf",
            CreatedOnUtc = DateTimeOffset.UtcNow
        };
        Session.Store(pdfFile);
        await Session.SaveChangesAsync();

        var userId = Guid.NewGuid();
        _mockMgmt
            .Setup(x => x.GetSubProjectMembersByPaperIdAsync(paper.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SubProjectMemberInfo>
            {
                new(Guid.NewGuid(), userId, AuthorizeConstants.PaperAuthor)
            });

        var handler = new TransitionPaperStatusCommandHandler(Session, _mockMgmt.Object);
        var dto = new TransitionPaperStatusDto
        {
            ProjectId = Guid.NewGuid(),
            TargetStatus = SubmissionStatus.Submitted,
            PdfFileId = pdfFile.Id
        };

        var result = await handler.Handle(
            new TransitionPaperStatusCommand(paper.Id, dto, userId, "author_user"),
            CancellationToken.None);

        result.Should().NotBeEmpty();
        var history = await Session.Query<PaperStatusHistoryEntity>()
            .Where(h => h.PaperId == paper.Id)
            .ToListAsync();
        history.Should().HaveCount(1);
        history[0].Status.Should().Be(SubmissionStatus.Submitted);
    }

    [Fact]
    public async Task TransitionPaperStatus_SubmittedButUserNotAuthor_ShouldThrowNoPermissionException()
    {
        var paper = PaperEntity.Create(Guid.NewGuid(), "P");
        Session.Store(paper);
        var version = new PaperVersionEntity
        {
            Id = Guid.NewGuid(), PaperId = paper.Id, Name = "v1",
            CreatedOnUtc = DateTimeOffset.UtcNow, LastModifiedOnUtc = DateTimeOffset.UtcNow
        };
        Session.Store(version);
        var pdfFile = new PaperVersionFileEntity
        {
            Id = Guid.NewGuid(), PaperVersionId = version.Id,
            FileName = "doc.pdf", FileUrl = "https://example.com/doc.pdf",
            CreatedOnUtc = DateTimeOffset.UtcNow
        };
        Session.Store(pdfFile);
        await Session.SaveChangesAsync();

        var userId = Guid.NewGuid();
        _mockMgmt
            .Setup(x => x.GetSubProjectMembersByPaperIdAsync(paper.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SubProjectMemberInfo>
            {
                new(Guid.NewGuid(), Guid.NewGuid(), AuthorizeConstants.PaperAuthor) // different userId
            });

        var handler = new TransitionPaperStatusCommandHandler(Session, _mockMgmt.Object);
        var dto = new TransitionPaperStatusDto
        {
            ProjectId = Guid.NewGuid(),
            TargetStatus = SubmissionStatus.Submitted,
            PdfFileId = pdfFile.Id
        };

        var act = () => handler.Handle(
            new TransitionPaperStatusCommand(paper.Id, dto, userId, "user"),
            CancellationToken.None);
        await act.Should().ThrowAsync<NoPermissionException>();
    }

    [Fact]
    public async Task TransitionPaperStatus_RevisionRequired_AsProjectAuthor_ShouldStoreHistory()
    {
        // Draft → Submitted → RevisionRequired
        var paper = PaperEntity.Create(Guid.NewGuid(), "P");
        Session.Store(paper);
        Session.Store(new PaperStatusHistoryEntity
        {
            Id = Guid.NewGuid(), PaperId = paper.Id,
            Status = SubmissionStatus.Submitted,
            CreatedOnUtc = DateTimeOffset.UtcNow
        });
        await Session.SaveChangesAsync();

        var projectId = Guid.NewGuid();
        _mockMgmt
            .Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthorizeConstants.ProjectAuthor);

        var handler = new TransitionPaperStatusCommandHandler(Session, _mockMgmt.Object);
        var dto = new TransitionPaperStatusDto
        {
            ProjectId = projectId,
            TargetStatus = SubmissionStatus.RevisionRequired
        };

        var result = await handler.Handle(
            new TransitionPaperStatusCommand(paper.Id, dto, Guid.NewGuid(), "editor"),
            CancellationToken.None);

        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task TransitionPaperStatus_EditorTransition_NotProjectRole_ShouldThrowNoPermissionException()
    {
        var paper = PaperEntity.Create(Guid.NewGuid(), "P");
        Session.Store(paper);
        Session.Store(new PaperStatusHistoryEntity
        {
            Id = Guid.NewGuid(), PaperId = paper.Id,
            Status = SubmissionStatus.Submitted,
            CreatedOnUtc = DateTimeOffset.UtcNow
        });
        await Session.SaveChangesAsync();

        var projectId = Guid.NewGuid();
        _mockMgmt
            .Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        var handler = new TransitionPaperStatusCommandHandler(Session, _mockMgmt.Object);
        var dto = new TransitionPaperStatusDto
        {
            ProjectId = projectId,
            TargetStatus = SubmissionStatus.RevisionRequired
        };

        var act = () => handler.Handle(
            new TransitionPaperStatusCommand(paper.Id, dto, Guid.NewGuid(), "user"),
            CancellationToken.None);
        await act.Should().ThrowAsync<NoPermissionException>();
    }

    // ─── UpdateCombinePaper ───────────────────────────────────────────────────

    [Fact]
    public async Task UpdateCombinePaper_WhenNotPaperAuthor_ShouldThrowUnauthorizedException()
    {
        var projectId = Guid.NewGuid();
        _mockMgmt
            .Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        var handler = new UpdateCombinePaperCommandHandler(Session, _mockMgmt.Object);
        var dto = new UpdateCombinePaperDto { Content = "content", ProjectId = projectId };

        var act = () => handler.Handle(
            new UpdateCombinePaperCommand(Guid.NewGuid(), Guid.NewGuid(), "user", dto),
            CancellationToken.None);
        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task UpdateCombinePaper_WithNonExistentPaper_ShouldThrowNotFoundException()
    {
        var projectId = Guid.NewGuid();
        _mockMgmt
            .Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthorizeConstants.PaperAuthor);

        var handler = new UpdateCombinePaperCommandHandler(Session, _mockMgmt.Object);
        var dto = new UpdateCombinePaperDto { Content = "content", ProjectId = projectId };

        var act = () => handler.Handle(
            new UpdateCombinePaperCommand(Guid.NewGuid(), Guid.NewGuid(), "user", dto),
            CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateCombinePaper_WithNonExistentVersion_ShouldThrowNotFoundException()
    {
        var paper = PaperEntity.Create(Guid.NewGuid(), "P");
        Session.Store(paper);
        await Session.SaveChangesAsync();

        var projectId = Guid.NewGuid();
        _mockMgmt
            .Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthorizeConstants.PaperAuthor);

        var handler = new UpdateCombinePaperCommandHandler(Session, _mockMgmt.Object);
        var dto = new UpdateCombinePaperDto { Content = "content", ProjectId = projectId };

        var act = () => handler.Handle(
            new UpdateCombinePaperCommand(paper.Id, Guid.NewGuid(), "user", dto),
            CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateCombinePaper_WithValidData_ShouldUpdateContentAndReturn()
    {
        var paper = PaperEntity.Create(Guid.NewGuid(), "P");
        Session.Store(paper);
        var version = new PaperVersionEntity
        {
            Id = Guid.NewGuid(), PaperId = paper.Id, Name = "v1",
            Content = "old content",
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow
        };
        Session.Store(version);
        await Session.SaveChangesAsync();

        var projectId = Guid.NewGuid();
        _mockMgmt
            .Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthorizeConstants.PaperAuthor);

        var handler = new UpdateCombinePaperCommandHandler(Session, _mockMgmt.Object);
        var dto = new UpdateCombinePaperDto { Content = "new combined content", ProjectId = projectId };

        var result = await handler.Handle(
            new UpdateCombinePaperCommand(paper.Id, version.Id, "author", dto),
            CancellationToken.None);

        result.Should().Be(version.Id);
        var updated = await Session.LoadAsync<PaperVersionEntity>(version.Id);
        updated!.Content.Should().Be("new combined content");
    }

    // ─── GetPaperById ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetPaperById_WithNonExistentId_ShouldThrowNotFoundException()
    {
        var handler = new GetPaperByIdQueryHandler(Session, Mapper, _mockMgmt.Object);
        var act = () => handler.Handle(new GetPaperByIdQuery(Guid.NewGuid()), CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetPaperById_WithExistingPaper_ShouldReturnMappedResult()
    {
        var paper = PaperEntity.Create(Guid.NewGuid(), "My Paper");
        Session.Store(paper);
        await Session.SaveChangesAsync();

        _mockMgmt
            .Setup(x => x.GetSubProjectMembersByPaperIdAsync(paper.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SubProjectMemberInfo>());

        var handler = new GetPaperByIdQueryHandler(Session, Mapper, _mockMgmt.Object);
        var result = await handler.Handle(new GetPaperByIdQuery(paper.Id), CancellationToken.None);

        result.Paper.Should().NotBeNull();
        result.Paper.Title.Should().Be("My Paper");
    }

    [Fact]
    public async Task GetPaperById_WithVersions_ShouldReturnVersionsInResult()
    {
        var paper = PaperEntity.Create(Guid.NewGuid(), "Paper With Versions");
        Session.Store(paper);
        var version = new PaperVersionEntity
        {
            Id = Guid.NewGuid(), PaperId = paper.Id, Name = "v1",
            Content = "content",
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow
        };
        Session.Store(version);
        await Session.SaveChangesAsync();

        _mockMgmt
            .Setup(x => x.GetSubProjectMembersByPaperIdAsync(paper.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SubProjectMemberInfo>());

        var handler = new GetPaperByIdQueryHandler(Session, Mapper, _mockMgmt.Object);
        var result = await handler.Handle(new GetPaperByIdQuery(paper.Id), CancellationToken.None);

        result.Paper.Versions.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetPaperById_WithMembers_ShouldResolveSubProjectId()
    {
        var paper = PaperEntity.Create(Guid.NewGuid(), "Paper");
        Session.Store(paper);
        await Session.SaveChangesAsync();

        var memberUserId = Guid.NewGuid();
        var subProjectId = Guid.NewGuid();

        _mockMgmt
            .Setup(x => x.GetSubProjectMembersByPaperIdAsync(paper.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SubProjectMemberInfo>
            {
                new(Guid.NewGuid(), memberUserId, AuthorizeConstants.PaperAuthor)
            });
        _mockMgmt
            .Setup(x => x.GetMemberByPaperIdAsync(paper.Id, memberUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((subProjectId, Guid.NewGuid(), Guid.NewGuid()));

        var handler = new GetPaperByIdQueryHandler(Session, Mapper, _mockMgmt.Object);
        var result = await handler.Handle(new GetPaperByIdQuery(paper.Id), CancellationToken.None);

        result.Paper.SubProjectId.Should().Be(subProjectId);
    }

    // ─── CreatePaperVersionFile ───────────────────────────────────────────────

    [Fact]
    public async Task CreatePaperVersionFile_WithNonExistentPaper_ShouldThrowNotFoundException()
    {
        var handler = new CreatePaperVersionFileCommandHandler(Session, _mockMinIo.Object);
        var dto = new CreatePaperVersionFileDto
        {
            UploadFile = new UploadFileBytes { FileName = "doc.pdf", Bytes = new byte[] { 1, 2, 3 }, ContentType = "application/pdf" }
        };

        var act = () => handler.Handle(
            new CreatePaperVersionFileCommand(Guid.NewGuid(), Guid.NewGuid(), dto, "user"),
            CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreatePaperVersionFile_WithNonExistentVersion_ShouldThrowNotFoundException()
    {
        var paper = PaperEntity.Create(Guid.NewGuid(), "P");
        Session.Store(paper);
        await Session.SaveChangesAsync();

        var handler = new CreatePaperVersionFileCommandHandler(Session, _mockMinIo.Object);
        var dto = new CreatePaperVersionFileDto
        {
            UploadFile = new UploadFileBytes { FileName = "doc.pdf", Bytes = new byte[] { 1, 2, 3 }, ContentType = "application/pdf" }
        };

        var act = () => handler.Handle(
            new CreatePaperVersionFileCommand(paper.Id, Guid.NewGuid(), dto, "user"),
            CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreatePaperVersionFile_VersionBelongsToDifferentPaper_ShouldThrowClientValidationException()
    {
        var paper = PaperEntity.Create(Guid.NewGuid(), "P");
        Session.Store(paper);
        var version = new PaperVersionEntity
        {
            Id = Guid.NewGuid(),
            PaperId = Guid.NewGuid(), // Different paper
            Name = "v1",
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow
        };
        Session.Store(version);
        await Session.SaveChangesAsync();

        var handler = new CreatePaperVersionFileCommandHandler(Session, _mockMinIo.Object);
        var dto = new CreatePaperVersionFileDto
        {
            UploadFile = new UploadFileBytes { FileName = "doc.pdf", Bytes = new byte[] { 1, 2, 3 }, ContentType = "application/pdf" }
        };

        var act = () => handler.Handle(
            new CreatePaperVersionFileCommand(paper.Id, version.Id, dto, "user"),
            CancellationToken.None);
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    [Fact]
    public async Task CreatePaperVersionFile_WithValidData_ShouldStoreAndReturnId()
    {
        var paper = PaperEntity.Create(Guid.NewGuid(), "P");
        Session.Store(paper);
        var version = new PaperVersionEntity
        {
            Id = Guid.NewGuid(), PaperId = paper.Id, Name = "v1",
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow
        };
        Session.Store(version);
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
                new() { PublicURL = "https://storage.example.com/paper.pdf" }
            });

        var handler = new CreatePaperVersionFileCommandHandler(Session, _mockMinIo.Object);
        var dto = new CreatePaperVersionFileDto
        {
            UploadFile = new UploadFileBytes { FileName = "paper.pdf", Bytes = new byte[] { 1, 2, 3 }, ContentType = "application/pdf" },
            Note = "Final version"
        };

        var result = await handler.Handle(
            new CreatePaperVersionFileCommand(paper.Id, version.Id, dto, "user"),
            CancellationToken.None);

        result.Should().NotBeEmpty();
        var stored = await Session.LoadAsync<PaperVersionFileEntity>(result);
        stored.Should().NotBeNull();
        stored!.FileUrl.Should().Be("https://storage.example.com/paper.pdf");
    }

    // ─── UpdatePaper with sections ────────────────────────────────────────────

    [Fact]
    public async Task UpdatePaper_WithSectionsInDb_ShouldUpdateSectionsPaperRule()
    {
        var (paper, journal) = SeedPaperWithJournal("Paper With Sections");
        var section = SectionEntity.Create(
            Guid.NewGuid(), "content", paper.Id, 1, Lab.Domain.Enums.SectionStatus.NotStarted,
            isMainSection: true, title: "Introduction", description: "Intro desc");
        Session.Store(section);
        await Session.SaveChangesAsync();

        var handler = new UpdatePaperCommandHandler(Session);
        var result = await handler.Handle(
            new UpdatePaperCommand(new UpdatePaperDto { Context = "new context" }, paper.Id, "user"),
            CancellationToken.None);

        result.Should().Be(paper.Id);
        var updatedSection = await Session.LoadAsync<SectionEntity>(section.Id);
        updatedSection!.PaperRule.Should().NotBeNull();
    }
}
