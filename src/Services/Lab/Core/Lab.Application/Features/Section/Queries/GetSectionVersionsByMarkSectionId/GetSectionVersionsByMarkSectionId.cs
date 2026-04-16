using AutoMapper;
using Lab.Application.Dtos.Sections;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;

namespace Lab.Application.Features.Section.Queries.GetSectionVersionsByMarkSectionId;

public record GetSectionVersionsByMarkSectionIdQuery(Guid MarkSectionId)
    : IQuery<GetSectionVersionsByMarkSectionIdResult>;

public sealed class GetSectionVersionsByMarkSectionIdQueryHandler(
    IDocumentSession session,
    IMapper mapper)
    : IQueryHandler<GetSectionVersionsByMarkSectionIdQuery, GetSectionVersionsByMarkSectionIdResult>
{
    public async Task<GetSectionVersionsByMarkSectionIdResult> Handle(
        GetSectionVersionsByMarkSectionIdQuery request,
        CancellationToken cancellationToken)
    {
        var mainSection = await session.LoadAsync<SectionEntity>(request.MarkSectionId, cancellationToken);

        if (mainSection is null)
            return new GetSectionVersionsByMarkSectionIdResult([]);

        // Walk backwards through the version chain collecting old main sections
        var oldVersions = new List<SectionEntity>();
        var currentPreviousId = mainSection.PreviousVersionSectionId;

        while (currentPreviousId.HasValue)
        {
            var previous = await session.LoadAsync<SectionEntity>(currentPreviousId.Value, cancellationToken);
            if (previous is null) break;

            if (previous.IsOldMainSection == true)
                oldVersions.Add(previous);

            currentPreviousId = previous.PreviousVersionSectionId;
        }

        var items = mapper.Map<List<SectionDto>>(oldVersions);
        return new GetSectionVersionsByMarkSectionIdResult(items);
    }
}
