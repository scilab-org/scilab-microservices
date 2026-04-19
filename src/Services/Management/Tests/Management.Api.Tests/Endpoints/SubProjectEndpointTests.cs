using Common.Constants;
using Common.Models;
using Management.Application.Dtos.Members;
using Management.Application.Dtos.Papers;
using Management.Application.Dtos.Projects;
using Management.Application.Features.Member.Commands;
using Management.Application.Features.Member.Queries;
using Management.Application.Features.Project.Commands;
using Management.Application.Features.Project.Queries;
using Management.Application.Models.Results;

namespace Management.Api.Tests.Endpoints;

public sealed class SubProjectEndpointTests : IClassFixture<ApiTestFactory>
{
    private readonly ApiTestFactory _factory;

    public SubProjectEndpointTests(ApiTestFactory factory)
    {
        _factory = factory;
        factory.SenderMock.Reset();
    }

    [Fact]
    public async Task CreateSubProject_WhenAuthenticated_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<CreateSubProjectCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        var client = _factory.CreateTestClient(AuthorizeConstants.User);
        var response = await client.PostAsJsonAsync($"/projects/{Guid.NewGuid()}/sub-projects", new CreateSubProjectDto { Name = "Sub" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateSubProject_WhenNoAuth_Returns403()
    {
        var client = _factory.CreateUnauthenticatedClient();
        var response = await client.PostAsJsonAsync($"/projects/{Guid.NewGuid()}/sub-projects", new CreateSubProjectDto { Name = "Sub" });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetSubProjects_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<GetSubProjectsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetSubProjectsPapersResult(new List<PaperInfoDto>(), 0, new PaginationRequest()));

        var client = _factory.CreateTestClient();
        var response = await client.GetAsync($"/projects/{Guid.NewGuid()}/sub-projects");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AddSubProjectMembers_WhenAuthenticated_Returns201()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<AddSubProjectMembersCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Guid> { Guid.NewGuid() });

        var client = _factory.CreateTestClient(AuthorizeConstants.User);
        var body = new AddProjectMembersDto { Members = new List<ProjectMemberEntry> { new() { UserId = Guid.NewGuid() } } };
        var response = await client.PostAsJsonAsync($"/sub-projects/{Guid.NewGuid()}/members", body);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task DeleteSubProjectMembers_WhenAuthenticated_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<DeleteSubProjectMembersCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Guid> { Guid.NewGuid() });

        var client = _factory.CreateTestClient(AuthorizeConstants.ProjectManager);
        var response = await client.PostAsJsonAsync($"/manager/sub-projects/{Guid.NewGuid()}/members/remove", new DeleteProjectMembersDto { MemberIds = new List<Guid> { Guid.NewGuid() } });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteSubProjectMembers_WhenNoAuth_Returns401()
    {
        var client = _factory.CreateUnauthenticatedClient();
        var response = await client.PostAsJsonAsync($"/manager/sub-projects/{Guid.NewGuid()}/members/remove", new DeleteProjectMembersDto { MemberIds = new List<Guid> { Guid.NewGuid() } });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteSubProjectPaper_WhenAuthenticated_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<DeleteSubProjectCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var client = _factory.CreateTestClient(AuthorizeConstants.ProjectManager);
        var response = await client.DeleteAsync($"/manager/sub-projects/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteSubProjectPaper_WhenNoAuth_Returns403()
    {
        var client = _factory.CreateUnauthenticatedClient();
        var response = await client.DeleteAsync($"/manager/sub-projects/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetSubProjectMembers_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<GetSubProjectMembersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetProjectMembersResult(new List<ProjectMemberDto>(), 0, new PaginationRequest()));

        var client = _factory.CreateTestClient();
        var response = await client.GetAsync($"/sub-projects/{Guid.NewGuid()}/members");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAvailableSubProjectMembers_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<GetAvailableSubProjectMembersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetProjectMembersResult(new List<ProjectMemberDto>(), 0, new PaginationRequest()));

        var client = _factory.CreateTestClient();
        var response = await client.GetAsync($"/sub-projects/{Guid.NewGuid()}/members/available");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetMemberByPaperId_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<GetMemberByPaperIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProjectMemberDto());

        var client = _factory.CreateTestClient();
        var response = await client.GetAsync($"/sub-projects/papers/{Guid.NewGuid()}/member?userId={Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetSubProjectMembersByPaperId_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<GetSubProjectMembersByPaperIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetSubProjectMembersByPaperIdResult(Guid.NewGuid(), new List<SubProjectMemberItemDto>()));

        var client = _factory.CreateTestClient();
        var response = await client.GetAsync($"/sub-projects/papers/{Guid.NewGuid()}/members");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
