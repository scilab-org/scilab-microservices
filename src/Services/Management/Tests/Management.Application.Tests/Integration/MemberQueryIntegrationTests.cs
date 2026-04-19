using Management.Application.Features.Member.Queries;
using Management.Application.Tests.Common;

namespace Management.Application.Tests.Integration;

#region GetMemberByPaperIdQuery

[Collection("MartenIntegration")]
public class GetMemberByPaperIdQueryHandlerIntegrationTests
{
    private readonly MartenFixture _fixture;
    public GetMemberByPaperIdQueryHandlerIntegrationTests(MartenFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Handle_Should_ThrowNotFound_WhenSubProjectNotFound()
    {
        await using var session = _fixture.CreateSession();
        var handler = new GetMemberByPaperIdQueryHandler(session);
        var query = new GetMemberByPaperIdQuery(Guid.NewGuid(), Guid.NewGuid());

        var act = () => handler.Handle(query, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Should_ThrowNotFound_WhenMemberNotFound()
    {
        await using var session = _fixture.CreateSession();
        var paperId = Guid.NewGuid();
        var subId = Guid.NewGuid();
        var parentId = Guid.NewGuid();
        session.Store(ProjectEntity.Create(subId, name: "Sub", parentProjectId: parentId,
            paperIds: new List<Guid> { paperId }));
        await session.SaveChangesAsync();

        var handler = new GetMemberByPaperIdQueryHandler(session);
        var query = new GetMemberByPaperIdQuery(paperId, Guid.NewGuid());

        var act = () => handler.Handle(query, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Should_ReturnMember_WhenFound()
    {
        await using var session = _fixture.CreateSession();
        var paperId = Guid.NewGuid();
        var parentId = Guid.NewGuid();
        var subId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var memberId = Guid.NewGuid();

        session.Store(ProjectEntity.Create(subId, name: "Sub", parentProjectId: parentId,
            paperIds: new List<Guid> { paperId }));
        session.Store(MemberEntity.Create(memberId, userId, subId,
            AuthorizeConstants.PaperAuthor, DateTimeOffset.UtcNow));
        await session.SaveChangesAsync();

        var handler = new GetMemberByPaperIdQueryHandler(session);
        var query = new GetMemberByPaperIdQuery(paperId, userId);

        var result = await handler.Handle(query, CancellationToken.None);

        result.MemberId.Should().Be(memberId);
        result.SubProjectId.Should().Be(subId);
        result.ProjectId.Should().Be(parentId);
        result.Role.Should().Be(AuthorizeConstants.PaperAuthor);
    }
}

#endregion

#region GetProjectMembersQuery

[Collection("MartenIntegration")]
public class GetProjectMembersQueryHandlerIntegrationTests
{
    private readonly MartenFixture _fixture;
    public GetProjectMembersQueryHandlerIntegrationTests(MartenFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Handle_Should_ThrowNotFound_WhenProjectDoesNotExist()
    {
        await using var session = _fixture.CreateSession();
        var userMock = new Mock<IUserApiService>();
        var handler = new GetProjectMembersQueryHandler(session, userMock.Object);
        var query = new GetProjectMembersQuery(Guid.NewGuid(), new GetProjectMembersFilter(), new PaginationRequest(1, 10));

        var act = () => handler.Handle(query, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Should_ReturnMembers_WithPagination()
    {
        await using var session = _fixture.CreateSession();
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        session.Store(ProjectEntity.Create(projectId, name: "P"));
        session.Store(MemberEntity.Create(Guid.NewGuid(), userId, projectId,
            AuthorizeConstants.ProjectManager, DateTimeOffset.UtcNow));
        await session.SaveChangesAsync();

        var userMock = new Mock<IUserApiService>();
        userMock.Setup(x => x.GetUsersByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserInfoDto>
            {
                new() { Id = userId.ToString(), Username = "manager", Email = "m@test.com" }
            });

        var handler = new GetProjectMembersQueryHandler(session, userMock.Object);
        var query = new GetProjectMembersQuery(projectId, new GetProjectMembersFilter(), new PaginationRequest(1, 10));

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].Username.Should().Be("manager");
    }

    [Fact]
    public async Task Handle_Should_FilterByEmail()
    {
        await using var session = _fixture.CreateSession();
        var projectId = Guid.NewGuid();
        var u1 = Guid.NewGuid();
        var u2 = Guid.NewGuid();
        session.Store(ProjectEntity.Create(projectId, name: "P"));
        session.Store(MemberEntity.Create(Guid.NewGuid(), u1, projectId, AuthorizeConstants.ProjectMember, DateTimeOffset.UtcNow));
        session.Store(MemberEntity.Create(Guid.NewGuid(), u2, projectId, AuthorizeConstants.ProjectMember, DateTimeOffset.UtcNow));
        await session.SaveChangesAsync();

        var userMock = new Mock<IUserApiService>();
        userMock.Setup(x => x.GetUsersByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserInfoDto>
            {
                new() { Id = u1.ToString(), Email = "alpha@test.com" },
                new() { Id = u2.ToString(), Email = "beta@test.com" }
            });

        var handler = new GetProjectMembersQueryHandler(session, userMock.Object);
        var query = new GetProjectMembersQuery(projectId,
            new GetProjectMembersFilter { SearchEmail = "alpha" }, new PaginationRequest(1, 10));

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_Should_FilterByRole()
    {
        await using var session = _fixture.CreateSession();
        var projectId = Guid.NewGuid();
        var u1 = Guid.NewGuid();
        var u2 = Guid.NewGuid();
        session.Store(ProjectEntity.Create(projectId, name: "P"));
        session.Store(MemberEntity.Create(Guid.NewGuid(), u1, projectId, AuthorizeConstants.ProjectManager, DateTimeOffset.UtcNow));
        session.Store(MemberEntity.Create(Guid.NewGuid(), u2, projectId, AuthorizeConstants.ProjectMember, DateTimeOffset.UtcNow));
        await session.SaveChangesAsync();

        var userMock = new Mock<IUserApiService>();
        userMock.Setup(x => x.GetUsersByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserInfoDto>
            {
                new() { Id = u1.ToString(), Email = "m@test.com" },
                new() { Id = u2.ToString(), Email = "u@test.com" }
            });

        var handler = new GetProjectMembersQueryHandler(session, userMock.Object);
        var query = new GetProjectMembersQuery(projectId,
            new GetProjectMembersFilter { ProjectRole = AuthorizeConstants.ProjectManager }, new PaginationRequest(1, 10));

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].Role.Should().Be(AuthorizeConstants.ProjectManager);
    }
}

#endregion

#region GetSubProjectMembersQuery

[Collection("MartenIntegration")]
public class GetSubProjectMembersQueryHandlerIntegrationTests
{
    private readonly MartenFixture _fixture;
    public GetSubProjectMembersQueryHandlerIntegrationTests(MartenFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Handle_Should_ThrowNotFound_WhenSubProjectDoesNotExist()
    {
        await using var session = _fixture.CreateSession();
        var userMock = new Mock<IUserApiService>();
        var handler = new GetSubProjectMembersQueryHandler(session, userMock.Object);
        var query = new GetSubProjectMembersQuery(Guid.NewGuid(), new GetProjectMembersFilter(), new PaginationRequest(1, 10));

        var act = () => handler.Handle(query, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Should_ReturnEmpty_WhenNoMembers()
    {
        await using var session = _fixture.CreateSession();
        var subId = Guid.NewGuid();
        session.Store(ProjectEntity.Create(subId, name: "Sub", parentProjectId: Guid.NewGuid()));
        await session.SaveChangesAsync();

        var userMock = new Mock<IUserApiService>();
        var handler = new GetSubProjectMembersQueryHandler(session, userMock.Object);
        var query = new GetSubProjectMembersQuery(subId, new GetProjectMembersFilter(), new PaginationRequest(1, 10));

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_ReturnMembers_WhenExist()
    {
        await using var session = _fixture.CreateSession();
        var subId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        session.Store(ProjectEntity.Create(subId, name: "Sub", parentProjectId: Guid.NewGuid()));
        session.Store(MemberEntity.Create(Guid.NewGuid(), userId, subId,
            AuthorizeConstants.PaperAuthor, DateTimeOffset.UtcNow));
        await session.SaveChangesAsync();

        var userMock = new Mock<IUserApiService>();
        userMock.Setup(x => x.GetUsersByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserInfoDto>
            {
                new() { Id = userId.ToString(), Username = "author", Email = "a@test.com" }
            });

        var handler = new GetSubProjectMembersQueryHandler(session, userMock.Object);
        var query = new GetSubProjectMembersQuery(subId, new GetProjectMembersFilter(), new PaginationRequest(1, 10));

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].Username.Should().Be("author");
    }
}

#endregion

#region GetSubProjectMembersByPaperIdQuery

[Collection("MartenIntegration")]
public class GetSubProjectMembersByPaperIdQueryHandlerIntegrationTests
{
    private readonly MartenFixture _fixture;
    public GetSubProjectMembersByPaperIdQueryHandlerIntegrationTests(MartenFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Handle_Should_ThrowNotFound_WhenSubProjectNotFound()
    {
        await using var session = _fixture.CreateSession();
        var userMock = new Mock<IUserApiService>();
        var handler = new GetSubProjectMembersByPaperIdQueryHandler(session, userMock.Object);
        var query = new GetSubProjectMembersByPaperIdQuery(Guid.NewGuid());

        var act = () => handler.Handle(query, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Should_ReturnEmpty_WhenNoMembers()
    {
        await using var session = _fixture.CreateSession();
        var paperId = Guid.NewGuid();
        var subId = Guid.NewGuid();
        session.Store(ProjectEntity.Create(subId, name: "Sub", parentProjectId: Guid.NewGuid(),
            paperIds: new List<Guid> { paperId }));
        await session.SaveChangesAsync();

        var userMock = new Mock<IUserApiService>();
        var handler = new GetSubProjectMembersByPaperIdQueryHandler(session, userMock.Object);
        var query = new GetSubProjectMembersByPaperIdQuery(paperId);

        var result = await handler.Handle(query, CancellationToken.None);

        result.SubProjectId.Should().Be(subId);
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_ReturnMembers_WithUserInfo()
    {
        await using var session = _fixture.CreateSession();
        var paperId = Guid.NewGuid();
        var subId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var memberId = Guid.NewGuid();

        session.Store(ProjectEntity.Create(subId, name: "Sub", parentProjectId: Guid.NewGuid(),
            paperIds: new List<Guid> { paperId }));
        session.Store(MemberEntity.Create(memberId, userId, subId,
            AuthorizeConstants.PaperAuthor, DateTimeOffset.UtcNow));
        await session.SaveChangesAsync();

        var userMock = new Mock<IUserApiService>();
        userMock.Setup(x => x.GetUsersByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserInfoDto>
            {
                new() { Id = userId.ToString(), Username = "author", Email = "a@test.com", Enabled = true }
            });

        var handler = new GetSubProjectMembersByPaperIdQueryHandler(session, userMock.Object);
        var query = new GetSubProjectMembersByPaperIdQuery(paperId);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].MemberId.Should().Be(memberId);
        result.Items[0].Username.Should().Be("author");
        result.Items[0].Role.Should().Be(AuthorizeConstants.PaperAuthor);
    }
}

#endregion

#region GetAvailableSubProjectMembersQuery

[Collection("MartenIntegration")]
public class GetAvailableSubProjectMembersQueryHandlerIntegrationTests
{
    private readonly MartenFixture _fixture;
    public GetAvailableSubProjectMembersQueryHandlerIntegrationTests(MartenFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Handle_Should_ThrowNotFound_WhenSubProjectDoesNotExist()
    {
        await using var session = _fixture.CreateSession();
        var userMock = new Mock<IUserApiService>();
        var handler = new GetAvailableSubProjectMembersQueryHandler(session, userMock.Object);
        var query = new GetAvailableSubProjectMembersQuery(Guid.NewGuid(),
            new GetAvailableSubProjectMembersFilter(), new PaginationRequest(1, 10));

        var act = () => handler.Handle(query, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Should_ReturnEmpty_WhenNoAvailableMembers()
    {
        await using var session = _fixture.CreateSession();
        var parentId = Guid.NewGuid();
        var subId = Guid.NewGuid();
        session.Store(ProjectEntity.Create(subId, name: "Sub", parentProjectId: parentId));
        // No parent members
        await session.SaveChangesAsync();

        var userMock = new Mock<IUserApiService>();
        var handler = new GetAvailableSubProjectMembersQueryHandler(session, userMock.Object);
        var query = new GetAvailableSubProjectMembersQuery(subId,
            new GetAvailableSubProjectMembersFilter(), new PaginationRequest(1, 10));

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_ReturnAvailableMembers()
    {
        await using var session = _fixture.CreateSession();
        var parentId = Guid.NewGuid();
        var subId = Guid.NewGuid();
        var availableUserId = Guid.NewGuid();
        var existingUserId = Guid.NewGuid();

        session.Store(ProjectEntity.Create(subId, name: "Sub", parentProjectId: parentId));

        // Parent members
        session.Store(MemberEntity.Create(Guid.NewGuid(), availableUserId, parentId,
            AuthorizeConstants.ProjectMember, DateTimeOffset.UtcNow));
        session.Store(MemberEntity.Create(Guid.NewGuid(), existingUserId, parentId,
            AuthorizeConstants.ProjectMember, DateTimeOffset.UtcNow));

        // existingUserId is already in sub-project
        session.Store(MemberEntity.Create(Guid.NewGuid(), existingUserId, subId,
            AuthorizeConstants.PaperMember, DateTimeOffset.UtcNow));
        await session.SaveChangesAsync();

        var userMock = new Mock<IUserApiService>();
        userMock.Setup(x => x.GetUsersByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserInfoDto>
            {
                new() { Id = availableUserId.ToString(), Username = "available", Email = "avail@test.com" }
            });

        var handler = new GetAvailableSubProjectMembersQueryHandler(session, userMock.Object);
        var query = new GetAvailableSubProjectMembersQuery(subId,
            new GetAvailableSubProjectMembersFilter(), new PaginationRequest(1, 10));

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].Username.Should().Be("available");
    }

    [Fact]
    public async Task Handle_Should_FilterByEmail()
    {
        await using var session = _fixture.CreateSession();
        var parentId = Guid.NewGuid();
        var subId = Guid.NewGuid();
        var u1 = Guid.NewGuid();
        var u2 = Guid.NewGuid();

        session.Store(ProjectEntity.Create(subId, name: "Sub", parentProjectId: parentId));
        session.Store(MemberEntity.Create(Guid.NewGuid(), u1, parentId, AuthorizeConstants.ProjectMember, DateTimeOffset.UtcNow));
        session.Store(MemberEntity.Create(Guid.NewGuid(), u2, parentId, AuthorizeConstants.ProjectMember, DateTimeOffset.UtcNow));
        await session.SaveChangesAsync();

        var userMock = new Mock<IUserApiService>();
        userMock.Setup(x => x.GetUsersByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserInfoDto>
            {
                new() { Id = u1.ToString(), Email = "alpha@test.com" },
                new() { Id = u2.ToString(), Email = "beta@test.com" }
            });

        var handler = new GetAvailableSubProjectMembersQueryHandler(session, userMock.Object);
        var query = new GetAvailableSubProjectMembersQuery(subId,
            new GetAvailableSubProjectMembersFilter { SearchEmail = "alpha" }, new PaginationRequest(1, 10));

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
    }
}

#endregion
