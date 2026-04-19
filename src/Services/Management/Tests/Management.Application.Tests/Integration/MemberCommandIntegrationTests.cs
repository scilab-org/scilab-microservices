using Management.Application.Features.Member.Commands;
using Management.Application.Tests.Common;
using MediatR;

namespace Management.Application.Tests.Integration;

#region AddProjectManagersCommand

[Collection("MartenIntegration")]
public class AddProjectManagersCommandHandlerIntegrationTests
{
    private readonly MartenFixture _fixture;
    public AddProjectManagersCommandHandlerIntegrationTests(MartenFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Handle_Should_ThrowNotFound_WhenProjectDoesNotExist()
    {
        await using var session = _fixture.CreateSession();
        var userMock = new Mock<IUserApiService>();
        var handler = new AddProjectManagersCommandHandler(session, userMock.Object);
        var command = new AddProjectManagersCommand(Guid.NewGuid(), new AddProjectManagersDto { UserId = Guid.NewGuid() });

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Should_ThrowClientValidation_WhenProjectAlreadyHasManager()
    {
        await using var session = _fixture.CreateSession();
        var projectId = Guid.NewGuid();
        var project = ProjectEntity.Create(projectId, name: "P");
        session.Store(project);

        var existingManager = MemberEntity.Create(Guid.NewGuid(), Guid.NewGuid(), projectId,
            AuthorizeConstants.ProjectManager, DateTimeOffset.UtcNow);
        session.Store(existingManager);
        await session.SaveChangesAsync();

        var userMock = new Mock<IUserApiService>();
        var handler = new AddProjectManagersCommandHandler(session, userMock.Object);
        var command = new AddProjectManagersCommand(projectId, new AddProjectManagersDto { UserId = Guid.NewGuid() });

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    [Fact]
    public async Task Handle_Should_ThrowNotFound_WhenUserDoesNotExist()
    {
        await using var session = _fixture.CreateSession();
        var projectId = Guid.NewGuid();
        var project = ProjectEntity.Create(projectId, name: "P");
        session.Store(project);
        await session.SaveChangesAsync();

        var userMock = new Mock<IUserApiService>();
        userMock.Setup(x => x.IsUserExistAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new AddProjectManagersCommandHandler(session, userMock.Object);
        var command = new AddProjectManagersCommand(projectId, new AddProjectManagersDto { UserId = Guid.NewGuid() });

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Should_ThrowClientValidation_WhenUserAlreadyMember()
    {
        await using var session = _fixture.CreateSession();
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var project = ProjectEntity.Create(projectId, name: "P");
        session.Store(project);

        var member = MemberEntity.Create(Guid.NewGuid(), userId, projectId,
            AuthorizeConstants.ProjectMember, DateTimeOffset.UtcNow);
        session.Store(member);
        await session.SaveChangesAsync();

        var userMock = new Mock<IUserApiService>();
        userMock.Setup(x => x.IsUserExistAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new AddProjectManagersCommandHandler(session, userMock.Object);
        var command = new AddProjectManagersCommand(projectId, new AddProjectManagersDto { UserId = userId });

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    [Fact]
    public async Task Handle_Should_AddManager_Successfully()
    {
        await using var session = _fixture.CreateSession();
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var project = ProjectEntity.Create(projectId, name: "P");
        session.Store(project);
        await session.SaveChangesAsync();

        var userMock = new Mock<IUserApiService>();
        userMock.Setup(x => x.IsUserExistAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        userMock.Setup(x => x.AssignUserRoleAsync(userId, AuthorizeConstants.ProjectManager, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new AddProjectManagersCommandHandler(session, userMock.Object);
        var command = new AddProjectManagersCommand(projectId, new AddProjectManagersDto { UserId = userId });

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeEmpty();
        userMock.Verify(x => x.AssignUserRoleAsync(userId, AuthorizeConstants.ProjectManager, It.IsAny<CancellationToken>()), Times.Once);
    }
}

#endregion

#region AddProjectMembersCommand

[Collection("MartenIntegration")]
public class AddProjectMembersCommandHandlerIntegrationTests
{
    private readonly MartenFixture _fixture;
    public AddProjectMembersCommandHandlerIntegrationTests(MartenFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Handle_Should_ThrowException_WhenSystemAdminGroupAssigned()
    {
        await using var session = _fixture.CreateSession();
        var userMock = new Mock<IUserApiService>();
        var handler = new AddProjectMembersCommandHandler(session, userMock.Object);
        var command = new AddProjectMembersCommand(Guid.NewGuid(), new AddProjectMembersDto
        {
            Members = new List<ProjectMemberEntry>
            {
                new() { UserId = Guid.NewGuid(), GroupName = AuthorizeConstants.SystemAdmin }
            }
        }, Guid.NewGuid());

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task Handle_Should_ThrowNotFound_WhenProjectDoesNotExist()
    {
        await using var session = _fixture.CreateSession();
        var userMock = new Mock<IUserApiService>();
        var handler = new AddProjectMembersCommandHandler(session, userMock.Object);
        var command = new AddProjectMembersCommand(Guid.NewGuid(), new AddProjectMembersDto
        {
            Members = new List<ProjectMemberEntry>
            {
                new() { UserId = Guid.NewGuid(), GroupName = AuthorizeConstants.ProjectMember }
            }
        }, Guid.NewGuid());

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Should_ThrowNoPermission_WhenUserNotProjectManager()
    {
        await using var session = _fixture.CreateSession();
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var project = ProjectEntity.Create(projectId, name: "P");
        session.Store(project);
        // userId is NOT a manager
        await session.SaveChangesAsync();

        var userMock = new Mock<IUserApiService>();
        var handler = new AddProjectMembersCommandHandler(session, userMock.Object);
        var command = new AddProjectMembersCommand(projectId, new AddProjectMembersDto
        {
            Members = new List<ProjectMemberEntry>
            {
                new() { UserId = Guid.NewGuid(), GroupName = AuthorizeConstants.ProjectMember }
            }
        }, userId);

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NoPermissionException>();
    }

    [Fact]
    public async Task Handle_Should_ThrowNotFound_WhenNoValidUsers()
    {
        await using var session = _fixture.CreateSession();
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var project = ProjectEntity.Create(projectId, name: "P");
        session.Store(project);

        var manager = MemberEntity.Create(Guid.NewGuid(), userId, projectId,
            AuthorizeConstants.ProjectManager, DateTimeOffset.UtcNow);
        session.Store(manager);
        await session.SaveChangesAsync();

        var userMock = new Mock<IUserApiService>();
        userMock.Setup(x => x.GetExistingUserIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Guid>());

        var handler = new AddProjectMembersCommandHandler(session, userMock.Object);
        var command = new AddProjectMembersCommand(projectId, new AddProjectMembersDto
        {
            Members = new List<ProjectMemberEntry>
            {
                new() { UserId = Guid.NewGuid(), GroupName = AuthorizeConstants.ProjectMember }
            }
        }, userId);

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Should_ThrowClientValidation_WhenAllMembersAlreadyExist()
    {
        await using var session = _fixture.CreateSession();
        var projectId = Guid.NewGuid();
        var managerId = Guid.NewGuid();
        var existingUserId = Guid.NewGuid();
        var project = ProjectEntity.Create(projectId, name: "P");
        session.Store(project);

        var manager = MemberEntity.Create(Guid.NewGuid(), managerId, projectId,
            AuthorizeConstants.ProjectManager, DateTimeOffset.UtcNow);
        var existingMember = MemberEntity.Create(Guid.NewGuid(), existingUserId, projectId,
            AuthorizeConstants.ProjectMember, DateTimeOffset.UtcNow);
        session.Store(manager);
        session.Store(existingMember);
        await session.SaveChangesAsync();

        var userMock = new Mock<IUserApiService>();
        userMock.Setup(x => x.GetExistingUserIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Guid> { existingUserId });

        var handler = new AddProjectMembersCommandHandler(session, userMock.Object);
        var command = new AddProjectMembersCommand(projectId, new AddProjectMembersDto
        {
            Members = new List<ProjectMemberEntry>
            {
                new() { UserId = existingUserId, GroupName = AuthorizeConstants.ProjectMember }
            }
        }, managerId);

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    [Fact]
    public async Task Handle_Should_CreateMembers_Successfully()
    {
        await using var session = _fixture.CreateSession();
        var projectId = Guid.NewGuid();
        var managerId = Guid.NewGuid();
        var newUserId = Guid.NewGuid();
        var project = ProjectEntity.Create(projectId, name: "P");
        session.Store(project);

        var manager = MemberEntity.Create(Guid.NewGuid(), managerId, projectId,
            AuthorizeConstants.ProjectManager, DateTimeOffset.UtcNow);
        session.Store(manager);
        await session.SaveChangesAsync();

        var userMock = new Mock<IUserApiService>();
        userMock.Setup(x => x.GetExistingUserIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Guid> { newUserId });
        userMock.Setup(x => x.AssignUserRoleAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new AddProjectMembersCommandHandler(session, userMock.Object);
        var command = new AddProjectMembersCommand(projectId, new AddProjectMembersDto
        {
            Members = new List<ProjectMemberEntry>
            {
                new() { UserId = newUserId, GroupName = AuthorizeConstants.ProjectMember }
            }
        }, managerId);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().HaveCount(1);
        userMock.Verify(x => x.AssignUserRoleAsync(newUserId, AuthorizeConstants.ProjectMember, It.IsAny<CancellationToken>()), Times.Once);
    }
}

#endregion

#region AddSubProjectMembersCommand

[Collection("MartenIntegration")]
public class AddSubProjectMembersCommandHandlerIntegrationTests
{
    private readonly MartenFixture _fixture;
    public AddSubProjectMembersCommandHandlerIntegrationTests(MartenFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Handle_Should_ThrowNotFound_WhenSubProjectDoesNotExist()
    {
        await using var session = _fixture.CreateSession();
        var userMock = new Mock<IUserApiService>();
        var labMock = new Mock<ILabApiService>();
        var handler = new AddSubProjectMembersCommandHandler(session, userMock.Object, labMock.Object);
        var command = new AddSubProjectMembersCommand(Guid.NewGuid(), new AddProjectMembersDto
        {
            Members = new List<ProjectMemberEntry> { new() { UserId = Guid.NewGuid() } }
        });

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Should_ThrowNotFound_WhenNoValidUsers()
    {
        await using var session = _fixture.CreateSession();
        var subId = Guid.NewGuid();
        var sub = ProjectEntity.Create(subId, name: "Sub", parentProjectId: Guid.NewGuid());
        session.Store(sub);
        await session.SaveChangesAsync();

        var userMock = new Mock<IUserApiService>();
        userMock.Setup(x => x.GetExistingUserIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Guid>());
        var labMock = new Mock<ILabApiService>();

        var handler = new AddSubProjectMembersCommandHandler(session, userMock.Object, labMock.Object);
        var command = new AddSubProjectMembersCommand(subId, new AddProjectMembersDto
        {
            Members = new List<ProjectMemberEntry> { new() { UserId = Guid.NewGuid() } }
        });

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Should_CreateMembers_WithAuthorContributors()
    {
        await using var session = _fixture.CreateSession();
        var subId = Guid.NewGuid();
        var paperId = Guid.NewGuid();
        var newUserId = Guid.NewGuid();
        var sub = ProjectEntity.Create(subId, name: "Sub", parentProjectId: Guid.NewGuid(),
            paperIds: new List<Guid> { paperId });
        session.Store(sub);
        await session.SaveChangesAsync();

        var userMock = new Mock<IUserApiService>();
        userMock.Setup(x => x.GetExistingUserIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Guid> { newUserId });

        var sectionId = Guid.NewGuid();
        var labMock = new Mock<ILabApiService>();
        labMock.Setup(x => x.GetSectionsByPaperIdAsync(paperId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<LabSectionDto> { new() { Id = sectionId, PaperId = paperId } });
        labMock.Setup(x => x.CreatePaperContributorAsync(
                It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<List<Guid>>(),
                It.IsAny<Guid>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new AddSubProjectMembersCommandHandler(session, userMock.Object, labMock.Object);
        var command = new AddSubProjectMembersCommand(subId, new AddProjectMembersDto
        {
            Members = new List<ProjectMemberEntry>
            {
                new() { UserId = newUserId, GroupName = AuthorizeConstants.PaperAuthor }
            }
        });

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().HaveCount(1);
        labMock.Verify(x => x.CreatePaperContributorAsync(
            AuthorizeConstants.PaperAuthor, paperId, It.IsAny<List<Guid>>(),
            sectionId, sectionId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_CreateMembers_WithoutContributors_WhenNotAuthor()
    {
        await using var session = _fixture.CreateSession();
        var subId = Guid.NewGuid();
        var newUserId = Guid.NewGuid();
        var sub = ProjectEntity.Create(subId, name: "Sub", parentProjectId: Guid.NewGuid());
        session.Store(sub);
        await session.SaveChangesAsync();

        var userMock = new Mock<IUserApiService>();
        userMock.Setup(x => x.GetExistingUserIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Guid> { newUserId });
        var labMock = new Mock<ILabApiService>();

        var handler = new AddSubProjectMembersCommandHandler(session, userMock.Object, labMock.Object);
        var command = new AddSubProjectMembersCommand(subId, new AddProjectMembersDto
        {
            Members = new List<ProjectMemberEntry>
            {
                new() { UserId = newUserId, GroupName = AuthorizeConstants.PaperMember }
            }
        });

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().HaveCount(1);
        labMock.Verify(x => x.GetSectionsByPaperIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}

#endregion

#region DeleteProjectManagersCommand

[Collection("MartenIntegration")]
public class DeleteProjectManagersCommandHandlerIntegrationTests
{
    private readonly MartenFixture _fixture;
    public DeleteProjectManagersCommandHandlerIntegrationTests(MartenFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Handle_Should_ThrowNotFound_WhenProjectDoesNotExist()
    {
        await using var session = _fixture.CreateSession();
        var handler = new DeleteProjectManagersCommandHandler(session);
        var command = new DeleteProjectManagersCommand(Guid.NewGuid(),
            new DeleteProjectManagersDto { MemberIds = new List<Guid> { Guid.NewGuid() } });

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Should_ThrowClientValidation_WhenEmptyMemberIds()
    {
        await using var session = _fixture.CreateSession();
        var projectId = Guid.NewGuid();
        var project = ProjectEntity.Create(projectId, name: "P");
        session.Store(project);
        await session.SaveChangesAsync();

        var handler = new DeleteProjectManagersCommandHandler(session);
        var command = new DeleteProjectManagersCommand(projectId,
            new DeleteProjectManagersDto { MemberIds = new List<Guid> { Guid.Empty } });

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    [Fact]
    public async Task Handle_Should_ThrowNotFound_WhenNoMatchingManagers()
    {
        await using var session = _fixture.CreateSession();
        var projectId = Guid.NewGuid();
        var project = ProjectEntity.Create(projectId, name: "P");
        session.Store(project);
        await session.SaveChangesAsync();

        var handler = new DeleteProjectManagersCommandHandler(session);
        var command = new DeleteProjectManagersCommand(projectId,
            new DeleteProjectManagersDto { MemberIds = new List<Guid> { Guid.NewGuid() } });

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Should_DeleteManagers_Successfully()
    {
        await using var session = _fixture.CreateSession();
        var projectId = Guid.NewGuid();
        var project = ProjectEntity.Create(projectId, name: "P");
        session.Store(project);

        var managerId = Guid.NewGuid();
        var manager = MemberEntity.Create(managerId, Guid.NewGuid(), projectId,
            AuthorizeConstants.ProjectManager, DateTimeOffset.UtcNow);
        session.Store(manager);
        await session.SaveChangesAsync();

        var handler = new DeleteProjectManagersCommandHandler(session);
        var command = new DeleteProjectManagersCommand(projectId,
            new DeleteProjectManagersDto { MemberIds = new List<Guid> { managerId } });

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().Contain(managerId);
    }
}

#endregion

#region DeleteProjectMembersCommand

[Collection("MartenIntegration")]
public class DeleteProjectMembersCommandHandlerIntegrationTests
{
    private readonly MartenFixture _fixture;
    public DeleteProjectMembersCommandHandlerIntegrationTests(MartenFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Handle_Should_ThrowNotFound_WhenProjectDoesNotExist()
    {
        await using var session = _fixture.CreateSession();
        var labMock = new Mock<ILabApiService>();
        var handler = new DeleteProjectMembersCommandHandler(session, labMock.Object);
        var command = new DeleteProjectMembersCommand(Guid.NewGuid(),
            new DeleteProjectMembersDto { MemberIds = new List<Guid> { Guid.NewGuid() } }, Guid.NewGuid());

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Should_ThrowNoPermission_WhenUserNotProjectManager()
    {
        await using var session = _fixture.CreateSession();
        var projectId = Guid.NewGuid();
        var project = ProjectEntity.Create(projectId, name: "P");
        session.Store(project);
        await session.SaveChangesAsync();

        var labMock = new Mock<ILabApiService>();
        var handler = new DeleteProjectMembersCommandHandler(session, labMock.Object);
        var command = new DeleteProjectMembersCommand(projectId,
            new DeleteProjectMembersDto { MemberIds = new List<Guid> { Guid.NewGuid() } }, Guid.NewGuid());

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NoPermissionException>();
    }

    [Fact]
    public async Task Handle_Should_ThrowClientValidation_WhenEmptyMemberIds()
    {
        await using var session = _fixture.CreateSession();
        var projectId = Guid.NewGuid();
        var managerId = Guid.NewGuid();
        var project = ProjectEntity.Create(projectId, name: "P");
        session.Store(project);

        var manager = MemberEntity.Create(Guid.NewGuid(), managerId, projectId,
            AuthorizeConstants.ProjectManager, DateTimeOffset.UtcNow);
        session.Store(manager);
        await session.SaveChangesAsync();

        var labMock = new Mock<ILabApiService>();
        var handler = new DeleteProjectMembersCommandHandler(session, labMock.Object);
        var command = new DeleteProjectMembersCommand(projectId,
            new DeleteProjectMembersDto { MemberIds = new List<Guid> { Guid.Empty } }, managerId);

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    [Fact]
    public async Task Handle_Should_DeleteMembers_WithSubProjectCascade()
    {
        await using var session = _fixture.CreateSession();
        var projectId = Guid.NewGuid();
        var subProjectId = Guid.NewGuid();
        var managerId = Guid.NewGuid();
        var memberUserId = Guid.NewGuid();
        var paperId = Guid.NewGuid();

        var project = ProjectEntity.Create(projectId, name: "P", paperIds: new List<Guid> { paperId });
        var subProject = ProjectEntity.Create(subProjectId, name: "Sub", parentProjectId: projectId);
        session.Store(project);
        session.Store(subProject);

        var manager = MemberEntity.Create(Guid.NewGuid(), managerId, projectId,
            AuthorizeConstants.ProjectManager, DateTimeOffset.UtcNow);
        var memberId = Guid.NewGuid();
        var memberToDelete = MemberEntity.Create(memberId, memberUserId, projectId,
            AuthorizeConstants.ProjectMember, DateTimeOffset.UtcNow);
        var subMember = MemberEntity.Create(Guid.NewGuid(), memberUserId, subProjectId,
            AuthorizeConstants.PaperMember, DateTimeOffset.UtcNow);
        session.Store(manager);
        session.Store(memberToDelete);
        session.Store(subMember);
        await session.SaveChangesAsync();

        var labMock = new Mock<ILabApiService>();
        labMock.Setup(x => x.GetPaperContributorsAsync(paperId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<LabPaperContributorDto>
            {
                new() { Id = Guid.NewGuid(), MemberId = memberId, PaperId = paperId }
            });
        labMock.Setup(x => x.DeletePaperContributorAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new DeleteProjectMembersCommandHandler(session, labMock.Object);
        var command = new DeleteProjectMembersCommand(projectId,
            new DeleteProjectMembersDto { MemberIds = new List<Guid> { memberId } }, managerId);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().Contain(memberId);
        labMock.Verify(x => x.DeletePaperContributorAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}

#endregion

#region DeleteSubProjectMembersCommand

[Collection("MartenIntegration")]
public class DeleteSubProjectMembersCommandHandlerIntegrationTests
{
    private readonly MartenFixture _fixture;
    public DeleteSubProjectMembersCommandHandlerIntegrationTests(MartenFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Handle_Should_ThrowNotFound_WhenSubProjectDoesNotExist()
    {
        await using var session = _fixture.CreateSession();
        var labMock = new Mock<ILabApiService>();
        var handler = new DeleteSubProjectMembersCommandHandler(session, labMock.Object);
        var command = new DeleteSubProjectMembersCommand(Guid.NewGuid(),
            new DeleteProjectMembersDto { MemberIds = new List<Guid> { Guid.NewGuid() } }, Guid.NewGuid());

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Should_ThrowClientValidation_WhenEmptyMemberIds()
    {
        await using var session = _fixture.CreateSession();
        var subId = Guid.NewGuid();
        var sub = ProjectEntity.Create(subId, name: "Sub", parentProjectId: Guid.NewGuid());
        session.Store(sub);
        await session.SaveChangesAsync();

        var labMock = new Mock<ILabApiService>();
        var handler = new DeleteSubProjectMembersCommandHandler(session, labMock.Object);
        var command = new DeleteSubProjectMembersCommand(subId,
            new DeleteProjectMembersDto { MemberIds = new List<Guid> { Guid.Empty } }, Guid.NewGuid());

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    [Fact]
    public async Task Handle_Should_DeleteMembers_WithContributorCleanup()
    {
        await using var session = _fixture.CreateSession();
        var subId = Guid.NewGuid();
        var paperId = Guid.NewGuid();
        var sub = ProjectEntity.Create(subId, name: "Sub", parentProjectId: Guid.NewGuid(),
            paperIds: new List<Guid> { paperId });
        session.Store(sub);

        var memberId = Guid.NewGuid();
        var member = MemberEntity.Create(memberId, Guid.NewGuid(), subId,
            AuthorizeConstants.PaperMember, DateTimeOffset.UtcNow);
        session.Store(member);
        await session.SaveChangesAsync();

        var contributorId = Guid.NewGuid();
        var labMock = new Mock<ILabApiService>();
        labMock.Setup(x => x.GetPaperContributorsAsync(paperId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<LabPaperContributorDto>
            {
                new() { Id = contributorId, MemberId = memberId, PaperId = paperId }
            });
        labMock.Setup(x => x.DeletePaperContributorAsync(contributorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new DeleteSubProjectMembersCommandHandler(session, labMock.Object);
        var command = new DeleteSubProjectMembersCommand(subId,
            new DeleteProjectMembersDto { MemberIds = new List<Guid> { memberId } }, Guid.NewGuid());

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().Contain(memberId);
        labMock.Verify(x => x.DeletePaperContributorAsync(contributorId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowNotFound_WhenNoMatchingMembers()
    {
        await using var session = _fixture.CreateSession();
        var subId = Guid.NewGuid();
        var sub = ProjectEntity.Create(subId, name: "Sub", parentProjectId: Guid.NewGuid());
        session.Store(sub);
        await session.SaveChangesAsync();

        var labMock = new Mock<ILabApiService>();
        var handler = new DeleteSubProjectMembersCommandHandler(session, labMock.Object);
        var command = new DeleteSubProjectMembersCommand(subId,
            new DeleteProjectMembersDto { MemberIds = new List<Guid> { Guid.NewGuid() } }, Guid.NewGuid());

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }
}

#endregion

#region UpdateProjectMemberRoleCommand

[Collection("MartenIntegration")]
public class UpdateProjectMemberRoleCommandHandlerIntegrationTests
{
    private readonly MartenFixture _fixture;
    public UpdateProjectMemberRoleCommandHandlerIntegrationTests(MartenFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Handle_Should_ThrowNotFound_WhenProjectDoesNotExist()
    {
        await using var session = _fixture.CreateSession();
        var handler = new UpdateProjectMemberRoleCommandHandler(session);
        var command = new UpdateProjectMemberRoleCommand(Guid.NewGuid(),
            new UpdateProjectMemberRoleDto { MemberId = Guid.NewGuid(), ProjectRole = "role" }, Guid.NewGuid().ToString());

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Should_ThrowNoPermission_WhenUserNotProjectManager()
    {
        await using var session = _fixture.CreateSession();
        var projectId = Guid.NewGuid();
        var project = ProjectEntity.Create(projectId, name: "P");
        session.Store(project);
        await session.SaveChangesAsync();

        var handler = new UpdateProjectMemberRoleCommandHandler(session);
        var command = new UpdateProjectMemberRoleCommand(projectId,
            new UpdateProjectMemberRoleDto { MemberId = Guid.NewGuid(), ProjectRole = "role" }, Guid.NewGuid().ToString());

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NoPermissionException>();
    }

    [Fact]
    public async Task Handle_Should_ThrowNotFound_WhenMemberNotFound()
    {
        await using var session = _fixture.CreateSession();
        var projectId = Guid.NewGuid();
        var managerId = Guid.NewGuid();
        var project = ProjectEntity.Create(projectId, name: "P");
        session.Store(project);

        var manager = MemberEntity.Create(Guid.NewGuid(), managerId, projectId,
            AuthorizeConstants.ProjectManager, DateTimeOffset.UtcNow);
        session.Store(manager);
        await session.SaveChangesAsync();

        var handler = new UpdateProjectMemberRoleCommandHandler(session);
        var command = new UpdateProjectMemberRoleCommand(projectId,
            new UpdateProjectMemberRoleDto { MemberId = Guid.NewGuid(), ProjectRole = "role" }, managerId.ToString());

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Should_UpdateRole_Successfully()
    {
        await using var session = _fixture.CreateSession();
        var projectId = Guid.NewGuid();
        var managerId = Guid.NewGuid();
        var project = ProjectEntity.Create(projectId, name: "P");
        session.Store(project);

        var manager = MemberEntity.Create(Guid.NewGuid(), managerId, projectId,
            AuthorizeConstants.ProjectManager, DateTimeOffset.UtcNow);
        var memberId = Guid.NewGuid();
        var member = MemberEntity.Create(memberId, Guid.NewGuid(), projectId,
            AuthorizeConstants.ProjectMember, DateTimeOffset.UtcNow);
        session.Store(manager);
        session.Store(member);
        await session.SaveChangesAsync();

        var handler = new UpdateProjectMemberRoleCommandHandler(session);
        var command = new UpdateProjectMemberRoleCommand(projectId,
            new UpdateProjectMemberRoleDto { MemberId = memberId, ProjectRole = AuthorizeConstants.ProjectAuthor },
            managerId.ToString());

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().Be(memberId);
        var updated = await session.LoadAsync<MemberEntity>(memberId);
        updated!.ProjectRole.Should().Be(AuthorizeConstants.ProjectAuthor);
    }
}

#endregion
