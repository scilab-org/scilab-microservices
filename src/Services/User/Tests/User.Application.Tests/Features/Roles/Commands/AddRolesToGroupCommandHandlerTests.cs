using User.Application.Features.Roles.Commands;
using User.Application.Tests.Common;

namespace User.Application.Tests.Features.Roles.Commands;

public sealed class AddRolesToGroupCommandHandlerTests : BaseTest
{
    private readonly Mock<IKeycloakService> _keycloakService;
    private readonly AddRolesToGroupCommandHandler _handler;

    public AddRolesToGroupCommandHandlerTests()
    {
        _keycloakService = new Mock<IKeycloakService>();
        _handler = new AddRolesToGroupCommandHandler(_keycloakService.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTrue_WhenRolesAreAddedSuccessfully()
    {
        // Arrange
        const string groupId = "group-id-001";
        var roleNames = new List<string> { "view-data", "edit-data" };
        var command = new AddRolesToGroupCommand(groupId, roleNames);

        _keycloakService
            .Setup(s => s.AddRolesToGroupAsync(groupId, roleNames, CancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Should().BeTrue();
        _keycloakService.Verify(
            s => s.AddRolesToGroupAsync(groupId, roleNames, CancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnTrue_WhenSingleRoleIsAdded()
    {
        // Arrange
        const string groupId = "group-id-002";
        var roleNames = new List<string> { "admin" };
        var command = new AddRolesToGroupCommand(groupId, roleNames);

        _keycloakService
            .Setup(s => s.AddRolesToGroupAsync(groupId, roleNames, CancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldPropagateException_WhenGroupNotFound()
    {
        // Arrange
        const string groupId = "non-existent-group";
        var roleNames = new List<string> { "view-data" };
        var command = new AddRolesToGroupCommand(groupId, roleNames);

        _keycloakService
            .Setup(s => s.AddRolesToGroupAsync(groupId, roleNames, CancellationToken))
            .ThrowsAsync(new InvalidOperationException("Group not found"));

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Group not found");
    }
}
