using Common.Constants;
using Common.Models;
using Management.Application.Dtos.Members;
using Management.Application.Dtos.Papers;
using Management.Application.Dtos.Projects;
using Management.Application.Features.Project.Commands;
using Management.Application.Features.Project.Queries;
using Management.Application.Models.Results;

namespace Management.Api.Tests.Endpoints;

public sealed class ProjectEndpointTests : IClassFixture<ApiTestFactory>
{
    private readonly ApiTestFactory _factory;

    public ProjectEndpointTests(ApiTestFactory factory)
    {
        _factory = factory;
        factory.SenderMock.Reset();
    }

    [Fact]
    public async Task CreateProject_Returns201()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<CreateProjectCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        var client = _factory.CreateTestClient();
        var response = await client.PostAsJsonAsync("/admin/projects", new CreateProjectDto { Name = "Test" });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task UpdateProject_WhenSystemAdmin_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<UpdateProjectCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        var client = _factory.CreateTestClient(AuthorizeConstants.SystemAdmin);
        var response = await client.PutAsJsonAsync($"/admin/projects/{Guid.NewGuid()}", new UpdateProjectDto { Name = "Updated" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateProject_WhenNotAdmin_Returns403()
    {
        var client = _factory.CreateTestClient(AuthorizeConstants.ProjectManager);
        var response = await client.PutAsJsonAsync($"/admin/projects/{Guid.NewGuid()}", new UpdateProjectDto { Name = "X" });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateProject_WhenNoAuth_Returns401()
    {
        var client = _factory.CreateUnauthenticatedClient();
        var response = await client.PutAsJsonAsync($"/admin/projects/{Guid.NewGuid()}", new UpdateProjectDto { Name = "X" });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteProject_WhenSystemAdmin_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<DeleteProjectCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var client = _factory.CreateTestClient(AuthorizeConstants.SystemAdmin);
        var response = await client.DeleteAsync($"/admin/projects/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteProject_WhenNotAdmin_Returns403()
    {
        var client = _factory.CreateTestClient(AuthorizeConstants.ProjectManager);
        var response = await client.DeleteAsync($"/admin/projects/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteProject_WhenNoAuth_Returns401()
    {
        var client = _factory.CreateUnauthenticatedClient();
        var response = await client.DeleteAsync($"/admin/projects/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetProjects_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<GetProjectsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetProjectsResult(new List<ProjectDto>(), 0, new PaginationRequest()));

        var client = _factory.CreateTestClient();
        var response = await client.GetAsync("/admin/projects");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProjectById_WhenAuthenticated_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<GetProjectByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetProjectByIdResult(new ProjectDto()));

        var client = _factory.CreateTestClient(AuthorizeConstants.User);
        var response = await client.GetAsync($"/projects/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProjectById_WhenNoAuth_Returns403()
    {
        var client = _factory.CreateUnauthenticatedClient();
        var response = await client.GetAsync($"/projects/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetMyProjects_WhenAuthenticated_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<GetMyProjectsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetProjectsResult(new List<ProjectDto>(), 0, new PaginationRequest()));

        var client = _factory.CreateTestClient(AuthorizeConstants.User);
        var response = await client.GetAsync("/projects/me");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetMyProjects_WhenNoAuth_Returns401()
    {
        var client = _factory.CreateUnauthenticatedClient();
        var response = await client.GetAsync("/projects/me");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMyProjectRole_WhenAuthenticated_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<GetMyProjectRoleQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("ProjectManager");

        var client = _factory.CreateTestClient(AuthorizeConstants.User);
        var response = await client.GetAsync($"/projects/{Guid.NewGuid()}/my-role");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetMyProjectRole_WhenNoAuth_Returns401()
    {
        var client = _factory.CreateUnauthenticatedClient();
        var response = await client.GetAsync($"/projects/{Guid.NewGuid()}/my-role");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetProjectsByUserId_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<GetProjectsByUserIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetProjectsResult(new List<ProjectDto>(), 0, new PaginationRequest()));

        var client = _factory.CreateTestClient();
        var response = await client.GetAsync($"/projects/users/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAvailableProjectUsers_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<GetAvailableProjectUsersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetAvailableProjectUsersResult(new List<UserInfoDto>()));

        var client = _factory.CreateTestClient();
        var response = await client.GetAsync($"/projects/{Guid.NewGuid()}/users/available");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAssignedPapers_WhenAuthenticated_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<GetAssignedPapersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetAssignedPapersResult(new List<AssignedPaperDto>(), 0, new PaginationRequest()));

        var client = _factory.CreateTestClient(AuthorizeConstants.User);
        var response = await client.GetAsync("/projects/me/assigned-papers");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAssignedPapers_WhenNoAuth_Returns401()
    {
        var client = _factory.CreateUnauthenticatedClient();
        var response = await client.GetAsync("/projects/me/assigned-papers");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task InitializeData_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<Management.Application.Features.System.InitialDataCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var client = _factory.CreateTestClient();
        var response = await client.PostAsync("/admin/system/initialize-data", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
