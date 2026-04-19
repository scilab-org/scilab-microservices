using Common.Constants;
using Common.Models;
using Management.Application.Dtos.Members;
using Management.Application.Features.Member.Commands;
using Management.Application.Features.Member.Queries;
using Management.Application.Models.Results;

namespace Management.Api.Tests.Endpoints;

public sealed class MemberEndpointTests : IClassFixture<ApiTestFactory>
{
    private readonly ApiTestFactory _factory;

    public MemberEndpointTests(ApiTestFactory factory)
    {
        _factory = factory;
        factory.SenderMock.Reset();
    }

    [Fact]
    public async Task AddProjectManagers_WhenSystemAdmin_Returns201()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<AddProjectManagersCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        var client = _factory.CreateTestClient(AuthorizeConstants.SystemAdmin);
        var response = await client.PostAsJsonAsync($"/admin/projects/{Guid.NewGuid()}/managers", new AddProjectManagersDto { UserId = Guid.NewGuid() });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task AddProjectManagers_WhenNotAdmin_Returns403()
    {
        var client = _factory.CreateTestClient(AuthorizeConstants.ProjectManager);
        var response = await client.PostAsJsonAsync($"/admin/projects/{Guid.NewGuid()}/managers", new AddProjectManagersDto { UserId = Guid.NewGuid() });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AddProjectManagers_WhenNoAuth_Returns401()
    {
        var client = _factory.CreateUnauthenticatedClient();
        var response = await client.PostAsJsonAsync($"/admin/projects/{Guid.NewGuid()}/managers", new AddProjectManagersDto { UserId = Guid.NewGuid() });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteProjectManagers_WhenSystemAdmin_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<DeleteProjectManagersCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Guid> { Guid.NewGuid() });

        var client = _factory.CreateTestClient(AuthorizeConstants.SystemAdmin);
        var response = await client.PostAsJsonAsync($"/admin/projects/{Guid.NewGuid()}/managers/remove", new DeleteProjectManagersDto { MemberIds = new List<Guid> { Guid.NewGuid() } });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteProjectManagers_WhenNotAdmin_Returns403()
    {
        var client = _factory.CreateTestClient(AuthorizeConstants.ProjectManager);
        var response = await client.PostAsJsonAsync($"/admin/projects/{Guid.NewGuid()}/managers/remove", new DeleteProjectManagersDto { MemberIds = new List<Guid> { Guid.NewGuid() } });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteProjectManagers_WhenNoAuth_Returns401()
    {
        var client = _factory.CreateUnauthenticatedClient();
        var response = await client.PostAsJsonAsync($"/admin/projects/{Guid.NewGuid()}/managers/remove", new DeleteProjectManagersDto { MemberIds = new List<Guid> { Guid.NewGuid() } });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AddProjectMembers_WhenAuthenticated_Returns201()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<AddProjectMembersCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Guid> { Guid.NewGuid() });

        var client = _factory.CreateTestClient(AuthorizeConstants.ProjectManager);
        var body = new AddProjectMembersDto { Members = new List<ProjectMemberEntry> { new() { UserId = Guid.NewGuid() } } };
        var response = await client.PostAsJsonAsync($"/manager/projects/{Guid.NewGuid()}/members", body);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task AddProjectMembers_WhenNoAuth_Returns401()
    {
        var client = _factory.CreateUnauthenticatedClient();
        var body = new AddProjectMembersDto { Members = new List<ProjectMemberEntry> { new() { UserId = Guid.NewGuid() } } };
        var response = await client.PostAsJsonAsync($"/manager/projects/{Guid.NewGuid()}/members", body);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteProjectMembers_WhenAuthenticated_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<DeleteProjectMembersCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Guid> { Guid.NewGuid() });

        var client = _factory.CreateTestClient(AuthorizeConstants.ProjectManager);
        var response = await client.PostAsJsonAsync($"/manager/projects/{Guid.NewGuid()}/members/remove", new DeleteProjectMembersDto { MemberIds = new List<Guid> { Guid.NewGuid() } });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteProjectMembers_WhenNoAuth_Returns401()
    {
        var client = _factory.CreateUnauthenticatedClient();
        var response = await client.PostAsJsonAsync($"/manager/projects/{Guid.NewGuid()}/members/remove", new DeleteProjectMembersDto { MemberIds = new List<Guid> { Guid.NewGuid() } });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateProjectMemberRole_WhenAuthenticated_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<UpdateProjectMemberRoleCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        var client = _factory.CreateTestClient(AuthorizeConstants.ProjectManager);
        var response = await client.PutAsJsonAsync($"/manager/projects/{Guid.NewGuid()}/members/{Guid.NewGuid()}/role", "ProjectMember");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateProjectMemberRole_WhenNoAuth_Returns401()
    {
        var client = _factory.CreateUnauthenticatedClient();
        var response = await client.PutAsJsonAsync($"/manager/projects/{Guid.NewGuid()}/members/{Guid.NewGuid()}/role", "ProjectMember");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetProjectMembers_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<GetProjectMembersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetProjectMembersResult(new List<ProjectMemberDto>(), 0, new PaginationRequest()));

        var client = _factory.CreateTestClient();
        var response = await client.GetAsync($"/projects/{Guid.NewGuid()}/members");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetMemberById_WhenAuthenticated_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<GetMemberByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MemberDto());

        var client = _factory.CreateTestClient(AuthorizeConstants.User);
        var response = await client.GetAsync($"/projects/members/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetMemberById_WhenNotFound_Returns404()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<GetMemberByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((MemberDto?)null);

        var client = _factory.CreateTestClient(AuthorizeConstants.User);
        var response = await client.GetAsync($"/projects/members/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
