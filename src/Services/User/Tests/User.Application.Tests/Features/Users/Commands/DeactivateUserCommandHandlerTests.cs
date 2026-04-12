using User.Application.Features.Users;
using User.Application.Tests.Common;
using User.Application.Tests.Common.TestData;

namespace User.Application.Tests.Features.Users.Commands;

public sealed class DeactivateUserCommandHandlerTests : BaseTest
{
    private readonly Mock<IKeycloakService> _keycloakService;
    private readonly DeactivateUserCommandHandler _handler;

    public DeactivateUserCommandHandlerTests()
    {
        _keycloakService = new Mock<IKeycloakService>();
        _handler = new DeactivateUserCommandHandler(_keycloakService.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTrue_WhenUserIsDeactivatedSuccessfully()
    {
        // Arrange
        const string userId = "user-id-001";
        var command = new DeactivateUserCommand(userId, UserTestData.SystemActor());

        _keycloakService
            .Setup(s => s.DeactivateUserAsync(userId, CancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Should().BeTrue();
        _keycloakService.Verify(s => s.DeactivateUserAsync(userId, CancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldPropagateException_WhenKeycloakServiceThrows()
    {
        // Arrange
        const string userId = "user-id-002";
        var command = new DeactivateUserCommand(userId, UserTestData.SystemActor());

        _keycloakService
            .Setup(s => s.DeactivateUserAsync(userId, CancellationToken))
            .ThrowsAsync(new InvalidOperationException("User not found in Keycloak"));

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("User not found in Keycloak");
    }
}
