#region using

using Microsoft.Extensions.Logging;
using User.Application.Dtos.Users;
using User.Application.Services;

#endregion

namespace User.Application.Features.Users;

public sealed record CreateUserCommand(CreateUserDto Dto, Actor Actor) : ICommand<string>;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    #region Ctors

    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest)
            .DependentRules(() =>
            {
                RuleFor(x => x.Dto.Username)
                    .NotEmpty()
                    .WithMessage(MessageCode.UsernameIsRequired)
                    .MinimumLength(3)
                    .WithMessage(MessageCode.BadRequest)
                    .MaximumLength(50)
                    .WithMessage(MessageCode.BadRequest);

                RuleFor(x => x.Dto.Email)
                    .NotEmpty()
                    .WithMessage(MessageCode.EmailIsRequired)
                    .EmailAddress()
                    .WithMessage(MessageCode.InvalidEmail);

                RuleFor(x => x.Dto.OcrId)
                    .Matches(@"^\d{4}-\d{4}-\d{4}-\d{3}[\dX]$")
                    .WithMessage(MessageCode.BadRequest)
                    .When(x => x.Dto.OcrId != null);

                RuleFor(x => x.Dto.InitialPassword)
                    .NotEmpty()
                    .WithMessage(MessageCode.InitialPasswordIsRequired)
                    .MinimumLength(8)
                    .WithMessage(MessageCode.PasswordMinimumLength);
            });
    }

    #endregion
}

public sealed class CreateUserCommandHandler(
    IKeycloakService keycloakService,
    IMinIoCloudService minIoCloudService,
    ILogger<CreateUserCommandHandler> logger) : ICommandHandler<CreateUserCommand, string>
{
    #region Implementations

    public async Task<string> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var dto = command.Dto;

        // Step 1: Upload avatar first using username as filename
        var avatarUrl = await UploadAvatarAsync(dto.Username, dto.AvatarImage, cancellationToken);

        // Step 2: Create user in Keycloak with avatar URL already set
        var keycloakUserId = await keycloakService.CreateUserAsync(
            username: dto.Username,
            email: dto.Email,
            firstName: dto.FirstName,
            lastName: dto.LastName,
            ocrId: dto.OcrId,
            initialPassword: dto.InitialPassword,
            temporaryPassword: dto.TemporaryPassword,
            groupNames: dto.GroupNames,
            avatarUrl: avatarUrl,
            ocrId: dto.OcrId,
            cancellationToken: cancellationToken);

        return keycloakUserId;
    }

    #endregion

    #region Methods

    private async Task<string> UploadAvatarAsync(
        string username,
        UploadFileBytes? avatarImage,
        CancellationToken cancellationToken)
    {
        if (avatarImage is null)
            return AppConstants.Bucket.DefaultAvatar;

        try
        {
            var ext = Path.GetExtension(avatarImage.FileName);
            var objectName = $"{username}{ext}";

            var uploaded = await minIoCloudService.UploadFileAsync(
                avatarImage,
                AppConstants.Bucket.UserAvatars,
                objectName,
                isPublicBucket: true,
                cancellationToken);

            return uploaded.PublicURL ?? AppConstants.Bucket.DefaultAvatar;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to upload avatar for user '{Username}', using default avatar", username);
            return AppConstants.Bucket.DefaultAvatar;
        }
    }

    #endregion
}