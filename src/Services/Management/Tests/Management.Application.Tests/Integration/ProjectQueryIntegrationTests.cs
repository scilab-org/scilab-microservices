using AutoMapper;
using Management.Application.Features.Project.Queries;
using Management.Application.Mappings;
using Management.Application.Tests.Common;

namespace Management.Application.Tests.Integration;

#region GetProjectByIdQuery

[Collection("MartenIntegration")]
public class GetProjectByIdQueryHandlerIntegrationTests
{
    private readonly MartenFixture _fixture;
    public GetProjectByIdQueryHandlerIntegrationTests(MartenFixture fixture) => _fixture = fixture;

    private static IMapper CreateMapper() =>
        new MapperConfiguration(cfg => cfg.AddProfile<ManagementMappingProfile>()).CreateMapper();

    [Fact]
    public async Task Handle_Should_ThrowNotFound_WhenProjectDoesNotExist()
    {
        await using var session = _fixture.CreateSession();
        var handler = new GetProjectByIdQueryHandler(session, CreateMapper());
        var query = new GetProjectByIdQuery(Guid.NewGuid(), Guid.NewGuid(), new List<string>());

        var act = () => handler.Handle(query, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Should_ThrowNotFound_WhenNonAdminNotMember()
    {
        await using var session = _fixture.CreateSession();
        var projectId = Guid.NewGuid();
        var project = ProjectEntity.Create(projectId, name: "P");
        session.Store(project);
        await session.SaveChangesAsync();

        var handler = new GetProjectByIdQueryHandler(session, CreateMapper());
        var query = new GetProjectByIdQuery(projectId, Guid.NewGuid(), new List<string> { "user" });

        var act = () => handler.Handle(query, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Should_ReturnProject_WhenSystemAdmin()
    {
        await using var session = _fixture.CreateSession();
        var projectId = Guid.NewGuid();
        var project = ProjectEntity.Create(projectId, name: "AdminProject");
        session.Store(project);
        await session.SaveChangesAsync();

        var handler = new GetProjectByIdQueryHandler(session, CreateMapper());
        var query = new GetProjectByIdQuery(projectId, Guid.NewGuid(), new List<string> { AuthorizeConstants.SystemAdmin });

        var result = await handler.Handle(query, CancellationToken.None);

        result.Project.Should().NotBeNull();
        result.Project.Name.Should().Be("AdminProject");
    }

    [Fact]
    public async Task Handle_Should_ReturnProject_WhenMember()
    {
        await using var session = _fixture.CreateSession();
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var project = ProjectEntity.Create(projectId, name: "MemberProject");
        session.Store(project);

        var member = MemberEntity.Create(Guid.NewGuid(), userId, projectId,
            AuthorizeConstants.ProjectMember, DateTimeOffset.UtcNow);
        session.Store(member);
        await session.SaveChangesAsync();

        var handler = new GetProjectByIdQueryHandler(session, CreateMapper());
        var query = new GetProjectByIdQuery(projectId, userId, new List<string> { "user" });

        var result = await handler.Handle(query, CancellationToken.None);

        result.Project.Should().NotBeNull();
        result.Project.Name.Should().Be("MemberProject");
    }
}

#endregion

#region GetProjectsQuery

[Collection("MartenIntegration")]
public class GetProjectsQueryHandlerIntegrationTests : IAsyncLifetime
{
    private readonly MartenFixture _fixture;
    public GetProjectsQueryHandlerIntegrationTests(MartenFixture fixture) => _fixture = fixture;

    public async Task InitializeAsync() => await _fixture.CleanAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    private static IMapper CreateMapper() =>
        new MapperConfiguration(cfg => cfg.AddProfile<ManagementMappingProfile>()).CreateMapper();

    [Fact]
    public async Task Handle_Should_ReturnEmpty_WhenNoProjects()
    {
        await using var session = _fixture.CreateSession();
        var handler = new GetProjectsQueryHandler(session, CreateMapper());
        var query = new GetProjectsQuery(new GetProjectsFilter(), new PaginationRequest(1, 10));

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_ReturnProjects_WithNameFilter()
    {
        await using var session = _fixture.CreateSession();
        session.Store(ProjectEntity.Create(Guid.NewGuid(), name: "Alpha Project"));
        session.Store(ProjectEntity.Create(Guid.NewGuid(), name: "Beta Project"));
        await session.SaveChangesAsync();

        var handler = new GetProjectsQueryHandler(session, CreateMapper());
        var query = new GetProjectsQuery(new GetProjectsFilter { Name = "Alpha" }, new PaginationRequest(1, 10));

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].Name.Should().Be("Alpha Project");
    }

    [Fact]
    public async Task Handle_Should_ReturnProjects_WithCodeFilter()
    {
        await using var session = _fixture.CreateSession();
        session.Store(ProjectEntity.Create(Guid.NewGuid(), name: "P1", code: "CODE-A"));
        session.Store(ProjectEntity.Create(Guid.NewGuid(), name: "P2", code: "CODE-B"));
        await session.SaveChangesAsync();

        var handler = new GetProjectsQueryHandler(session, CreateMapper());
        var query = new GetProjectsQuery(new GetProjectsFilter { Code = "CODE-A" }, new PaginationRequest(1, 10));

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_Should_ReturnProjects_WithStatusFilter()
    {
        await using var session = _fixture.CreateSession();
        session.Store(ProjectEntity.Create(Guid.NewGuid(), name: "Active", status: ProjectStatus.Active));
        session.Store(ProjectEntity.Create(Guid.NewGuid(), name: "Archived", status: ProjectStatus.Archived));
        await session.SaveChangesAsync();

        var handler = new GetProjectsQueryHandler(session, CreateMapper());
        var query = new GetProjectsQuery(new GetProjectsFilter { Status = ProjectStatus.Active }, new PaginationRequest(1, 10));

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().OnlyContain(x => x.Status == ProjectStatus.Active);
    }

    [Fact]
    public async Task Handle_Should_ExcludeSubProjects()
    {
        await using var session = _fixture.CreateSession();
        var parentId = Guid.NewGuid();
        session.Store(ProjectEntity.Create(parentId, name: "Parent"));
        session.Store(ProjectEntity.Create(Guid.NewGuid(), name: "Sub", parentProjectId: parentId));
        await session.SaveChangesAsync();

        var handler = new GetProjectsQueryHandler(session, CreateMapper());
        var query = new GetProjectsQuery(new GetProjectsFilter(), new PaginationRequest(1, 10));

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().OnlyContain(x => x.Name == "Parent");
    }
}

#endregion

#region GetMyProjectsQuery

[Collection("MartenIntegration")]
public class GetMyProjectsQueryHandlerIntegrationTests : IAsyncLifetime
{
    private readonly MartenFixture _fixture;
    public GetMyProjectsQueryHandlerIntegrationTests(MartenFixture fixture) => _fixture = fixture;

    public async Task InitializeAsync() => await _fixture.CleanAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    private static IMapper CreateMapper() =>
        new MapperConfiguration(cfg => cfg.AddProfile<ManagementMappingProfile>()).CreateMapper();

    [Fact]
    public async Task Handle_Should_ReturnEmpty_WhenUserHasNoMemberships()
    {
        await using var session = _fixture.CreateSession();
        var handler = new GetMyProjectsQueryHandler(session, CreateMapper());
        var query = new GetMyProjectsQuery(Guid.NewGuid(), new PaginationRequest(1, 10), new GetMyProjectsFilter());

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_ReturnProjects_WhenUserIsMember()
    {
        await using var session = _fixture.CreateSession();
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        session.Store(ProjectEntity.Create(projectId, name: "MyProject"));
        session.Store(MemberEntity.Create(Guid.NewGuid(), userId, projectId,
            AuthorizeConstants.ProjectMember, DateTimeOffset.UtcNow));
        await session.SaveChangesAsync();

        var handler = new GetMyProjectsQueryHandler(session, CreateMapper());
        var query = new GetMyProjectsQuery(userId, new PaginationRequest(1, 10), new GetMyProjectsFilter());

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].Name.Should().Be("MyProject");
    }

    [Fact]
    public async Task Handle_Should_FilterByName()
    {
        await using var session = _fixture.CreateSession();
        var userId = Guid.NewGuid();
        var p1 = Guid.NewGuid();
        var p2 = Guid.NewGuid();
        session.Store(ProjectEntity.Create(p1, name: "Alpha"));
        session.Store(ProjectEntity.Create(p2, name: "Beta"));
        session.Store(MemberEntity.Create(Guid.NewGuid(), userId, p1, AuthorizeConstants.ProjectMember, DateTimeOffset.UtcNow));
        session.Store(MemberEntity.Create(Guid.NewGuid(), userId, p2, AuthorizeConstants.ProjectMember, DateTimeOffset.UtcNow));
        await session.SaveChangesAsync();

        var handler = new GetMyProjectsQueryHandler(session, CreateMapper());
        var query = new GetMyProjectsQuery(userId, new PaginationRequest(1, 10), new GetMyProjectsFilter { Name = "Alpha" });

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
    }
}

#endregion

#region GetProjectsByUserIdQuery

[Collection("MartenIntegration")]
public class GetProjectsByUserIdQueryHandlerIntegrationTests : IAsyncLifetime
{
    private readonly MartenFixture _fixture;
    public GetProjectsByUserIdQueryHandlerIntegrationTests(MartenFixture fixture) => _fixture = fixture;

    public async Task InitializeAsync() => await _fixture.CleanAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    private static IMapper CreateMapper() =>
        new MapperConfiguration(cfg => cfg.AddProfile<ManagementMappingProfile>()).CreateMapper();

    [Fact]
    public async Task Handle_Should_ReturnEmpty_WhenUserHasNoMemberships()
    {
        await using var session = _fixture.CreateSession();
        var handler = new GetProjectsByUserIdQueryHandler(session, CreateMapper());
        var query = new GetProjectsByUserIdQuery(Guid.NewGuid(), new PaginationRequest(1, 10));

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_ReturnProjects_WhenUserIsMember()
    {
        await using var session = _fixture.CreateSession();
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        session.Store(ProjectEntity.Create(projectId, name: "UserProject"));
        session.Store(MemberEntity.Create(Guid.NewGuid(), userId, projectId,
            AuthorizeConstants.ProjectMember, DateTimeOffset.UtcNow));
        await session.SaveChangesAsync();

        var handler = new GetProjectsByUserIdQueryHandler(session, CreateMapper());
        var query = new GetProjectsByUserIdQuery(userId, new PaginationRequest(1, 10));

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
    }
}

#endregion

#region GetMyProjectRoleQuery

[Collection("MartenIntegration")]
public class GetMyProjectRoleQueryHandlerIntegrationTests
{
    private readonly MartenFixture _fixture;
    public GetMyProjectRoleQueryHandlerIntegrationTests(MartenFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Handle_Should_ReturnRole_WhenMemberExists()
    {
        await using var session = _fixture.CreateSession();
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        session.Store(MemberEntity.Create(Guid.NewGuid(), userId, projectId,
            AuthorizeConstants.ProjectManager, DateTimeOffset.UtcNow));
        await session.SaveChangesAsync();

        var redisMock = new Mock<IRedisService>();
        redisMock.Setup(x => x.GetOrSetCacheAsync(
                It.IsAny<string>(),
                It.IsAny<Func<CancellationToken, Task<string>>>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<CancellationToken>()))
            .Returns<string, Func<CancellationToken, Task<string>>, TimeSpan?, CancellationToken>(
                (_, factory, _, ct) => factory(ct)!);

        var handler = new GetMyProjectRoleQueryHandler(session, redisMock.Object);
        var query = new GetMyProjectRoleQuery(userId, projectId);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().Be(AuthorizeConstants.ProjectManager);
    }

    [Fact]
    public async Task Handle_Should_ReturnNone_WhenMemberDoesNotExist()
    {
        await using var session = _fixture.CreateSession();

        var redisMock = new Mock<IRedisService>();
        redisMock.Setup(x => x.GetOrSetCacheAsync(
                It.IsAny<string>(),
                It.IsAny<Func<CancellationToken, Task<string>>>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<CancellationToken>()))
            .Returns<string, Func<CancellationToken, Task<string>>, TimeSpan?, CancellationToken>(
                (_, factory, _, ct) => factory(ct)!);

        var handler = new GetMyProjectRoleQueryHandler(session, redisMock.Object);
        var query = new GetMyProjectRoleQuery(Guid.NewGuid(), Guid.NewGuid());

        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().Be("None");
    }
}

#endregion

#region GetSubProjectsQuery

[Collection("MartenIntegration")]
public class GetSubProjectsQueryHandlerIntegrationTests
{
    private readonly MartenFixture _fixture;
    public GetSubProjectsQueryHandlerIntegrationTests(MartenFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Handle_Should_ThrowNotFound_WhenProjectDoesNotExist()
    {
        await using var session = _fixture.CreateSession();
        var labMock = new Mock<ILabApiService>();
        var handler = new GetSubProjectsQueryHandler(session, labMock.Object);
        var query = new GetSubProjectsQuery(Guid.NewGuid(), new PaginationRequest(1, 10));

        var act = () => handler.Handle(query, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Should_ReturnEmpty_WhenNoSubProjects()
    {
        await using var session = _fixture.CreateSession();
        var projectId = Guid.NewGuid();
        session.Store(ProjectEntity.Create(projectId, name: "P"));
        await session.SaveChangesAsync();

        var labMock = new Mock<ILabApiService>();
        var handler = new GetSubProjectsQueryHandler(session, labMock.Object);
        var query = new GetSubProjectsQuery(projectId, new PaginationRequest(1, 10));

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_ReturnPapers_WhenSubProjectsExist()
    {
        await using var session = _fixture.CreateSession();
        var projectId = Guid.NewGuid();
        var subId = Guid.NewGuid();
        var paperId = Guid.NewGuid();
        session.Store(ProjectEntity.Create(projectId, name: "Parent"));
        session.Store(ProjectEntity.Create(subId, name: "Sub", parentProjectId: projectId,
            paperIds: new List<Guid> { paperId }));
        await session.SaveChangesAsync();

        var labMock = new Mock<ILabApiService>();
        labMock.Setup(x => x.GetPapersByIdsPagedAsync(
                It.IsAny<IEnumerable<Guid>>(), It.IsAny<string?>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<PaperInfoDto>
            {
                new() { Id = paperId, Title = "Paper1" }
            }, 1L));

        var handler = new GetSubProjectsQueryHandler(session, labMock.Object);
        var query = new GetSubProjectsQuery(projectId, new PaginationRequest(1, 10));

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].SubProjectId.Should().Be(subId);
    }

    [Fact]
    public async Task Handle_Should_ReturnEmpty_WhenSubProjectsHaveNoPapers()
    {
        await using var session = _fixture.CreateSession();
        var projectId = Guid.NewGuid();
        session.Store(ProjectEntity.Create(projectId, name: "Parent"));
        session.Store(ProjectEntity.Create(Guid.NewGuid(), name: "Sub", parentProjectId: projectId));
        await session.SaveChangesAsync();

        var labMock = new Mock<ILabApiService>();
        var handler = new GetSubProjectsQueryHandler(session, labMock.Object);
        var query = new GetSubProjectsQuery(projectId, new PaginationRequest(1, 10));

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().BeEmpty();
    }
}

#endregion

#region GetAssignedPapersQuery

[Collection("MartenIntegration")]
public class GetAssignedPapersQueryHandlerIntegrationTests : IAsyncLifetime
{
    private readonly MartenFixture _fixture;
    public GetAssignedPapersQueryHandlerIntegrationTests(MartenFixture fixture) => _fixture = fixture;

    public async Task InitializeAsync() => await _fixture.CleanAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Handle_Should_ReturnEmpty_WhenNoMatchingParentProjects()
    {
        await using var session = _fixture.CreateSession();
        var labMock = new Mock<ILabApiService>();
        var handler = new GetAssignedPapersQueryHandler(session, labMock.Object);
        var query = new GetAssignedPapersQuery(Guid.NewGuid(), new PaginationRequest(1, 10), new GetAssignedPapersFilter());

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_ReturnEmpty_WhenUserHasNoMemberships()
    {
        await using var session = _fixture.CreateSession();
        var projectId = Guid.NewGuid();
        session.Store(ProjectEntity.Create(projectId, name: "P"));
        await session.SaveChangesAsync();

        var labMock = new Mock<ILabApiService>();
        var handler = new GetAssignedPapersQueryHandler(session, labMock.Object);
        var query = new GetAssignedPapersQuery(Guid.NewGuid(), new PaginationRequest(1, 10), new GetAssignedPapersFilter());

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_ReturnAssignedPapers()
    {
        await using var session = _fixture.CreateSession();
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var subId = Guid.NewGuid();
        var paperId = Guid.NewGuid();

        session.Store(ProjectEntity.Create(projectId, name: "Parent", code: "P001"));
        session.Store(ProjectEntity.Create(subId, name: "Sub", parentProjectId: projectId,
            paperIds: new List<Guid> { paperId }));

        // User is member of parent project
        session.Store(MemberEntity.Create(Guid.NewGuid(), userId, projectId,
            AuthorizeConstants.ProjectMember, DateTimeOffset.UtcNow));
        // User is member of sub-project
        session.Store(MemberEntity.Create(Guid.NewGuid(), userId, subId,
            AuthorizeConstants.PaperAuthor, DateTimeOffset.UtcNow));
        await session.SaveChangesAsync();

        var labMock = new Mock<ILabApiService>();
        labMock.Setup(x => x.GetPapersByIdsPagedAsync(
                It.IsAny<IEnumerable<Guid>>(), It.IsAny<string?>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<PaperInfoDto>
            {
                new() { Id = paperId, Title = "MyPaper" }
            }, 1L));

        var handler = new GetAssignedPapersQueryHandler(session, labMock.Object);
        var query = new GetAssignedPapersQuery(userId, new PaginationRequest(1, 10), new GetAssignedPapersFilter());

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].ProjectCode.Should().Be("P001");
    }

    [Fact]
    public async Task Handle_Should_ReturnEmpty_WhenNoSubProjectMembership()
    {
        await using var session = _fixture.CreateSession();
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var subId = Guid.NewGuid();

        session.Store(ProjectEntity.Create(projectId, name: "Parent"));
        session.Store(ProjectEntity.Create(subId, name: "Sub", parentProjectId: projectId,
            paperIds: new List<Guid> { Guid.NewGuid() }));

        // User is member of parent but NOT sub-project
        session.Store(MemberEntity.Create(Guid.NewGuid(), userId, projectId,
            AuthorizeConstants.ProjectMember, DateTimeOffset.UtcNow));
        await session.SaveChangesAsync();

        var labMock = new Mock<ILabApiService>();
        var handler = new GetAssignedPapersQueryHandler(session, labMock.Object);
        var query = new GetAssignedPapersQuery(userId, new PaginationRequest(1, 10), new GetAssignedPapersFilter());

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_FilterByProjectId()
    {
        await using var session = _fixture.CreateSession();
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var subId = Guid.NewGuid();
        var paperId = Guid.NewGuid();

        session.Store(ProjectEntity.Create(projectId, name: "Target"));
        session.Store(ProjectEntity.Create(subId, name: "Sub", parentProjectId: projectId,
            paperIds: new List<Guid> { paperId }));
        session.Store(MemberEntity.Create(Guid.NewGuid(), userId, projectId,
            AuthorizeConstants.ProjectMember, DateTimeOffset.UtcNow));
        session.Store(MemberEntity.Create(Guid.NewGuid(), userId, subId,
            AuthorizeConstants.PaperAuthor, DateTimeOffset.UtcNow));
        await session.SaveChangesAsync();

        var labMock = new Mock<ILabApiService>();
        labMock.Setup(x => x.GetPapersByIdsPagedAsync(
                It.IsAny<IEnumerable<Guid>>(), It.IsAny<string?>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<PaperInfoDto> { new() { Id = paperId } }, 1L));

        var handler = new GetAssignedPapersQueryHandler(session, labMock.Object);
        var query = new GetAssignedPapersQuery(userId, new PaginationRequest(1, 10),
            new GetAssignedPapersFilter { ProjectId = projectId });

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
    }
}

#endregion

#region GetAvailableProjectUsersQuery

[Collection("MartenIntegration")]
public class GetAvailableProjectUsersQueryHandlerIntegrationTests
{
    private readonly MartenFixture _fixture;
    public GetAvailableProjectUsersQueryHandlerIntegrationTests(MartenFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Handle_Should_ThrowNotFound_WhenProjectDoesNotExist()
    {
        await using var session = _fixture.CreateSession();
        var userMock = new Mock<IUserApiService>();
        var handler = new GetProjectAvailableUsersQueryHandler(session, userMock.Object);
        var query = new GetAvailableProjectUsersQuery(Guid.NewGuid(), AuthorizeConstants.SystemAdmin);

        var act = () => handler.Handle(query, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Should_ReturnAvailableUsers()
    {
        await using var session = _fixture.CreateSession();
        var projectId = Guid.NewGuid();
        var existingUserId = Guid.NewGuid();
        session.Store(ProjectEntity.Create(projectId, name: "P"));
        session.Store(MemberEntity.Create(Guid.NewGuid(), existingUserId, projectId,
            AuthorizeConstants.ProjectMember, DateTimeOffset.UtcNow));
        await session.SaveChangesAsync();

        var userMock = new Mock<IUserApiService>();
        userMock.Setup(x => x.GetAvailableProjectUsersAsync(
                It.IsAny<IEnumerable<Guid>>(), It.IsAny<string>(),
                It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserInfoDto>
            {
                new() { Id = Guid.NewGuid().ToString(), Username = "newuser" }
            });

        var handler = new GetProjectAvailableUsersQueryHandler(session, userMock.Object);
        var query = new GetAvailableProjectUsersQuery(projectId, AuthorizeConstants.SystemAdmin);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        userMock.Verify(x => x.GetAvailableProjectUsersAsync(
            It.Is<IEnumerable<Guid>>(ids => ids.Contains(existingUserId)),
            AuthorizeConstants.SystemAdmin, null, It.IsAny<CancellationToken>()), Times.Once);
    }
}

#endregion

#region GetProjectSubmissionStatusSummaryQuery

[Collection("MartenIntegration")]
public class GetProjectSubmissionStatusSummaryQueryHandlerIntegrationTests
{
    private readonly MartenFixture _fixture;
    public GetProjectSubmissionStatusSummaryQueryHandlerIntegrationTests(MartenFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Handle_Should_ThrowNotFound_WhenProjectDoesNotExist()
    {
        await using var session = _fixture.CreateSession();
        var labMock = new Mock<ILabApiService>();
        var handler = new GetProjectSubmissionStatusSummaryQueryHandler(session, labMock.Object);
        var query = new GetProjectSubmissionStatusSummaryQuery(Guid.NewGuid());

        var act = () => handler.Handle(query, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptySummary_WhenNoPaperIds()
    {
        await using var session = _fixture.CreateSession();
        var projectId = Guid.NewGuid();
        session.Store(ProjectEntity.Create(projectId, name: "P"));
        await session.SaveChangesAsync();

        var labMock = new Mock<ILabApiService>();
        var handler = new GetProjectSubmissionStatusSummaryQueryHandler(session, labMock.Object);
        var query = new GetProjectSubmissionStatusSummaryQuery(projectId);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_ReturnSummary_WhenPapersExist()
    {
        await using var session = _fixture.CreateSession();
        var projectId = Guid.NewGuid();
        var paperId = Guid.NewGuid();
        session.Store(ProjectEntity.Create(projectId, name: "P"));
        session.Store(ProjectEntity.Create(Guid.NewGuid(), name: "Sub", parentProjectId: projectId,
            paperIds: new List<Guid> { paperId }));
        await session.SaveChangesAsync();

        var labMock = new Mock<ILabApiService>();
        labMock.Setup(x => x.GetSubmissionStatusSummaryAsync(
                It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SubmissionStatusSummaryResult
            {
                Items = new List<SubmissionStatusSummaryItem>
                {
                    new() { Status = 1, Count = 5 }
                }
            });

        var handler = new GetProjectSubmissionStatusSummaryQueryHandler(session, labMock.Object);
        var query = new GetProjectSubmissionStatusSummaryQuery(projectId);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].Count.Should().Be(5);
    }
}

#endregion
