#region using

using Microsoft.Extensions.Logging;
using User.Application.Dtos.Users;
using User.Application.Services;

#endregion

namespace User.Application.Features.Users;

public sealed record UpdateUserCommand(string UserId, UpdateUserDto Dto, Actor Actor) : ICommand<bool>;

public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    #region Ctors

    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(MessageCode.UserIdIsRequired);

        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest);

        RuleFor(x => x.Dto.OcrId)
            .Matches(@"^\d{4}-\d{4}-\d{4}-\d{3}[\dX]$")
            .WithMessage(MessageCode.BadRequest)
            .When(x => x.Dto.OcrId != null);
    }

    #endregion
}

public sealed class UpdateUserCommandHandler(
    IKeycloakService keycloakService,
    IMinIoCloudService minIoCloudService,
    ILogger<UpdateUserCommandHandler> logger) : ICommandHandler<UpdateUserCommand, bool>
{
    #region Implementations

    public async Task<bool> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var dto = command.Dto;

        string? avatarUrl = null;
        if (dto.AvatarImage is not null)
        {
            avatarUrl = await UploadAvatarAsync(command.UserId, dto.AvatarImage, cancellationToken);
        }

        await keycloakService.UpdateUserAsync(
            userId: command.UserId,
            firstName: dto.FirstName,
            lastName: dto.LastName,
            ocrId: dto.OcrId,
            enabled: dto.Enabled,
            groupNames: dto.GroupNames,
            avatarUrl: avatarUrl,
            cancellationToken: cancellationToken);

        return true;
    }

    #endregion

    #region Methods

    private async Task<string> UploadAvatarAsync(
        string userId,
        UploadFileBytes avatarImage,
        CancellationToken cancellationToken)
    {
        try
        {
            var ext = Path.GetExtension(avatarImage.FileName);
            var objectName = $"{userId}{ext}";

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
            logger.LogWarning(ex, "Failed to upload avatar for user '{UserId}', using default avatar", userId);
            return AppConstants.Bucket.DefaultAvatar;
        }
    }

    #endregion
}
