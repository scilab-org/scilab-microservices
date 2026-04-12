using User.Application.Features.Users;
using User.Application.Tests.Common;
using User.Application.Tests.Common.TestData;

namespace User.Application.Tests.Features.Users.Commands;

public sealed class ActivateUserCommandHandlerTests : BaseTest
{
    private readonly Mock<IKeycloakService> _keycloakService;
    private readonly ActivateUserCommandHandler _handler;

    public ActivateUserCommandHandlerTests()
    {
        _keycloakService = new Mock<IKeycloakService>();
        _handler = new ActivateUserCommandHandler(_keycloakService.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTrue_WhenUserIsActivatedSuccessfully()
    {
        // Arrange
        const string userId = "user-id-001";
        var command = new ActivateUserCommand(userId, UserTestData.SystemActor());

        _keycloakService
            .Setup(s => s.ActivateUserAsync(userId, CancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Should().BeTrue();
        _keycloakService.Verify(s => s.ActivateUserAsync(userId, CancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldPropagateException_WhenKeycloakServiceThrows()
    {
        // Arrange
        const string userId = "user-id-002";
        var command = new ActivateUserCommand(userId, UserTestData.SystemActor());

        _keycloakService
            .Setup(s => s.ActivateUserAsync(userId, CancellationToken))
            .ThrowsAsync(new InvalidOperationException("Keycloak error"));

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Keycloak error");
    }
}
