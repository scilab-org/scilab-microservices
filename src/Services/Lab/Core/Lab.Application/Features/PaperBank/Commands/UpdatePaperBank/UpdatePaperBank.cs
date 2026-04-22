using Lab.Application.Dtos.PaperBanks;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.PaperBank.Commands.UpdatePaperBank;

public record UpdatePaperBankCommand(Guid Id, UpdatePaperBankDto BankDto) : ICommand<Guid>;

public class UpdatePaperCommandVaBanklidator : AbstractValidator<UpdatePaperBankCommand>
{
    public UpdatePaperCommandVaBanklidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(MessageCode.PaperIdIsRequired);

        RuleFor(x => x.BankDto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest)
            .DependentRules(() =>
            {
                RuleFor(x => x.BankDto)
                    .NotNull()
                    .WithMessage(MessageCode.BadRequest)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.BankDto.Title)
                            .NotEmpty()
                            .WithMessage(MessageCode.PaperTitleIsRequired)
                            .NotNull()
                            .WithMessage(MessageCode.PaperTitleIsRequired);

                        RuleFor(x => x.BankDto.PublicationDate)
                            .LessThanOrEqualTo(DateTimeOffset.UtcNow)
                            .When(x => x.BankDto.PublicationDate.HasValue)
                            .WithMessage(MessageCode.PaperPublicationDateInvalid);
                    });

                RuleFor(x => x.BankDto.PublicationDate)
                    .LessThanOrEqualTo(DateTimeOffset.UtcNow)
                    .When(x => x.BankDto.PublicationDate.HasValue)
                    .WithMessage(MessageCode.PaperPublicationDateInvalid);

                RuleFor(x => x.BankDto.ConferenceJournalId)
                    .NotEmpty()
                    .WithMessage(MessageCode.JournalIdIsRequired)
                    .NotNull()
                    .WithMessage(MessageCode.JournalIdIsRequired);
            });
    }
}

public class UpdatePaperCommandBankHandler(IDocumentSession session)
    : IRequestHandler<UpdatePaperBankCommand, Guid>
{
    public async Task<Guid> Handle(UpdatePaperBankCommand request, CancellationToken cancellationToken)
    {
        var dto = request.BankDto;
        var keywords = NormalizeTagNames(dto.Keywords);

        await session.BeginTransactionAsync(cancellationToken);

        var entity = await session.LoadAsync<PaperBankEntity>(request.Id, cancellationToken)
                     ?? throw new ClientValidationException(MessageCode.PaperIsNotExists, request.Id);

        var journal = await session.LoadAsync<ConferenceJournalEntity>(dto.ConferenceJournalId, cancellationToken)
                      ?? throw new ClientValidationException(MessageCode.JournalIsNotExists, dto.ConferenceJournalId);

        await EnsureTagsExistAsync(keywords, cancellationToken);

        entity.Update(
            title: dto.Title,
            authors: dto.Authors,
            publisher: dto.Publisher,
            ranking: dto.Ranking,
            abstractText: dto.Abstract,
            doi: dto.Doi,
            url: dto.Url,
            code: dto.Code,
            isIngested: dto.IsIngested,
            isAutoTagged: dto.IsAutoTagged,
            publicationDate: dto.PublicationDate,
            paperType: dto.PaperType,
            pages: dto.Pages,
            number: dto.Number,
            volume: dto.Volume,
            conferenceJournalId: journal.Id,
            referenceContent: dto.ReferenceContent,
            ingestStatus: dto.IngestStatus);

        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    #region Methods

    private List<string> NormalizeTagNames(List<string>? keywords)
    {
        if (keywords == null) return new List<string>();

        return keywords.Select(x => x.Trim().ToLowerInvariant()).ToList();
    }

    private async Task EnsureTagsExistAsync(
        List<string> keywords,
        CancellationToken cancellationToken)
    {
        if (keywords.Count == 0) return;

        var existingTags = await session
            .Query<KeywordEntity>()
            .Where(x => keywords.Contains(x.Name))
            .ToListAsync(cancellationToken);

        var existingTagNames = existingTags
            .Select(x => x.Name)
            .ToHashSet();

        var newTagNames = keywords
            .Where(x => !existingTagNames.Contains(x))
            .Distinct()
            .ToList();

        foreach (var name in newTagNames)
        {
            var tag = KeywordEntity.Create(Guid.NewGuid(), name);
            session.Store(tag);
        }
    }

    #endregion
}