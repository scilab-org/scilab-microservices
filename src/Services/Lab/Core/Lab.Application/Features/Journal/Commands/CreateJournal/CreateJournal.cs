using Lab.Application.Dtos.Journals;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.Journal.Commands.CreateJournal;

public record CreateJournalCommand(CreateJournalEntityDto Dto) : ICommand<Guid>;

public class CreateJournalCommandValidator : AbstractValidator<CreateJournalCommand>
{
    public CreateJournalCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest)
            .DependentRules(() =>
            {
                RuleFor(x => x.Dto.Name)
                    .NotEmpty().WithMessage(MessageCode.JournalNameIsRequired)
                    .NotNull().WithMessage(MessageCode.JournalNameIsRequired);
            });
    }
}

public class CreateJournalCommandHandler(IDocumentSession session) : IRequestHandler<CreateJournalCommand, Guid>
{
    #region Implementations

    public async Task<Guid> Handle(CreateJournalCommand request, CancellationToken cancellationToken)
    {
        await session.BeginTransactionAsync(cancellationToken);

        var normalizedName = request.Dto.Name.Trim();

        var existingJournal = await session.Query<JournalEntity>()
            .FirstOrDefaultAsync(x => x.Name == normalizedName, cancellationToken);

        if (existingJournal != null)
            throw new ClientValidationException(MessageCode.JournalNameAlreadyExists, request.Dto.Name);

        var entity = JournalEntity.Create(
            id: Guid.NewGuid(),
            name: normalizedName,
            styles: request.Dto.Styles);

        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    #endregion
}