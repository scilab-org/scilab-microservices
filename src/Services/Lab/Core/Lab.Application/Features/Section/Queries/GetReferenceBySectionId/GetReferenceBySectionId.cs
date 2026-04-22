using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;

namespace Lab.Application.Features.Section.Queries.GetReferenceBySectionId;

public record GetReferenceBySectionIdQuery(Guid Id) : ICommand<GetRefrerenceBySectionIdResult>;

public class GetReferenceBySectionIdQueryValidator : AbstractValidator<GetReferenceBySectionIdQuery>
{
    public GetReferenceBySectionIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(MessageCode.SectionIdIsRequired)
            .NotNull()
            .WithMessage(MessageCode.SectionIdIsRequired);
    }
}

public class GetReferenceBySectionIdQueryHandler(IDocumentSession session)
    : ICommandHandler<GetReferenceBySectionIdQuery, GetRefrerenceBySectionIdResult>
{
    public async Task<GetRefrerenceBySectionIdResult> Handle(GetReferenceBySectionIdQuery request,
        CancellationToken cancellationToken)
    {
        var section = await session.LoadAsync<SectionEntity>(request.Id, cancellationToken)
                      ?? throw new NotFoundException(MessageCode.SectionIsNotExists, request.Id.ToString());

        var inUseIds = (section.References ?? [])
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToList();

        var inUsePaperBanks = inUseIds.Count == 0
            ? []
            : await session.Query<PaperBankEntity>()
                .Where(x => inUseIds.Contains(x.Id))
                .ToListAsync(cancellationToken);

        var paper = await session.LoadAsync<PaperEntity>(section.PaperId, cancellationToken)
                    ?? throw new NotFoundException(MessageCode.PaperIsNotExists, section.PaperId.ToString());

        var paperReferences = paper.References ?? [];

        var filteredReferences = paperReferences
            .Where(x => x.PaperId != Guid.Empty)
            .Select(x => new
            {
                x.PaperId,
                SectionIds = x.SectionIds
                    .Where(id => id != Guid.Empty && id != section.Id)
                    .Distinct()
                    .ToList()
            })
            .Where(x => x.SectionIds.Count != 0)
            .ToList();

        var otherReferencePaperIds = filteredReferences
            .Select(x => x.PaperId)
            .Distinct()
            .ToList();

        var otherPaperBanks = otherReferencePaperIds.Count == 0
            ? []
            : await session.Query<PaperBankEntity>()
                .Where(x => otherReferencePaperIds.Contains(x.Id))
                .ToListAsync(cancellationToken);

        var journalIds = inUsePaperBanks
            .Concat(otherPaperBanks)
            .Select(x => x.ConferenceJournalId)
            .Where(x => x.HasValue)
            .Distinct()
            .ToList();

        var journals = journalIds.Count > 0
            ? await session.Query<ConferenceJournalEntity>()
                .Where(x => journalIds.Contains(x.Id))
                .ToListAsync(cancellationToken)
            : [];

        var inUse = inUsePaperBanks
            .Select(x => ToPaperBankDto(x, journals))
            .ToList();

        var otherPaperBankMap = otherPaperBanks
            .ToDictionary(x => x.Id, x => ToPaperBankDto(x, journals));

        var allSectionIds = filteredReferences
            .SelectMany(x => x.SectionIds)
            .Distinct()
            .ToList();

        var filteredReferenceMap = filteredReferences
            .GroupBy(x => x.PaperId)
            .ToDictionary(
                x => x.Key,
                x => x.SelectMany(y => y.SectionIds)
                    .Distinct()
                    .ToList());

        var sections = allSectionIds.Count == 0
            ? []
            : await session.Query<SectionEntity>()
                .Where(x => allSectionIds.Contains(x.Id))
                .ToListAsync(cancellationToken);

        var sectionMap = sections.ToDictionary(x => x.Id, x => new ReferenceSectionDto
        {
            Id = x.Id,
            Title = x.Title,
            Content = x.Content,
            DisplayOrder = x.DisplayOrder,
            PaperId = x.PaperId,
            CreatedBy = x.CreatedBy
        });

        var otherReferences = otherReferencePaperIds
            .Where(otherPaperBankMap.ContainsKey)
            .Select(paperId => new PaperBankReferenceDto
            {
                PaperBank = otherPaperBankMap[paperId],
                Sections = filteredReferenceMap[paperId]
                    .Where(sectionMap.ContainsKey)
                    .Select(id => sectionMap[id])
                    .OrderBy(s => s.DisplayOrder)
                    .ToList()
            })
            .OrderBy(x => x.PaperBank.Title)
            .ToList();

        return new GetRefrerenceBySectionIdResult
        {
            InUse = inUse,
            OtherReference = otherReferences
        };
    }

    private static ReferencePaperBankDto ToPaperBankDto(
        PaperBankEntity paperBank,
        IReadOnlyList<ConferenceJournalEntity> journals)
    {
        return new ReferencePaperBankDto
        {
            Id = paperBank.Id,
            Title = paperBank.Title,
            Authors = paperBank.Authors,
            Publisher = paperBank.Publisher,
            Abstract = paperBank.Abstract,
            Doi = paperBank.Doi,
            Url = paperBank.Url,
            Code = paperBank.Code,
            FilePath = paperBank.FilePath,
            BibFilePath = paperBank.BibFilePath,
            IsIngested = paperBank.IsIngested,
            IsAutoTagged = paperBank.IsAutoTagged,
            PublicationDate = paperBank.PublicationDate,
            PaperType = paperBank.PaperType,
            Pages = paperBank.Pages,
            Number = paperBank.Number,
            Volume = paperBank.Volume,
            ConferenceJournalName = journals.FirstOrDefault(x => x.Id == paperBank.ConferenceJournalId)?.Name,
            ReferenceContent = paperBank.ReferenceContent,
            Keywords = paperBank.Keywords
        };
    }
}