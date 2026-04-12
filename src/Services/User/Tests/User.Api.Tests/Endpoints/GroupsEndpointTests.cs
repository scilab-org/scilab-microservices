using Common.Constants;
using User.Application.Dtos.Groups;
using User.Application.Dtos.Roles;
using User.Application.Features.Groups.Queries;
using User.Application.Features.Roles.Queries;

namespace User.Api.Tests.Endpoints;

public sealed class GroupsEndpointTests : IClassFixture<ApiTestFactory>
{
    private readonly ApiTestFactory _factory;

    public GroupsEndpointTests(ApiTestFactory factory)
    {
        _factory = factory;
        factory.SenderMock.Reset();
    }

    // ==========================================
    // GET /groups — GetGroups
    // ==========================================

    [Fact]
    public async Task GetGroups_AnyAuthenticatedUser_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<GetGroupsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<GroupDto>
            {
                new() { Id = "g1", Name = "system:admin" },
                new() { Id = "g2", Name = "app:user" }
            });

        var client = _factory.CreateTestClient("app:user");

        var response = await client.GetAsync("/groups");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _factory.SenderMock.Verify(
            s => s.Send(It.IsAny<GetGroupsQuery>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // ==========================================
    // GET /groups/{groupId}/roles — GetGroupRoles
    // ==========================================

    [Fact]
    public async Task GetGroupRoles_WhenSystemAdmin_Returns200()
    {
        _factory.SenderMock
            .Setup(s => s.Send(It.IsAny<GetGroupRolesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RoleDto> { new() { Id = "r1", Name = "view-users" } });

        var client = _factory.CreateTestClient(AuthorizeConstants.SystemAdmin);

        var response = await client.GetAsync("/groups/group-123/roles");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _factory.SenderMock.Verify(
            s => s.Send(
                It.Is<GetGroupRolesQuery>(q => q.GroupId == "group-123"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetGroupRoles_WhenNotSystemAdmin_ReturnsError()
    {
        var client = _factory.CreateTestClient("app:user");

        var response = await client.GetAsync("/groups/group-123/roles");

        response.IsSuccessStatusCode.Should().BeFalse();
        _factory.SenderMock.Verify(
            s => s.Send(It.IsAny<GetGroupRolesQuery>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
