#region using

using User.Application.Services;

#endregion

namespace User.Application.Features.Users;

public sealed record DeactivateUserCommand(string UserId, Actor Actor) : ICommand<bool>;

public sealed class DeactivateUserCommandValidator : AbstractValidator<DeactivateUserCommand>
{
    #region Ctors

    public DeactivateUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(MessageCode.UserIdIsRequired);
    }

    #endregion
}

public sealed class DeactivateUserCommandHandler(
    IKeycloakService keycloakService) : ICommandHandler<DeactivateUserCommand, bool>
{
    #region Implementations

    public async Task<bool> Handle(DeactivateUserCommand command, CancellationToken cancellationToken)
    {
        await keycloakService.DeactivateUserAsync(
            userId: command.UserId,
            cancellationToken: cancellationToken);

        return true;
    }

    #endregion
}
