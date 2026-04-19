using Management.Application.Features.Project.Commands;
using Management.Application.Tests.Common;
using MediatR;

namespace Management.Application.Tests.Integration;

#region CreateProjectCommand

[Collection("MartenIntegration")]
public class CreateProjectCommandHandlerIntegrationTests
{
    private readonly MartenFixture _fixture;
    public CreateProjectCommandHandlerIntegrationTests(MartenFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Handle_Should_CreateProject_WhenCodeIsUnique()
    {
        await using var session = _fixture.CreateSession();
        var handler = new CreateProjectCommandHandler(session);
        var command = new CreateProjectCommand(new CreateProjectDto
        {
            Name = "Test Project",
            Code = "UNIQUE-001"
        });

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeEmpty();
        var saved = await session.LoadAsync<ProjectEntity>(result);
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("Test Project");
        saved.Code.Should().Be("UNIQUE-001");
    }

    [Fact]
    public async Task Handle_Should_ThrowClientValidation_WhenCodeAlreadyExists()
    {
        await using var session = _fixture.CreateSession();
        var existing = ProjectEntity.Create(Guid.NewGuid(), name: "Existing", code: "DUPE-CODE");
        session.Store(existing);
        await session.SaveChangesAsync();

        var handler = new CreateProjectCommandHandler(session);
        var command = new CreateProjectCommand(new CreateProjectDto
        {
            Name = "New Project",
            Code = "DUPE-CODE"
        });

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    [Fact]
    public async Task Handle_Should_CreateProject_WhenCodeIsNull()
    {
        await using var session = _fixture.CreateSession();
        var handler = new CreateProjectCommandHandler(session);
        var command = new CreateProjectCommand(new CreateProjectDto
        {
            Name = "No Code Project",
            Code = null
        });

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeEmpty();
    }
}

#endregion

#region DeleteProjectCommand

[Collection("MartenIntegration")]
public class DeleteProjectCommandHandlerIntegrationTests
{
    private readonly MartenFixture _fixture;
    public DeleteProjectCommandHandlerIntegrationTests(MartenFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Handle_Should_ThrowNotFound_WhenProjectDoesNotExist()
    {
        await using var session = _fixture.CreateSession();
        var handler = new DeleteProjectCommandHandler(session);
        var command = new DeleteProjectCommand(Guid.NewGuid());

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    [Fact]
    public async Task Handle_Should_ThrowClientValidation_WhenProjectHasSubProjects()
    {
        await using var session = _fixture.CreateSession();
        var parentId = Guid.NewGuid();
        var parent = ProjectEntity.Create(parentId, name: "Parent");
        var sub = ProjectEntity.Create(Guid.NewGuid(), name: "Sub", parentProjectId: parentId);
        session.Store(parent);
        session.Store(sub);
        await session.SaveChangesAsync();

        var handler = new DeleteProjectCommandHandler(session);
        var command = new DeleteProjectCommand(parentId);

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    [Fact]
    public async Task Handle_Should_DeleteProject_WhenNoSubProjects()
    {
        await using var session = _fixture.CreateSession();
        var project = ProjectEntity.Create(Guid.NewGuid(), name: "ToDelete");
        session.Store(project);
        await session.SaveChangesAsync();

        var handler = new DeleteProjectCommandHandler(session);
        var command = new DeleteProjectCommand(project.Id);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().Be(Unit.Value);
        var deleted = await session.LoadAsync<ProjectEntity>(project.Id);
        deleted.Should().BeNull();
    }
}

#endregion

#region UpdateProjectCommand

[Collection("MartenIntegration")]
public class UpdateProjectCommandHandlerIntegrationTests
{
    private readonly MartenFixture _fixture;
    public UpdateProjectCommandHandlerIntegrationTests(MartenFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Handle_Should_ThrowNotFound_WhenProjectDoesNotExist()
    {
        await using var session = _fixture.CreateSession();
        var labMock = new Mock<ILabApiService>();
        var handler = new UpdateProjectCommandHandler(session, labMock.Object);
        var command = new UpdateProjectCommand(Guid.NewGuid(), new UpdateProjectDto { Name = "X" });

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Should_UpdateProject_WithoutRuleSync()
    {
        await using var session = _fixture.CreateSession();
        var project = ProjectEntity.Create(Guid.NewGuid(), name: "Old", context: "ctx", domain: "dom", keypoint: "kp");
        session.Store(project);
        await session.SaveChangesAsync();

        var labMock = new Mock<ILabApiService>();
        var handler = new UpdateProjectCommandHandler(session, labMock.Object);
        var command = new UpdateProjectCommand(project.Id, new UpdateProjectDto
        {
            Name = "Updated",
            Context = "ctx",
            Domain = "dom",
            Keypoint = "kp"
        });

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().Be(project.Id);
        var updated = await session.LoadAsync<ProjectEntity>(project.Id);
        updated!.Name.Should().Be("Updated");
        labMock.Verify(x => x.UpdateProjectRulesAsync(
            It.IsAny<IEnumerable<Guid>>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_UpdateProject_WithRuleSync_WhenDomainChanges()
    {
        await using var session = _fixture.CreateSession();
        var projectId = Guid.NewGuid();
        var project = ProjectEntity.Create(projectId, name: "P", context: "oldCtx", domain: "oldDom", keypoint: "oldKp");
        session.Store(project);

        var paperId = Guid.NewGuid();
        var sub = ProjectEntity.Create(Guid.NewGuid(), name: "Sub", parentProjectId: projectId,
            paperIds: new List<Guid> { paperId });
        session.Store(sub);
        await session.SaveChangesAsync();

        var labMock = new Mock<ILabApiService>();
        labMock.Setup(x => x.UpdateProjectRulesAsync(
                It.IsAny<IEnumerable<Guid>>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new UpdateProjectCommandHandler(session, labMock.Object);
        var command = new UpdateProjectCommand(projectId, new UpdateProjectDto
        {
            Name = "Updated",
            Context = "newCtx",
            Domain = "newDom",
            Keypoint = "newKp"
        });

        await handler.Handle(command, CancellationToken.None);

        labMock.Verify(x => x.UpdateProjectRulesAsync(
            It.Is<IEnumerable<Guid>>(ids => ids.Contains(paperId)),
            "newCtx", "newDom", "newKp",
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_UpdateProject_NoRuleSync_WhenNoSubProject()
    {
        await using var session = _fixture.CreateSession();
        var project = ProjectEntity.Create(Guid.NewGuid(), name: "P", context: "ctx1");
        session.Store(project);
        await session.SaveChangesAsync();

        var labMock = new Mock<ILabApiService>();
        var handler = new UpdateProjectCommandHandler(session, labMock.Object);
        var command = new UpdateProjectCommand(project.Id, new UpdateProjectDto
        {
            Name = "Updated",
            Context = "ctx2" // different context but no sub-project
        });

        await handler.Handle(command, CancellationToken.None);

        labMock.Verify(x => x.UpdateProjectRulesAsync(
            It.IsAny<IEnumerable<Guid>>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }
}

#endregion

#region DeleteProjectConferenceJournalByJournalIdCommand

[Collection("MartenIntegration")]
public class DeleteProjectConferenceJournalByJournalIdCommandHandlerIntegrationTests
{
    private readonly MartenFixture _fixture;
    public DeleteProjectConferenceJournalByJournalIdCommandHandlerIntegrationTests(MartenFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Handle_Should_ReturnEmptyList_WhenNoProjectsContainJournal()
    {
        await using var session = _fixture.CreateSession();
        var handler = new DeleteProjectConferenceJournalByJournalIdCommandHandler(session);
        var command = new DeleteProjectConferenceJournalByJournalIdCommand(Guid.NewGuid());

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_RemoveJournal_FromAffectedProjects()
    {
        await using var session = _fixture.CreateSession();
        var journalId = Guid.NewGuid();
        var project = ProjectEntity.Create(Guid.NewGuid(), name: "P",
            conferenceJournalIds: new List<Guid> { journalId, Guid.NewGuid() });
        session.Store(project);
        await session.SaveChangesAsync();

        var handler = new DeleteProjectConferenceJournalByJournalIdCommandHandler(session);
        var command = new DeleteProjectConferenceJournalByJournalIdCommand(journalId);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().Contain(project.Id);
        var updated = await session.LoadAsync<ProjectEntity>(project.Id);
        updated!.ConferenceJournalIds.Should().NotContain(journalId);
    }
}

#endregion

#region DeleteProjectPaperByBankIdCommand

[Collection("MartenIntegration")]
public class DeleteProjectPaperByBankIdCommandHandlerIntegrationTests
{
    private readonly MartenFixture _fixture;
    public DeleteProjectPaperByBankIdCommandHandlerIntegrationTests(MartenFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Handle_Should_ThrowNotFound_WhenNoPaperInAnyProject()
    {
        await using var session = _fixture.CreateSession();
        var handler = new DeleteProjectPaperByBankIdCommandHandler(session);
        var command = new DeleteProjectPaperByBankIdCommand(Guid.NewGuid());

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Should_RemovePaper_FromProjects()
    {
        await using var session = _fixture.CreateSession();
        var paperId = Guid.NewGuid();
        var project = ProjectEntity.Create(Guid.NewGuid(), name: "P",
            paperIds: new List<Guid> { paperId, Guid.NewGuid() });
        session.Store(project);
        await session.SaveChangesAsync();

        var handler = new DeleteProjectPaperByBankIdCommandHandler(session);
        var command = new DeleteProjectPaperByBankIdCommand(paperId);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().Contain(paperId);
        var updated = await session.LoadAsync<ProjectEntity>(project.Id);
        updated!.PaperIds.Should().NotContain(paperId);
    }
}

#endregion

#region DeleteSubProjectCommand

[Collection("MartenIntegration")]
public class DeleteSubProjectCommandHandlerIntegrationTests
{
    private readonly MartenFixture _fixture;
    public DeleteSubProjectCommandHandlerIntegrationTests(MartenFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Handle_Should_ThrowNotFound_WhenSubProjectDoesNotExist()
    {
        await using var session = _fixture.CreateSession();
        var labMock = new Mock<ILabApiService>();
        var handler = new DeleteSubProjectCommandHandler(session, labMock.Object);
        var command = new DeleteSubProjectCommand(Guid.NewGuid(), Guid.NewGuid(), "user");

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Should_ThrowNoPermission_WhenUserIsNotManagerOrAuthor()
    {
        await using var session = _fixture.CreateSession();
        var parentId = Guid.NewGuid();
        var subId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var parent = ProjectEntity.Create(parentId, name: "Parent");
        var sub = ProjectEntity.Create(subId, name: "Sub", parentProjectId: parentId);
        session.Store(parent);
        session.Store(sub);
        // No member records for this user
        await session.SaveChangesAsync();

        var labMock = new Mock<ILabApiService>();
        var handler = new DeleteSubProjectCommandHandler(session, labMock.Object);
        var command = new DeleteSubProjectCommand(subId, userId, "user");

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NoPermissionException>();
    }

    [Fact]
    public async Task Handle_Should_ThrowNoPermission_WhenAuthorIsNotCreator()
    {
        await using var session = _fixture.CreateSession();
        var parentId = Guid.NewGuid();
        var subId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var parent = ProjectEntity.Create(parentId, name: "Parent");
        var sub = ProjectEntity.Create(subId, name: "Sub", parentProjectId: parentId, createdBy: "otherUser");
        session.Store(parent);
        session.Store(sub);

        var authorMember = MemberEntity.Create(Guid.NewGuid(), userId, subId,
            AuthorizeConstants.PaperAuthor, DateTimeOffset.UtcNow);
        session.Store(authorMember);
        await session.SaveChangesAsync();

        var labMock = new Mock<ILabApiService>();
        var handler = new DeleteSubProjectCommandHandler(session, labMock.Object);
        var command = new DeleteSubProjectCommand(subId, userId, "user");

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NoPermissionException>();
    }

    [Fact]
    public async Task Handle_Should_DeleteSubProject_WhenUserIsManager()
    {
        await using var session = _fixture.CreateSession();
        var parentId = Guid.NewGuid();
        var subId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var paperId = Guid.NewGuid();
        var parent = ProjectEntity.Create(parentId, name: "Parent");
        var sub = ProjectEntity.Create(subId, name: "Sub", parentProjectId: parentId,
            paperIds: new List<Guid> { paperId });
        session.Store(parent);
        session.Store(sub);

        var manager = MemberEntity.Create(Guid.NewGuid(), userId, parentId,
            AuthorizeConstants.ProjectManager, DateTimeOffset.UtcNow);
        session.Store(manager);
        await session.SaveChangesAsync();

        var labMock = new Mock<ILabApiService>();
        labMock.Setup(x => x.DeletePaperAsync(paperId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new DeleteSubProjectCommandHandler(session, labMock.Object);
        var command = new DeleteSubProjectCommand(subId, userId, "user");

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().Be(Unit.Value);
        labMock.Verify(x => x.DeletePaperAsync(paperId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_DeleteSubProject_WhenAuthorIsCreator()
    {
        await using var session = _fixture.CreateSession();
        var parentId = Guid.NewGuid();
        var subId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var parent = ProjectEntity.Create(parentId, name: "Parent");
        var sub = ProjectEntity.Create(subId, name: "Sub", parentProjectId: parentId, createdBy: "creator");
        session.Store(parent);
        session.Store(sub);

        var authorMember = MemberEntity.Create(Guid.NewGuid(), userId, subId,
            AuthorizeConstants.PaperAuthor, DateTimeOffset.UtcNow);
        session.Store(authorMember);
        await session.SaveChangesAsync();

        var labMock = new Mock<ILabApiService>();
        var handler = new DeleteSubProjectCommandHandler(session, labMock.Object);
        var command = new DeleteSubProjectCommand(subId, userId, "creator");

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().Be(Unit.Value);
    }
}

#endregion
