using User.Application.Features.Roles.Commands;
using User.Application.Tests.Common;

namespace User.Application.Tests.Features.Roles.Commands;

public sealed class RemoveRolesFromGroupCommandHandlerTests : BaseTest
{
    private readonly Mock<IKeycloakService> _keycloakService;
    private readonly RemoveRolesFromGroupCommandHandler _handler;

    public RemoveRolesFromGroupCommandHandlerTests()
    {
        _keycloakService = new Mock<IKeycloakService>();
        _handler = new RemoveRolesFromGroupCommandHandler(_keycloakService.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTrue_WhenRolesAreRemovedSuccessfully()
    {
        // Arrange
        const string groupId = "group-id-001";
        var roleNames = new List<string> { "view-data", "edit-data" };
        var command = new RemoveRolesFromGroupCommand(groupId, roleNames);

        _keycloakService
            .Setup(s => s.RemoveRolesFromGroupAsync(groupId, roleNames, CancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Should().BeTrue();
        _keycloakService.Verify(
            s => s.RemoveRolesFromGroupAsync(groupId, roleNames, CancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldPropagateException_WhenRoleNameNotFound()
    {
        // Arrange
        const string groupId = "group-id-002";
        var roleNames = new List<string> { "non-existent-role" };
        var command = new RemoveRolesFromGroupCommand(groupId, roleNames);

        _keycloakService
            .Setup(s => s.RemoveRolesFromGroupAsync(groupId, roleNames, CancellationToken))
            .ThrowsAsync(new InvalidOperationException("Role does not exist"));

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Role does not exist");
    }
}
