using User.Application.Features.Users.Queries;
using User.Application.Tests.Common;
using User.Application.Tests.Common.TestData;

namespace User.Application.Tests.Features.Users.Queries;

public sealed class GetUserByIdQueryHandlerTests : BaseTest
{
    private readonly Mock<IKeycloakService> _keycloakService;
    private readonly GetUserByIdQueryHandler _handler;

    public GetUserByIdQueryHandlerTests()
    {
        _keycloakService = new Mock<IKeycloakService>();
        _handler = new GetUserByIdQueryHandler(_keycloakService.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnUserResult_WhenUserExists()
    {
        // Arrange
        const string userId = "user-id-001";
        var userDto = UserTestData.CreateUserDto(id: userId, username: "jdoe", email: "jdoe@example.com");
        var query = new GetUserByIdQuery(userId);

        _keycloakService
            .Setup(s => s.GetUserByIdAsync(userId, CancellationToken))
            .ReturnsAsync(userDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.User.Should().NotBeNull();
        result.User.Id.Should().Be(userId);
        result.User.Username.Should().Be("jdoe");
        result.User.Email.Should().Be("jdoe@example.com");
    }

    [Fact]
    public async Task Handle_ShouldPropagateException_WhenUserNotFound()
    {
        // Arrange
        const string userId = "non-existent-user";
        var query = new GetUserByIdQuery(userId);

        _keycloakService
            .Setup(s => s.GetUserByIdAsync(userId, CancellationToken))
            .ThrowsAsync(new KeyNotFoundException($"User '{userId}' not found"));

        // Act
        var act = async () => await _handler.Handle(query, CancellationToken);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldCallKeycloakServiceOnce_WithCorrectUserId()
    {
        // Arrange
        const string userId = "user-id-002";
        var userDto = UserTestData.CreateUserDto(id: userId);
        var query = new GetUserByIdQuery(userId);

        _keycloakService
            .Setup(s => s.GetUserByIdAsync(userId, CancellationToken))
            .ReturnsAsync(userDto);

        // Act
        await _handler.Handle(query, CancellationToken);

        // Assert
        _keycloakService.Verify(s => s.GetUserByIdAsync(userId, CancellationToken), Times.Once);
    }
}
