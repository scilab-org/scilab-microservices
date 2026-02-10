#region using

using User.Application.Services;

#endregion

namespace User.Application.Features.Roles.Commands;

public sealed record RemoveRolesFromGroupCommand(string GroupId, List<string> RoleNames) : ICommand<bool>;

public sealed class RemoveRolesFromGroupCommandValidator : AbstractValidator<RemoveRolesFromGroupCommand>
{
    #region Ctors

    public RemoveRolesFromGroupCommandValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty()
            .WithMessage(MessageCode.GroupIdIsRequired);

        RuleFor(x => x.RoleNames)
            .NotEmpty()
            .WithMessage(MessageCode.RoleNamesAreRequired);
    }

    #endregion
}

public sealed class RemoveRolesFromGroupCommandHandler(
    IKeycloakService keycloakService) : ICommandHandler<RemoveRolesFromGroupCommand, bool>
{
    #region Implementations

    public async Task<bool> Handle(RemoveRolesFromGroupCommand command, CancellationToken cancellationToken)
    {
        await keycloakService.RemoveRolesFromGroupAsync(
            groupId: command.GroupId,
            roleNames: command.RoleNames,
            cancellationToken: cancellationToken);

        return true;
    }

    #endregion
}
