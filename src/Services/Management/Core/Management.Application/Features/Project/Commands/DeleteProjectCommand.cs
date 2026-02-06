using Management.Domain.Entities;
using Marten;
using MediatR;

namespace Management.Application.Features.Project.Commands;

public record DeleteProjectCommand(Guid ProjectId) : ICommand<Unit>;

public class DeleteProjectCommandValidator : AbstractValidator<DeleteProjectCommand>
{
    #region Ctors

    public DeleteProjectCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage(MessageCode.ProjectIdIsRequired);
    }

    #endregion
}

public class DeleteProjectCommandHandler(IDocumentSession session) : ICommandHandler<DeleteProjectCommand, Unit>
{
    #region Implementations

    public async Task<Unit> Handle(DeleteProjectCommand command, CancellationToken cancellationToken)
    {
        var project = await session.LoadAsync<ProjectEntity>(command.ProjectId, cancellationToken)
            ?? throw new ClientValidationException(MessageCode.ProjectIsNotExists, command.ProjectId.ToString());

        session.Delete(project);
        await session.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    #endregion
}