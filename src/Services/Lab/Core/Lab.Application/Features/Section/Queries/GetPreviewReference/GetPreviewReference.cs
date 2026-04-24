using Lab.Application.Dtos.GapTypes;
using Lab.Application.Dtos.PaperBanks;
using Lab.Application.Dtos.Sections;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Marten;

namespace Lab.Application.Features.Section.Queries.GetPreviewReference;

public record GetPreviewReferenceQuery(PreviewReferenceDto Dto) : ICommand<GetInUseReferenceBySectionIdResult>;

public class GetPreviewReferenceQueryValidator : AbstractValidator<GetPreviewReferenceQuery>
{
    public GetPreviewReferenceQueryValidator()
    {
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest)
            .DependentRules(() =>
            {
                RuleFor(x => x.Dto.PaperBankIds)
                    .NotNull()
                    .Must(ids => ids != null && ids.All(id => id != Guid.Empty))
                    .WithMessage(MessageCode.PaperBankIdsIsRequired);
            });
    }
}

public class
    GetPreviewReferenceQueryHandler(IDocumentSession session)
    : ICommandHandler<GetPreviewReferenceQuery, GetInUseReferenceBySectionIdResult>
{
    public async Task<GetInUseReferenceBySectionIdResult> Handle(GetPreviewReferenceQuery request,
        CancellationToken cancellationToken)
    {
        var requestedIds = request.Dto.PaperBankIds
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToList();

        if (requestedIds.Count == 0)
            return new GetInUseReferenceBySectionIdResult();

        var paperBanks = await session.Query<PaperBankEntity>()
            .Where(x => requestedIds.Contains(x.Id))
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

        var gaptypeIds = paperBanks
            .Select(x => x.ConferenceJournalId)
            .Where(x => x.HasValue)
            .Distinct()
            .ToList();

        var gaptypes = gaptypeIds.Count > 0
            ? await session.Query<GapTypeEntity>()
                .Where(x => gaptypeIds.Contains(x.Id))
                .ToListAsync(cancellationToken)
            : [];

        var paperBankMap = paperBanks.ToDictionary(x => x.Id, x => ToPaperBankInfoDto(x, journals, gaptypes));
        var items = requestedIds
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
        IReadOnlyList<ConferenceJournalEntity> journals, IReadOnlyList<GapTypeEntity> gapTypes)
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
            GapTypes = gapTypes
                .Where(x => (paperBank.GapTypeIds ?? []).Contains(x.Id))
                .Select(x => new GapTypeInfoDto
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToList(),
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