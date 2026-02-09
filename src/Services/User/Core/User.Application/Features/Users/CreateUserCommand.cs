#region using

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
    IKeycloakService keycloakService) : ICommandHandler<CreateUserCommand, string>
{
    #region Implementations

    public async Task<string> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var dto = command.Dto;

        var keycloakUserId = await keycloakService.CreateUserAsync(
            username: dto.Username,
            email: dto.Email,
            firstName: dto.FirstName,
            lastName: dto.LastName,
            initialPassword: dto.InitialPassword,
            temporaryPassword: dto.TemporaryPassword,
            groupNames: dto.GroupNames,
            cancellationToken: cancellationToken);

        return keycloakUserId;
    }

    #endregion
}