#region using

using User.Application.Services;

#endregion

namespace User.Application.Features.Users;

public sealed record ActivateUserCommand(string UserId, Actor Actor) : ICommand<bool>;

public sealed class ActivateUserCommandValidator : AbstractValidator<ActivateUserCommand>
{
    #region Ctors

    public ActivateUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(MessageCode.UserIdIsRequired);
    }

    #endregion
}

public sealed class ActivateUserCommandHandler(
    IKeycloakService keycloakService) : ICommandHandler<ActivateUserCommand, bool>
{
    #region Implementations

    public async Task<bool> Handle(ActivateUserCommand command, CancellationToken cancellationToken)
    {
        await keycloakService.ActivateUserAsync(
            userId: command.UserId,
            cancellationToken: cancellationToken);

        return true;
    }

    #endregion
}
