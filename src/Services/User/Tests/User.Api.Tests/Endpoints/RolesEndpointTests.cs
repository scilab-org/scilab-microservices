using Common.Constants;
using User.Application.Dtos.Roles;
using User.Application.Features.Roles.Commands;
using User.Application.Features.Roles.Queries;

namespace User.Api.Tests.Endpoints;

public sealed class RolesEndpointTests : IClassFixture<ApiTestFactory>
{
    private readonly ApiTestFactory _factory;

    public RolesEndpointTests(ApiTestFactory factory)
    {
        _factory = factory;
        factory.SenderMock.Reset();
    }

    // ==========================================
    // GET /roles — GetRealmRoles
    // ==========================================

    [Fact]
    public async Task GetRealmRoles_WhenSystemAdmin_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<GetRealmRolesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RoleDto> { new() { Id = "r1", Name = "manage-users" } });

        var client = _factory.CreateTestClient(AuthorizeConstants.SystemAdmin);

        var response = await client.GetAsync("/roles");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _factory.SenderMock.Verify(
            s => s.Send(It.IsAny<GetRealmRolesQuery>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetRealmRoles_WhenNotSystemAdmin_ReturnsError()
    {
        var client = _factory.CreateTestClient("app:user");

        var response = await client.GetAsync("/roles");

        response.IsSuccessStatusCode.Should().BeFalse();
        _factory.SenderMock.Verify(
            s => s.Send(It.IsAny<GetRealmRolesQuery>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ==========================================
    // POST /groups/{groupId}/roles — AddRolesToGroup
    // ==========================================

    [Fact]
    public async Task AddRolesToGroup_WhenSystemAdmin_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<AddRolesToGroupCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var client = _factory.CreateTestClient(AuthorizeConstants.SystemAdmin);
        var body = new StringContent(
            JsonSerializer.Serialize(new[] { "manage-users", "view-users" }),
            Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/groups/group-123/roles", body);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _factory.SenderMock.Verify(
            s => s.Send(
                It.Is<AddRolesToGroupCommand>(cmd =>
                    cmd.GroupId == "group-123" && cmd.RoleNames.Count == 2),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task AddRolesToGroup_WhenNotSystemAdmin_ReturnsError()
    {
        var client = _factory.CreateTestClient("app:user");
        var body = new StringContent(
            JsonSerializer.Serialize(new[] { "manage-users" }),
            Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/groups/group-123/roles", body);

        response.IsSuccessStatusCode.Should().BeFalse();
        _factory.SenderMock.Verify(
            s => s.Send(It.IsAny<AddRolesToGroupCommand>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ==========================================
    // DELETE /groups/{groupId}/roles — RemoveRolesFromGroup
    // ==========================================

    [Fact]
    public async Task RemoveRolesFromGroup_WhenSystemAdmin_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<RemoveRolesFromGroupCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var client = _factory.CreateTestClient(AuthorizeConstants.SystemAdmin);
        var body = new StringContent(
            JsonSerializer.Serialize(new[] { "manage-users" }),
            Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Delete, "/groups/group-123/roles")
        {
            Content = body
        };
        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _factory.SenderMock.Verify(
            s => s.Send(
                It.Is<RemoveRolesFromGroupCommand>(cmd =>
                    cmd.GroupId == "group-123" && cmd.RoleNames.Count == 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RemoveRolesFromGroup_WhenNotSystemAdmin_ReturnsError()
    {
        var client = _factory.CreateTestClient("app:user");
        var body = new StringContent(
            JsonSerializer.Serialize(new[] { "manage-users" }),
            Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Delete, "/groups/group-123/roles")
        {
            Content = body
        };
        var response = await client.SendAsync(request);

        response.IsSuccessStatusCode.Should().BeFalse();
        _factory.SenderMock.Verify(
            s => s.Send(It.IsAny<RemoveRolesFromGroupCommand>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
