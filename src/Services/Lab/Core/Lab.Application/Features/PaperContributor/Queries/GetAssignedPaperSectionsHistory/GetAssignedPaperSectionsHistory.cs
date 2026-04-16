using Lab.Application.Dtos.Sections;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Marten;

namespace Lab.Application.Features.PaperContributor.Queries.GetAssignedPaperSectionsHistory;

public record GetAssignedPaperSectionsHistoryQuery(
    Guid PaperId,
    Guid UserId,
    GetAssignedPaperSectionsHistoryFilter Filter,
    PaginationRequest Paging)
    : IQuery<GetAssignedPaperSectionsHistoryResult>;

public sealed class GetAssignedPaperSectionsHistoryQueryValidator
    : AbstractValidator<GetAssignedPaperSectionsHistoryQuery>
{
    public GetAssignedPaperSectionsHistoryQueryValidator()
    {
        RuleFor(x => x.PaperId)
            .NotEmpty()
            .WithMessage(MessageCode.PaperIdIsRequired);

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(MessageCode.UserIdIsRequired);

        RuleFor(x => x.Filter)
            .Must(f => !f.FromDate.HasValue || !f.ToDate.HasValue || f.FromDate <= f.ToDate)
            .WithMessage("FromDate must be less than or equal to ToDate.");
    }
}

public sealed class GetAssignedPaperSectionsHistoryQueryHandler(
    IDocumentSession session,
    IManagementApiService managementApiService)
    : IQueryHandler<GetAssignedPaperSectionsHistoryQuery, GetAssignedPaperSectionsHistoryResult>
{
public async Task<GetAssignedPaperSectionsHistoryResult> Handle(
    GetAssignedPaperSectionsHistoryQuery request,
    CancellationToken cancellationToken)
{
    var paging = request.Paging;
    var filter = request.Filter;

    var memberInfo = await managementApiService.GetMemberByPaperIdAsync(
        request.PaperId, request.UserId, cancellationToken);

    if (memberInfo == null)
        throw new NotFoundException(MessageCode.MemberNotFound, request.UserId.ToString());

    var (_, memberId, _) = memberInfo.Value;


    var contributors = await session.Query<PaperContributorEntity>()
        .Where(x => x.PaperId == request.PaperId
                 && x.MemberId == memberId)
        .ToListAsync(cancellationToken);

    if (!contributors.Any())
        return new GetAssignedPaperSectionsHistoryResult([], 0, paging);

    if (!string.IsNullOrWhiteSpace(filter.SectionRole))
    {
        contributors = contributors
            .Where(x => x.SectionRole.Equals(filter.SectionRole, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }


    var contributorGroups = contributors
        .GroupBy(x => x.MarkSectionId)
        .Select(g => g.OrderByDescending(x => x.CreatedOnUtc).First())
        .ToList();


    var allSections = await session.Query<SectionEntity>()
        .Where(x => x.PaperId == request.PaperId)
        .ToListAsync(cancellationToken);

    var sectionById = allSections.ToDictionary(x => x.Id);

    var historyItems = new List<AssignedSectionHistoryItemDto>();

    foreach (var contributor in contributorGroups)
    {
        if (!sectionById.TryGetValue(contributor.MarkSectionId, out var currentMain))
            continue;

        var visited = new HashSet<Guid>();
        var current = currentMain.PreviousVersionSectionId;

        while (current.HasValue && sectionById.TryGetValue(current.Value, out var section))
        {
            if (!visited.Add(section.Id))
            {
                current = section.PreviousVersionSectionId;
                continue;
            }

            if (section.IsOldMainSection == true)
            {
                var matchesFilter =
                    (!filter.FromDate.HasValue || section.CreatedOnUtc >= filter.FromDate.Value) &&
                    (!filter.ToDate.HasValue || section.CreatedOnUtc <= filter.ToDate.Value);

                if (matchesFilter)
                {
                    historyItems.Add(new AssignedSectionHistoryItemDto
                    {
                        Id = section.Id,
                        PaperId = section.PaperId,
                        Title = section.Title,
                        Content = section.Content,
                        Description = section.Description,
                        MainIdea = section.MainIdea,
                        SectionSumary = section.SectionSumary,
                        CreatedOnUtc = section.CreatedOnUtc,
                        LastModifiedOnUtc = section.LastModifiedOnUtc,
                        DisplayOrder = section.DisplayOrder,
                        PaperContributorId = contributor.Id,
                        MemberId = memberId,
                        MarkSectionId = section.Id,
                        SectionRole = contributor.SectionRole,
                        IsOldMainSection = section.IsOldMainSection,
                        IsMainSection = section.IsMainSection
                    });
                }
            }

            current = section.PreviousVersionSectionId;
        }
    }
    historyItems = historyItems
        .GroupBy(x => x.Id)
        .Select(g => g.First())
        .ToList();

    var sortedItems = historyItems
        .OrderBy(x => x.DisplayOrder)
        .ThenByDescending(x => x.CreatedOnUtc)
        .ToList();

    var totalCount = sortedItems.Count;

    var pagedItems = sortedItems
        .Skip((paging.PageNumber - 1) * paging.PageSize)
        .Take(paging.PageSize)
        .ToList();

    return new GetAssignedPaperSectionsHistoryResult(pagedItems, totalCount, paging);
}
}