using Lab.Application.Dtos.Journals;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.Journal.Commands.UpdateJournal;

public record UpdateJournalCommand(UpdateJournalEntityDto Dto, Guid Id, string UserName) : ICommand<Guid>;

public class UpdateJournalCommandValidator : AbstractValidator<UpdateJournalCommand>
{
    public UpdateJournalCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest)
            .DependentRules(() =>
            {
                RuleFor(x => x.Id)
                    .NotEmpty().WithMessage(MessageCode.JournalIdIsRequired);
            });

        RuleFor(x => x.Dto.EndAt)
            .LessThanOrEqualTo(DateTimeOffset.UtcNow)
            .When(x => x.Dto.EndAt.HasValue)
            .WithMessage(MessageCode.JournalEndAtInvalid);

        RuleFor(x => x.Dto.EndAt)
            .LessThanOrEqualTo(x => x.Dto.StartAt)
            .When(x => x.Dto is { EndAt: not null, StartAt: not null })
            .WithMessage(MessageCode.JournalEndAtInvalid);
    }
}

public class UpdateJournalCommandHandler(
    IDocumentSession session) : IRequestHandler<UpdateJournalCommand, Guid>
{
    #region Implementations

    public async Task<Guid> Handle(UpdateJournalCommand request, CancellationToken cancellationToken)
    {
        await session.BeginTransactionAsync(cancellationToken);

        var entity = await session.LoadAsync<ConferenceJournalEntity>(request.Id, cancellationToken)
                     ?? throw new ClientValidationException(MessageCode.JournalIsNotExists, request.Id);

        entity.Update(
            startAt: request.Dto.StartAt,
            endAt: request.Dto.EndAt,
            lastModifiedBy: request.UserName);

        session.Update(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    #endregion
}