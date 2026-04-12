using User.Application.Features.Roles.Queries;
using User.Application.Tests.Common;
using User.Application.Tests.Common.TestData;

namespace User.Application.Tests.Features.Roles.Queries;

public sealed class GetGroupRolesQueryHandlerTests : BaseTest
{
    private readonly Mock<IKeycloakService> _keycloakService;
    private readonly GetGroupRolesQueryHandler _handler;

    public GetGroupRolesQueryHandlerTests()
    {
        _keycloakService = new Mock<IKeycloakService>();
        _handler = new GetGroupRolesQueryHandler(_keycloakService.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnRoles_WhenGroupHasRoles()
    {
        // Arrange
        const string groupId = "group-id-001";
        var roles = RoleTestData.CreateRoleDtoList(count: 3);
        var query = new GetGroupRolesQuery(groupId);

        _keycloakService
            .Setup(s => s.GetGroupRolesAsync(groupId, CancellationToken))
            .ReturnsAsync(roles);

        // Act
        var result = await _handler.Handle(query, CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().AllSatisfy(r => r.Id.Should().NotBeNullOrEmpty());
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenGroupHasNoRoles()
    {
        // Arrange
        const string groupId = "group-id-002";
        var query = new GetGroupRolesQuery(groupId);

        _keycloakService
            .Setup(s => s.GetGroupRolesAsync(groupId, CancellationToken))
            .ReturnsAsync(new List<RoleDto>());

        // Act
        var result = await _handler.Handle(query, CancellationToken);

        // Assert
        result.Should().BeEmpty();
    }
}
