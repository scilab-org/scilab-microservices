#region using

using User.Application.Services;

#endregion

namespace User.Application.Features.Roles.Commands;

public sealed record AddRolesToGroupCommand(string GroupId, List<string> RoleNames) : ICommand<bool>;

public sealed class AddRolesToGroupCommandValidator : AbstractValidator<AddRolesToGroupCommand>
{
    #region Ctors

    public AddRolesToGroupCommandValidator()
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

public sealed class AddRolesToGroupCommandHandler(
    IKeycloakService keycloakService) : ICommandHandler<AddRolesToGroupCommand, bool>
{
    #region Implementations

    public async Task<bool> Handle(AddRolesToGroupCommand command, CancellationToken cancellationToken)
    {
        await keycloakService.AddRolesToGroupAsync(
            groupId: command.GroupId,
            roleNames: command.RoleNames,
            cancellationToken: cancellationToken);

        return true;
    }

    #endregion
}
