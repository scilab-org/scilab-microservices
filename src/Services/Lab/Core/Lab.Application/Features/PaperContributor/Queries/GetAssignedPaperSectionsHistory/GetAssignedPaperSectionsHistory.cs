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

public sealed class GetAssignedPaperSectionsHistoryQueryValidator : AbstractValidator<GetAssignedPaperSectionsHistoryQuery>
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

        var (_, memberId) = memberInfo.Value;

        var contributors = await session.Query<PaperContributorEntity>()
            .Where(x => x.PaperId == request.PaperId && x.MemberId == memberId && x.MarkSectionId != Guid.Empty)
            .ToListAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(filter.SectionRole))
        {
            contributors = contributors
                .Where(x => x.SectionRole.Equals(filter.SectionRole, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (!contributors.Any())
            return new GetAssignedPaperSectionsHistoryResult([], 0, paging);

        var sectionIds = contributors
            .Select(c => c.MarkSectionId)
            .Distinct()
            .ToList();

        var sections = await session.Query<SectionEntity>()
            .Where(s => s.PaperId == request.PaperId && sectionIds.Contains(s.Id))
            .ToListAsync(cancellationToken);

        var sectionMap = sections.ToDictionary(x => x.Id);

        var sectionPairs = contributors
            .Select(c =>
            {
                sectionMap.TryGetValue(c.MarkSectionId, out var section);
                return new
                {
                    Contributor = c,
                    Section = section,
                    TitleKey = section?.Title?.Trim() ?? c.MarkSectionId.ToString()
                };
            })
            .Where(x => x.Section != null)
            .ToList();

        var historyItems = sectionPairs
            .GroupBy(x => x.TitleKey, StringComparer.OrdinalIgnoreCase)
            .SelectMany(group => group
                .OrderByDescending(x => x.Section?.CreatedOnUtc)
                .ThenByDescending(x => x.Section?.LastModifiedOnUtc ?? x.Section?.CreatedOnUtc)
                .ThenByDescending(x => x.Section?.Id)
                .Skip(1))
            .Where(x =>
                (!filter.FromDate.HasValue || x.Section?.CreatedOnUtc >= filter.FromDate.Value) &&
                (!filter.ToDate.HasValue || x.Section?.CreatedOnUtc <= filter.ToDate.Value))
            .Select(x => new AssignedSectionHistoryItemDto
            {
                Id = x.Section!.Id,
                PaperId = x.Section.PaperId,
                Title = x.Section.Title,
                Content = x.Section.Content,
                Description = x.Section.Description,
                SectionSumary = x.Section.SectionSumary,
                CreatedOnUtc = x.Section.CreatedOnUtc,
                LastModifiedOnUtc = x.Section.LastModifiedOnUtc,
                DisplayOrder = x.Section.DisplayOrder,
                Numbered = x.Section.Numbered,
                ParentSectionId = x.Section.ParentSectionId,
                PaperContributorId = x.Contributor.Id,
                MemberId = x.Contributor.MemberId,
                MarkSectionId = x.Contributor.MarkSectionId,
                SectionRole = x.Contributor.SectionRole
            })
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Title)
            .ToList();

        var pagedItems = historyItems
            .Skip((paging.PageNumber - 1) * paging.PageSize)
            .Take(paging.PageSize)
            .ToList();

        return new GetAssignedPaperSectionsHistoryResult(pagedItems, historyItems.Count, paging);
    }
}
