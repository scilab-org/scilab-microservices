using Microsoft.Extensions.Logging;
using User.Application.Features.Users;
using User.Application.Tests.Common;
using User.Application.Tests.Common.TestData;

namespace User.Application.Tests.Features.Users.Commands;

public sealed class UpdateUserCommandHandlerTests : BaseTest
{
    private readonly Mock<IKeycloakService> _keycloakService;
    private readonly Mock<IMinIoCloudService> _minIoCloudService;
    private readonly UpdateUserCommandHandler _handler;

    public UpdateUserCommandHandlerTests()
    {
        _keycloakService = new Mock<IKeycloakService>();
        _minIoCloudService = new Mock<IMinIoCloudService>();
        _handler = new UpdateUserCommandHandler(
            _keycloakService.Object,
            _minIoCloudService.Object,
            CreateLogger<UpdateUserCommandHandler>());
    }

    [Fact]
    public async Task Handle_ShouldReturnTrue_WhenUpdateSucceedsWithoutNewAvatar()
    {
        // Arrange
        const string userId = "user-id-001";
        var dto = UserTestData.UpdateUserDtoRequest(avatarImage: null);
        var command = new UpdateUserCommand(userId, dto, UserTestData.UserActor(userId));

        _keycloakService
            .Setup(s => s.UpdateUserAsync(
                userId,
                dto.FirstName,
                dto.LastName,
                dto.OcrId,
                dto.Enabled,
                dto.GroupNames,
                null,
                CancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Should().BeTrue();
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
    public async Task Handle_ShouldUploadNewAvatarAndReturnTrue_WhenAvatarIsProvided()
    {
        // Arrange
        const string userId = "user-id-002";
        const string avatarPublicUrl = "https://storage.example.com/user-avatars/user-id-002.png";
        var avatar = UserTestData.CreateAvatarBytes();
        var dto = UserTestData.UpdateUserDtoRequest(avatarImage: avatar);
        var command = new UpdateUserCommand(userId, dto, UserTestData.UserActor(userId));

        var uploadResult = new UploadFileResult { PublicURL = avatarPublicUrl };

        _minIoCloudService
            .Setup(s => s.UploadFileAsync(
                avatar,
                AppConstants.Bucket.UserAvatars,
                $"{userId}.png",
                true,
                CancellationToken))
            .ReturnsAsync(uploadResult);

        _keycloakService
            .Setup(s => s.UpdateUserAsync(
                userId,
                dto.FirstName,
                dto.LastName,
                dto.OcrId,
                dto.Enabled,
                dto.GroupNames,
                avatarPublicUrl,
                CancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Should().BeTrue();
        _minIoCloudService.Verify(
            s => s.UploadFileAsync(
                avatar,
                AppConstants.Bucket.UserAvatars,
                $"{userId}.png",
                true,
                CancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFallBackToDefaultAvatar_WhenAvatarUploadThrows()
    {
        // Arrange
        const string userId = "user-id-003";
        var avatar = UserTestData.CreateAvatarBytes();
        var dto = UserTestData.UpdateUserDtoRequest(avatarImage: avatar);
        var command = new UpdateUserCommand(userId, dto, UserTestData.UserActor(userId));

        _minIoCloudService
            .Setup(s => s.UploadFileAsync(
                It.IsAny<UploadFileBytes>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Storage unavailable"));

        _keycloakService
            .Setup(s => s.UpdateUserAsync(
                userId,
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<bool?>(),
                It.IsAny<List<string>?>(),
                AppConstants.Bucket.DefaultAvatar,
                CancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Should().BeTrue();
        _keycloakService.Verify(
            s => s.UpdateUserAsync(
                userId,
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<bool?>(),
                It.IsAny<List<string>?>(),
                AppConstants.Bucket.DefaultAvatar,
                CancellationToken),
            Times.Once);
    }
}
