using Common.Constants;
using Lab.Application.Dtos.PaperAuthors;
using Lab.Application.Features.PaperAuthor.Commands.CreatePaperAuthor;
using Lab.Application.Features.PaperAuthor.Commands.UpdatePaperAuthor;
using Lab.Application.Tests.Common;

namespace Lab.Application.Tests.Features.PaperAuthor;

public class PaperAuthorCommandsIntegrationTests : MartenTestBase
{
    protected override string SchemaName => "paper_author_commands_tests";

    private readonly Mock<IManagementApiService> _mockMgmt = new();

    // ─── CreatePaperAuthor ────────────────────────────────────────────────────

    [Fact]
    public async Task CreatePaperAuthor_WhenRoleIsNull_ShouldThrowUnauthorizedException()
    {
        var projectId = Guid.NewGuid();
        _mockMgmt.Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        var handler = new CreatePaperAuthorCommandHandler(Session, _mockMgmt.Object);
        var dto = new CreatePaperAuthorDto
        {
            Name = "John Doe",
            Email = "john@example.com",
            PaperId = Guid.NewGuid(),
            AuthorRoleId = Guid.NewGuid(),
            MemberId = Guid.NewGuid(),
            ProjectId = projectId,
            AffiliationId = Guid.NewGuid(),
            AffiliationName = "MIT"
        };

        var act = () => handler.Handle(new CreatePaperAuthorCommand(dto), CancellationToken.None);
        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task CreatePaperAuthor_WhenRoleIsNotPaperAuthor_ShouldThrowUnauthorizedException()
    {
        var projectId = Guid.NewGuid();
        _mockMgmt.Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthorizeConstants.PaperMember);

        var handler = new CreatePaperAuthorCommandHandler(Session, _mockMgmt.Object);
        var dto = new CreatePaperAuthorDto
        {
            Name = "Jane Smith",
            Email = "jane@example.com",
            PaperId = Guid.NewGuid(),
            AuthorRoleId = Guid.NewGuid(),
            MemberId = Guid.NewGuid(),
            ProjectId = projectId,
            AffiliationId = Guid.NewGuid(),
            AffiliationName = "Harvard"
        };

        var act = () => handler.Handle(new CreatePaperAuthorCommand(dto), CancellationToken.None);
        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task CreatePaperAuthor_WithValidData_ShouldStoreEntityAndReturnId()
    {
        var projectId = Guid.NewGuid();
        _mockMgmt.Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthorizeConstants.PaperAuthor);

        var handler = new CreatePaperAuthorCommandHandler(Session, _mockMgmt.Object);
        var dto = new CreatePaperAuthorDto
        {
            Name = "  Alice Researcher  ",
            OcrId = "ocr-001",
            Email = "  alice@university.edu  ",
            PaperId = Guid.NewGuid(),
            AuthorRoleId = Guid.NewGuid(),
            MemberId = Guid.NewGuid(),
            ProjectId = projectId,
            AffiliationId = Guid.NewGuid(),
            AffiliationName = "Stanford"
        };

        var result = await handler.Handle(new CreatePaperAuthorCommand(dto), CancellationToken.None);

        result.Should().NotBeEmpty();
        var stored = await Session.LoadAsync<PaperAuthorEntity>(result);
        stored.Should().NotBeNull();
        stored!.Name.Should().Be("Alice Researcher"); // trimmed
        stored.Email.Should().Be("alice@university.edu"); // trimmed
        stored.OcrId.Should().Be("ocr-001");
    }

    // ─── UpdatePaperAuthor ────────────────────────────────────────────────────

    [Fact]
    public async Task UpdatePaperAuthor_WhenRoleIsNull_ShouldThrowUnauthorizedException()
    {
        var projectId = Guid.NewGuid();
        _mockMgmt.Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        var handler = new UpdatePaperAuthorCommandHandler(Session, _mockMgmt.Object);
        var dto = new UpdatePaperAuthorDto { ProjectId = projectId };

        var act = () => handler.Handle(new UpdatePaperAuthorCommand(Guid.NewGuid(), dto), CancellationToken.None);
        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task UpdatePaperAuthor_WhenRoleIsNotPaperAuthor_ShouldThrowUnauthorizedException()
    {
        var projectId = Guid.NewGuid();
        _mockMgmt.Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthorizeConstants.ProjectManager);

        var handler = new UpdatePaperAuthorCommandHandler(Session, _mockMgmt.Object);
        var dto = new UpdatePaperAuthorDto { ProjectId = projectId };

        var act = () => handler.Handle(new UpdatePaperAuthorCommand(Guid.NewGuid(), dto), CancellationToken.None);
        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task UpdatePaperAuthor_WithNonExistentId_ShouldThrowNotFoundException()
    {
        var projectId = Guid.NewGuid();
        _mockMgmt.Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthorizeConstants.PaperAuthor);

        var handler = new UpdatePaperAuthorCommandHandler(Session, _mockMgmt.Object);
        var dto = new UpdatePaperAuthorDto { ProjectId = projectId };

        var act = () => handler.Handle(new UpdatePaperAuthorCommand(Guid.NewGuid(), dto), CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdatePaperAuthor_WithValidData_ShouldUpdateFieldsAndReturnUnit()
    {
        var projectId = Guid.NewGuid();
        var existing = PaperAuthorEntity.Create(
            Guid.NewGuid(), "Old Name", null, "old@mail.com",
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), projectId, "Old University");
        Session.Store(existing);
        await Session.SaveChangesAsync();

        _mockMgmt.Setup(x => x.GetMyProjectRoleAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthorizeConstants.PaperAuthor);

        var handler = new UpdatePaperAuthorCommandHandler(Session, _mockMgmt.Object);
        var dto = new UpdatePaperAuthorDto
        {
            ProjectId = projectId,
            Name = "  New Name  ",
            Email = "  new@mail.com  ",
            OcrId = "ocr-002",
            AffiliationName = "New University"
        };

        var result = await handler.Handle(new UpdatePaperAuthorCommand(existing.Id, dto), CancellationToken.None);

        result.Should().Be(Unit.Value);
        var updated = await Session.LoadAsync<PaperAuthorEntity>(existing.Id);
        updated.Should().NotBeNull();
        updated!.Name.Should().Be("New Name");
        updated.Email.Should().Be("new@mail.com");
        updated.OcrId.Should().Be("ocr-002");
    }
}
