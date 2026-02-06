using Management.Domain.Entities;
using Marten;
using MediatR;

namespace Management.Application.Features.Dataset.Commands;

public record DeleteDatasetCommand(Guid DatasetId) : ICommand<Unit>;

public class DeleteDatasetValidator : AbstractValidator<DeleteDatasetCommand>
{
    #region Ctors

    public DeleteDatasetValidator()
    {
        RuleFor(x => x.DatasetId)
            .NotEmpty()
            .WithMessage(MessageCode.DatasetIdIsRequired);
    }

    #endregion
}

public class DeleteDatasetCommandHandler(IDocumentSession session, IMediator mediator)
    : ICommandHandler<DeleteDatasetCommand, Unit>
{
    #region Implementations

    public async Task<Unit> Handle(DeleteDatasetCommand command, CancellationToken cancellationToken)
    {
        var dataset = await session.LoadAsync<DatasetEntity>(command.DatasetId)
            ?? throw new ClientValidationException(MessageCode.DatasetIsNotExists, command.DatasetId.ToString());

        session.Delete(dataset);
        await session.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    #endregion
}