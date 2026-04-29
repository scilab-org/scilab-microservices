using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Marten;

namespace Lab.Application.Features.Section.Queries.GetNumberOfCompleteSection;

public sealed record GetNumberOfCompleteSectionQuery(Guid MarkSectionId) : IQuery<GetNumberOfCompleteSectionResult>;

public sealed class GetNumberOfCompleteSectionQueryHandler(IDocumentSession session) : IQueryHandler<GetNumberOfCompleteSectionQuery, GetNumberOfCompleteSectionResult>
{
    public async Task<GetNumberOfCompleteSectionResult> Handle(GetNumberOfCompleteSectionQuery request, CancellationToken cancellationToken)
    {
        var sectionIds = await session.Query<PaperContributorEntity>()
            .Where(c => c.MarkSectionId == request.MarkSectionId && c.SectionId != request.MarkSectionId)
            .Select(c => c.SectionId)
            .Where(sectionId => sectionId.HasValue)
            .Select(sectionId => sectionId!.Value)
            .Distinct()
            .ToListAsync(cancellationToken);
        
        var completedCount = await session.Query<SectionEntity>()
            .Where(s => sectionIds.Contains(s.Id) && s.Status == SectionStatus.Completed)
            .CountAsync(cancellationToken);

        return new GetNumberOfCompleteSectionResult
        {
            NumberOfCompleteSection = completedCount,
            TotalSection = sectionIds.Count
        };
    }
}
