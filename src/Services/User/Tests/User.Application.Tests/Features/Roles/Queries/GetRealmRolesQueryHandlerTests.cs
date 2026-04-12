using User.Application.Features.Roles.Queries;
using User.Application.Tests.Common;
using User.Application.Tests.Common.TestData;

namespace User.Application.Tests.Features.Roles.Queries;

public sealed class GetRealmRolesQueryHandlerTests : BaseTest
{
    private readonly Mock<IKeycloakService> _keycloakService;
    private readonly GetRealmRolesQueryHandler _handler;

    public GetRealmRolesQueryHandlerTests()
    {
        _keycloakService = new Mock<IKeycloakService>();
        _handler = new GetRealmRolesQueryHandler(_keycloakService.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllRealmRoles_WhenRolesExist()
    {
        // Arrange
        var roles = RoleTestData.CreateRoleDtoList(count: 5);
        var query = new GetRealmRolesQuery();

        _keycloakService
            .Setup(s => s.GetRealmRolesAsync(CancellationToken))
            .ReturnsAsync(roles);

        // Act
        var result = await _handler.Handle(query, CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(5);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoRealmRolesExist()
    {
        // Arrange
        var query = new GetRealmRolesQuery();

        _keycloakService
            .Setup(s => s.GetRealmRolesAsync(CancellationToken))
            .ReturnsAsync(new List<RoleDto>());

        // Act
        var result = await _handler.Handle(query, CancellationToken);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldCallGetRealmRolesAsyncOnce()
    {
        // Arrange
        var query = new GetRealmRolesQuery();

        _keycloakService
            .Setup(s => s.GetRealmRolesAsync(CancellationToken))
            .ReturnsAsync(new List<RoleDto>());

        // Act
        await _handler.Handle(query, CancellationToken);

        // Assert
        _keycloakService.Verify(s => s.GetRealmRolesAsync(CancellationToken), Times.Once);
    }
}
