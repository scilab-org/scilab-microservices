#region using

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
    }

    #endregion
}

public sealed class UpdateUserCommandHandler(
    IKeycloakService keycloakService) : ICommandHandler<UpdateUserCommand, bool>
{
    #region Implementations

    public async Task<bool> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var dto = command.Dto;

        await keycloakService.UpdateUserAsync(
            userId: command.UserId,
            firstName: dto.FirstName,
            lastName: dto.LastName,
            enabled: dto.Enabled,
            groupNames: dto.GroupNames,
            cancellationToken: cancellationToken);

        return true;
    }

    #endregion
}
