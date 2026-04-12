using Common.Constants;
using Common.Models;
using User.Application.Dtos.Users;
using User.Application.Features.Users;
using User.Application.Features.Users.Queries;
using User.Application.Models.Results;

namespace User.Api.Tests.Endpoints;

public sealed class UsersEndpointTests : IClassFixture<ApiTestFactory>
{
    private readonly ApiTestFactory _factory;

    public UsersEndpointTests(ApiTestFactory factory)
    {
        _factory = factory;
        factory.SenderMock.Reset();
    }

    // ==========================================
    // POST /users — CreateUser
    // ==========================================

    [Fact]
    public async Task CreateUser_WhenSystemAdmin_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("new-user-id");

        var client = _factory.CreateTestClient(AuthorizeConstants.SystemAdmin);
        var form = BuildCreateUserForm();

        var response = await client.PostAsync("/users", form);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _factory.SenderMock.Verify(
            s => s.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateUser_WhenNotSystemAdmin_ReturnsError()
    {
        var client = _factory.CreateTestClient(AuthorizeConstants.ProjectManager);
        var form = BuildCreateUserForm();

        var response = await client.PostAsync("/users", form);

        response.IsSuccessStatusCode.Should().BeFalse();
        _factory.SenderMock.Verify(
            s => s.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateUser_WhenNoGroupClaims_ReturnsError()
    {
        var client = _factory.CreateTestClient(); // no groups
        var form = BuildCreateUserForm();

        var response = await client.PostAsync("/users", form);

        response.IsSuccessStatusCode.Should().BeFalse();
    }

    // ==========================================
    // GET /users — GetUsers
    // ==========================================

    [Fact]
    public async Task GetUsers_Always_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<GetUsersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUsersResult([], 0, new PaginationRequest()));

        var client = _factory.CreateTestClient("app:user");

        var response = await client.GetAsync("/users");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _factory.SenderMock.Verify(
            s => s.Send(It.IsAny<GetUsersQuery>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetUsers_WhenProjectManager_SendsQueryExcludingSystemAdminGroup()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<GetUsersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUsersResult([], 0, new PaginationRequest()));

        var client = _factory.CreateTestClient(AuthorizeConstants.ProjectManager);
        await client.GetAsync("/users");

        _factory.SenderMock.Verify(
            s => s.Send(
                It.Is<GetUsersQuery>(q => q.ExcludeAdminGroupName == AuthorizeConstants.SystemAdmin),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetUsers_WhenSystemAdmin_SendsQueryWithNullExcludeAdminGroupName()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<GetUsersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUsersResult([], 0, new PaginationRequest()));

        var client = _factory.CreateTestClient(AuthorizeConstants.SystemAdmin);
        await client.GetAsync("/users");

        _factory.SenderMock.Verify(
            s => s.Send(
                It.Is<GetUsersQuery>(q => q.ExcludeAdminGroupName == null),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetUsers_ForwardsQueryParameters()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<GetUsersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUsersResult([], 0, new PaginationRequest()));

        var client = _factory.CreateTestClient("app:user");
        await client.GetAsync("/users?searchText=john&pageNumber=2&pageSize=20");

        _factory.SenderMock.Verify(
            s => s.Send(
                It.Is<GetUsersQuery>(q =>
                    q.Filter.SearchText == "john" &&
                    q.Paging.PageNumber == 2 &&
                    q.Paging.PageSize == 20),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // ==========================================
    // GET /users/{userId} — GetUserById
    // ==========================================

    [Fact]
    public async Task GetUserById_Always_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<GetUserByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserByIdResult(new UserDto { Id = "user-123" }));

        var client = _factory.CreateTestClient("app:user");

        var response = await client.GetAsync("/users/user-123");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _factory.SenderMock.Verify(
            s => s.Send(
                It.Is<GetUserByIdQuery>(q => q.UserId == "user-123"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // ==========================================
    // PUT /users/{userId}/activate — ActivateUser
    // ==========================================

    [Fact]
    public async Task ActivateUser_WhenSystemAdmin_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<ActivateUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var client = _factory.CreateTestClient(AuthorizeConstants.SystemAdmin);

        var response = await client.PutAsync("/users/user-123/activate", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _factory.SenderMock.Verify(
            s => s.Send(
                It.Is<ActivateUserCommand>(cmd => cmd.UserId == "user-123"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ActivateUser_WhenNotSystemAdmin_ReturnsError()
    {
        var client = _factory.CreateTestClient("app:user");

        var response = await client.PutAsync("/users/user-123/activate", null);

        response.IsSuccessStatusCode.Should().BeFalse();
        _factory.SenderMock.Verify(
            s => s.Send(It.IsAny<ActivateUserCommand>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ==========================================
    // DELETE /users/{userId}/deactivate — DeactivateUser
    // ==========================================

    [Fact]
    public async Task DeactivateUser_WhenSystemAdmin_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<DeactivateUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var client = _factory.CreateTestClient(AuthorizeConstants.SystemAdmin);

        var response = await client.DeleteAsync("/users/user-123/deactivate");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _factory.SenderMock.Verify(
            s => s.Send(
                It.Is<DeactivateUserCommand>(cmd => cmd.UserId == "user-123"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeactivateUser_WhenNotSystemAdmin_ReturnsError()
    {
        var client = _factory.CreateTestClient("app:user");

        var response = await client.DeleteAsync("/users/user-123/deactivate");

        response.IsSuccessStatusCode.Should().BeFalse();
        _factory.SenderMock.Verify(
            s => s.Send(It.IsAny<DeactivateUserCommand>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ==========================================
    // PUT /users/{userId} — UpdateUser
    // ==========================================

    [Theory]
    [InlineData("system:admin")]
    [InlineData("project:project-manager")]
    [InlineData("project:author")]
    public async Task UpdateUser_WhenAuthorizedRole_Returns200(string group)
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var client = _factory.CreateTestClient(group);
        var form = new MultipartFormDataContent();
        form.Add(new StringContent("John"), "FirstName");
        form.Add(new StringContent("Doe"), "LastName");
        form.Add(new StringContent("true"), "Enabled");

        var response = await client.PutAsync("/users/user-123", form);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _factory.SenderMock.Verify(
            s => s.Send(
                It.Is<UpdateUserCommand>(cmd => cmd.UserId == "user-123"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateUser_WhenNoAuthorizedRole_ReturnsError()
    {
        var client = _factory.CreateTestClient("app:user");
        var form = new MultipartFormDataContent();
        form.Add(new StringContent("John"), "FirstName");

        var response = await client.PutAsync("/users/user-123", form);

        response.IsSuccessStatusCode.Should().BeFalse();
        _factory.SenderMock.Verify(
            s => s.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ==========================================
    // PUT /users/{userId}/groups — UpdateUserGroups
    // ==========================================

    [Fact]
    public async Task UpdateUserGroups_Always_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<UpdateUserGroupsCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var client = _factory.CreateTestClient("app:user");
        var body = new StringContent(
            JsonSerializer.Serialize(new { GroupNames = new[] { "group1", "group2" } }),
            Encoding.UTF8, "application/json");

        var response = await client.PutAsync("/users/user-123/groups", body);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _factory.SenderMock.Verify(
            s => s.Send(
                It.Is<UpdateUserGroupsCommand>(cmd =>
                    cmd.UserId == "user-123" && cmd.GroupNames.Count == 2),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // ==========================================
    // Helpers
    // ==========================================

    private static MultipartFormDataContent BuildCreateUserForm() =>
        new()
        {
            { new StringContent("johndoe"), "Username" },
            { new StringContent("john@example.com"), "Email" },
            { new StringContent("John"), "FirstName" },
            { new StringContent("Doe"), "LastName" },
            { new StringContent("SecurePass123!"), "InitialPassword" },
            { new StringContent("true"), "TemporaryPassword" }
        };
}
