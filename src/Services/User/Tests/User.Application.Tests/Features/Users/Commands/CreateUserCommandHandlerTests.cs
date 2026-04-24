using Microsoft.Extensions.Logging;
using User.Application.Features.Users;
using User.Application.Tests.Common;
using User.Application.Tests.Common.TestData;

namespace User.Application.Tests.Features.Users.Commands;

public sealed class CreateUserCommandHandlerTests : BaseTest
{
    private readonly Mock<IKeycloakService> _keycloakService;
    private readonly Mock<IMinIoCloudService> _minIoCloudService;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _keycloakService = new Mock<IKeycloakService>();
        _minIoCloudService = new Mock<IMinIoCloudService>();
        _handler = new CreateUserCommandHandler(
            _keycloakService.Object,
            _minIoCloudService.Object,
            CreateLogger<CreateUserCommandHandler>());
    }

    [Fact]
    public async Task Handle_ShouldReturnKeycloakUserId_WhenNoAvatarProvided()
    {
        // Arrange
        const string expectedUserId = "kc-user-abc-123";
        var dto = UserTestData.CreateUserDtoRequest(avatarImage: null);
        var command = new CreateUserCommand(dto, UserTestData.SystemActor());

        _keycloakService
            .Setup(s => s.CreateUserAsync(
                dto.Username,
                dto.Email,
                dto.FirstName,
                dto.LastName,
                dto.OcrId,
                dto.InitialPassword,
                dto.TemporaryPassword,
                dto.GroupNames,
                AppConstants.Bucket.DefaultAvatar,
                CancellationToken))
            .ReturnsAsync(expectedUserId);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Should().Be(expectedUserId);
        _minIoCloudService.Verify(
            s => s.UploadFileAsync(
                It.IsAny<UploadFileBytes>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldUploadAvatarAndReturnKeycloakUserId_WhenAvatarProvided()
    {
        // Arrange
        const string expectedUserId = "kc-user-abc-456";
        const string avatarPublicUrl = "https://storage.example.com/user-avatars/jdoe.png";

        var avatar = UserTestData.CreateAvatarBytes();
        var dto = UserTestData.CreateUserDtoRequest(username: "jdoe", avatarImage: avatar);
        var command = new CreateUserCommand(dto, UserTestData.SystemActor());

        var uploadResult = new UploadFileResult { PublicURL = avatarPublicUrl };

        _minIoCloudService
            .Setup(s => s.UploadFileAsync(
                avatar,
                AppConstants.Bucket.UserAvatars,
                "jdoe.png",
                true,
                CancellationToken))
            .ReturnsAsync(uploadResult);

        _keycloakService
            .Setup(s => s.CreateUserAsync(
                dto.Username,
                dto.Email,
                dto.FirstName,
                dto.LastName,
                dto.OcrId,
                dto.InitialPassword,
                dto.TemporaryPassword,
                dto.GroupNames,
                avatarPublicUrl,
                CancellationToken))
            .ReturnsAsync(expectedUserId);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Should().Be(expectedUserId);
        _minIoCloudService.Verify(
            s => s.UploadFileAsync(
                avatar,
                AppConstants.Bucket.UserAvatars,
                "jdoe.png",
                true,
                CancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFallBackToDefaultAvatar_WhenAvatarUploadFails()
    {
        // Arrange
        const string expectedUserId = "kc-user-abc-789";
        var avatar = UserTestData.CreateAvatarBytes();
        var dto = UserTestData.CreateUserDtoRequest(username: "jdoe", avatarImage: avatar);
        var command = new CreateUserCommand(dto, UserTestData.SystemActor());

        _minIoCloudService
            .Setup(s => s.UploadFileAsync(
                It.IsAny<UploadFileBytes>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("MinIO unreachable"));

        _keycloakService
            .Setup(s => s.CreateUserAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<List<string>>(),
                AppConstants.Bucket.DefaultAvatar,
                CancellationToken))
            .ReturnsAsync(expectedUserId);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Should().Be(expectedUserId);
        _keycloakService.Verify(
            s => s.CreateUserAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<List<string>>(),
                AppConstants.Bucket.DefaultAvatar,
                CancellationToken),
            Times.Once);
    }
}
