using Microsoft.Extensions.Logging;
using User.Application.Features.Users;
using User.Application.Tests.Common;
using User.Application.Tests.Common.TestData;

namespace User.Application.Tests.Features.Users.Commands;

public sealed class UpdateUserGroupsCommandHandlerTests : BaseTest
{
    private readonly Mock<IKeycloakService> _keycloakService;
    private readonly UpdateUserGroupsCommandHandler _handler;

    public UpdateUserGroupsCommandHandlerTests()
    {
        _keycloakService = new Mock<IKeycloakService>();
        _handler = new UpdateUserGroupsCommandHandler(
            _keycloakService.Object,
            CreateLogger<UpdateUserGroupsCommandHandler>());
    }

    [Fact]
    public async Task Handle_ShouldReturnTrue_WhenGroupsAreUpdatedSuccessfully()
    {
        // Arrange
        const string userId = "user-id-001";
        var groupNames = new List<string> { "Researchers", "Admins" };
        var command = new UpdateUserGroupsCommand(userId, groupNames, UserTestData.UserActor(userId));

        _keycloakService
            .Setup(s => s.UpdateUserGroupsAsync(userId, groupNames, CancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Should().BeTrue();
        _keycloakService.Verify(
            s => s.UpdateUserGroupsAsync(userId, groupNames, CancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnTrue_WhenGroupNamesIsEmptyList()
    {
        // Arrange
        const string userId = "user-id-002";
        var groupNames = new List<string>();
        var command = new UpdateUserGroupsCommand(userId, groupNames, UserTestData.UserActor(userId));

        _keycloakService
            .Setup(s => s.UpdateUserGroupsAsync(userId, groupNames, CancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Should().BeTrue();
        _keycloakService.Verify(
            s => s.UpdateUserGroupsAsync(userId, It.Is<List<string>>(l => l.Count == 0), CancellationToken),
            Times.Once);
    }
}
