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

        var inUseSet = inUseIds.ToHashSet();

        var inUsePaperBanks = inUseIds.Count == 0
            ? []
            : await session.Query<PaperBankEntity>()
                .Where(x => inUseIds.Contains(x.Id))
                .ToListAsync(cancellationToken);

        var inUseMap = inUsePaperBanks.ToDictionary(x => x.Id, ToPaperBankDto);
        var inUse = inUseIds
            .Where(inUseMap.ContainsKey)
            .Select(x => inUseMap[x])
            .ToList();

        var paper = await session.LoadAsync<PaperEntity>(section.PaperId, cancellationToken)
                    ?? throw new NotFoundException(MessageCode.PaperIsNotExists, section.PaperId.ToString());

        var paperReferences = paper.References ?? [];

        var groupedOtherReferences = paperReferences
            .Where(x => x.PaperId != Guid.Empty && !inUseSet.Contains(x.PaperId))
            .GroupBy(x => x.PaperId)
            .Select(x => new
            {
                PaperId = x.Key,
                SectionIds = x.SelectMany(y => y.SectionIds).Where(id => id != Guid.Empty).Distinct().ToList()
            })
            .ToList();

        var otherReferencePaperIds = groupedOtherReferences.Select(x => x.PaperId).ToList();
        var otherPaperBanks = otherReferencePaperIds.Count == 0
            ? []
            : await session.Query<PaperBankEntity>()
                .Where(x => otherReferencePaperIds.Contains(x.Id))
                .ToListAsync(cancellationToken);

        var otherPaperBankMap = otherPaperBanks.ToDictionary(x => x.Id, ToPaperBankDto);

        var allSectionIds = groupedOtherReferences
            .SelectMany(x => x.SectionIds)
            .Distinct()
            .ToList();

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

        var otherReferences = groupedOtherReferences
            .Where(x => otherPaperBankMap.ContainsKey(x.PaperId))
            .Select(x => new PaperBankReferenceDto
            {
                PaperBank = otherPaperBankMap[x.PaperId],
                Sections = x.SectionIds
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

    private static ReferencePaperBankDto ToPaperBankDto(PaperBankEntity paperBank)
    {
        return new ReferencePaperBankDto
        {
            Id = paperBank.Id,
            Title = paperBank.Title,
            Abstract = paperBank.Abstract,
            Doi = paperBank.Doi,
            FilePath = paperBank.FilePath,
            Status = paperBank.Status,
            IsIngested = paperBank.IsIngested,
            IsAutoTagged = paperBank.IsAutoTagged,
            PublicationDate = paperBank.PublicationDate,
            PaperType = paperBank.PaperType,
            JournalName = paperBank.JournalName,
            ConferenceName = paperBank.ConferenceName,
            TagNames = paperBank.TagNames
        };
    }
}