using Lab.Application.Dtos.Journals;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.Journal.Commands.UpdateJournal;

public record UpdateJournalCommand(UpdateJournalEntityDto Dto) : ICommand<Guid>;

public class UpdateJournalCommandValidator : AbstractValidator<UpdateJournalCommand>
{
    public UpdateJournalCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest)
            .DependentRules(() =>
            {
                RuleFor(x => x.Dto.Id)
                    .NotEmpty().WithMessage(MessageCode.JournalIdIsRequired);
            });
    }
}

public class UpdateJournalCommandHandler(IDocumentSession session) : IRequestHandler<UpdateJournalCommand, Guid>
{
    #region Implementations

    public async Task<Guid> Handle(UpdateJournalCommand request, CancellationToken cancellationToken)
    {
        await session.BeginTransactionAsync(cancellationToken);

        var entity = await session.LoadAsync<JournalEntity>(request.Dto.Id, cancellationToken);

        if (entity == null)
            throw new ClientValidationException(MessageCode.JournalIsNotExists, request.Dto.Id);

        var normalizedName = "";
        if (request.Dto.Name != null)
            normalizedName = request.Dto.Name.Trim();

        if (!string.IsNullOrEmpty(normalizedName) && normalizedName != entity.Name)
        {
            var existingJournalName = await session.Query<JournalEntity>()
                .FirstOrDefaultAsync(x => x.Name == normalizedName && x.Id != request.Dto.Id, cancellationToken);

            if (existingJournalName != null)
                throw new ClientValidationException(MessageCode.JournalNameAlreadyExists, normalizedName);
        }

        entity.Update(
            name: normalizedName != "" ? normalizedName : null,
            styles: request.Dto.Styles);

        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    #endregion
}