using Microsoft.Extensions.Logging;
using User.Application.Services;

namespace User.Application.Features.Users;

public sealed record UpdateUserGroupsCommand(string UserId, List<string> GroupNames, Actor Actor) : ICommand<bool>;

public sealed class UpdateUserGroupsCommandValidator : AbstractValidator<UpdateUserGroupsCommand>
{
    public UpdateUserGroupsCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(MessageCode.UserIdIsRequired);

        RuleFor(x => x.GroupNames)
            .NotNull()
            .WithMessage(MessageCode.BadRequest);
    }
}

public sealed class UpdateUserGroupsCommandHandler(
    IKeycloakService keycloakService,
    ILogger<UpdateUserGroupsCommandHandler> logger) : ICommandHandler<UpdateUserGroupsCommand, bool>
{
    public async Task<bool> Handle(UpdateUserGroupsCommand command, CancellationToken cancellationToken)
    {
        await keycloakService.UpdateUserGroupsAsync(
            userId: command.UserId,
            groupNames: command.GroupNames,
            cancellationToken: cancellationToken);

        return true;
    }
}
