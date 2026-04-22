using Lab.Application.Dtos.PaperBanks;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Marten;

namespace Lab.Application.Features.Section.Queries.GetInUseReferenceBySectionId;

public record GetInUseReferenceBySectionIdQuery(Guid Id) : ICommand<GetInUseReferenceBySectionIdResult>;

public class GetInUseReferenceBySectionIdQueryValidator : AbstractValidator<GetInUseReferenceBySectionIdQuery>
{
    public GetInUseReferenceBySectionIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(MessageCode.SectionIdIsRequired)
            .NotNull()
            .WithMessage(MessageCode.SectionIdIsRequired);
    }
}

public class GetInUseReferenceBySectionIdQueryHandler(IDocumentSession session)
    : ICommandHandler<GetInUseReferenceBySectionIdQuery, GetInUseReferenceBySectionIdResult>
{
    public async Task<GetInUseReferenceBySectionIdResult> Handle(GetInUseReferenceBySectionIdQuery request,
        CancellationToken cancellationToken)
    {
        var section = await session.LoadAsync<SectionEntity>(request.Id, cancellationToken)
                      ?? throw new NotFoundException(MessageCode.SectionIsNotExists, request.Id.ToString());

        var inUseIds = (section.References ?? [])
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToList();

        if (inUseIds.Count == 0)
            return new GetInUseReferenceBySectionIdResult();

        var paper = await session.LoadAsync<PaperEntity>(section.PaperId, cancellationToken)
                    ?? throw new NotFoundException(MessageCode.PaperIsNotExists, section.PaperId.ToString());

        var paperReferenceIds = (paper.References ?? [])
            .Where(x => inUseIds.Contains(x.PaperId))
            .Select(x => x.PaperId)
            .Distinct()
            .ToHashSet();

        var paperBankIds = inUseIds
            .Where(id => paperReferenceIds.Count == 0 || paperReferenceIds.Contains(id))
            .ToList();

        var paperBanks = await session.Query<PaperBankEntity>()
            .Where(x => paperBankIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var journalIds = paperBanks
            .Select(x => x.ConferenceJournalId)
            .Where(x => x.HasValue)
            .Distinct()
            .ToList();

        var journals = journalIds.Count > 0
            ? await session.Query<ConferenceJournalEntity>()
                .Where(x => journalIds.Contains(x.Id))
                .ToListAsync(cancellationToken)
            : [];

        var paperBankMap = paperBanks.ToDictionary(x => x.Id, x => ToPaperBankInfoDto(x, journals));
        var items = paperBankIds
            .Where(paperBankMap.ContainsKey)
            .Select(id => paperBankMap[id])
            .ToList();

        var combinedReferenceContent = string.Join(
            $"{Environment.NewLine}{Environment.NewLine}",
            items
                .Select(x => x.ReferenceContent)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct());

        return new GetInUseReferenceBySectionIdResult
        {
            ReferenceContent = string.IsNullOrWhiteSpace(combinedReferenceContent)
                ? string.Empty
                : $"\\begin{{filecontents*}}{{references.bib}}{Environment.NewLine}{Environment.NewLine}" +
                  combinedReferenceContent +
                  $"{Environment.NewLine}{Environment.NewLine}\\end{{filecontents*}}{Environment.NewLine}{Environment.NewLine}" +
                  "\\addbibresource{references.bib}",
            PaperBanks = items
        };
    }

    private static PaperBankInfoDto ToPaperBankInfoDto(PaperBankEntity paperBank,
        IReadOnlyList<ConferenceJournalEntity> journals)
    {
        return new PaperBankInfoDto
        {
            Id = paperBank.Id,
            Title = paperBank.Title,
            Authors = paperBank.Authors,
            Publisher = paperBank.Publisher,
            Ranking = paperBank.Ranking,
            Abstract = paperBank.Abstract,
            Doi = paperBank.Doi,
            Url = paperBank.Url,
            FilePath = paperBank.FilePath,
            BibFilePath = paperBank.BibFilePath,
            ParsedText = paperBank.ParsedText,
            IsIngested = paperBank.IsIngested,
            IsAutoTagged = paperBank.IsAutoTagged,
            PublicationDate = paperBank.PublicationDate,
            PaperType = paperBank.PaperType,
            Pages = paperBank.Pages,
            Number = paperBank.Number,
            Volume = paperBank.Volume,
            ConferenceJournalId = paperBank.ConferenceJournalId,
            ConferenceJournalName = journals.FirstOrDefault(x => x.Id == paperBank.ConferenceJournalId)?.Name,
            ReferenceContent = paperBank.ReferenceContent,
            Keywords = paperBank.Keywords,
            IngestStatus = paperBank.IngestStatus ?? IngestStatus.Pending
        };
    }
}