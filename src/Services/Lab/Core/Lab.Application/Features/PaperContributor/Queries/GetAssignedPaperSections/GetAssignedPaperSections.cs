using Lab.Application.Dtos.Sections;
using Lab.Application.Models.Results;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Marten;
using Marten.Pagination;
namespace Lab.Application.Features.PaperContributor.Queries.GetAssignedPaperSections;
public record GetAssignedPaperSectionsQuery(Guid PaperId, Guid UserId, PaginationRequest Paging) : IQuery<GetMySectionsResult>;
public sealed class GetMySectionsQueryValidator : AbstractValidator<GetAssignedPaperSectionsQuery>
{
    public GetMySectionsQueryValidator()
    {
        RuleFor(x => x.PaperId)
            .NotEmpty()
            .WithMessage(MessageCode.PaperIdIsRequired);
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(MessageCode.UserIdIsRequired);
    }
}
public sealed class GetAssignedPaperSectionsQueryHandler(
    IDocumentSession session,
    IManagementApiService managementApiService)
    : IQueryHandler<GetAssignedPaperSectionsQuery, GetMySectionsResult>
{
    #region Implementations
    public async Task<GetMySectionsResult> Handle(GetAssignedPaperSectionsQuery request, CancellationToken cancellationToken)
    {
        var paging = request.Paging;

        // Single round-trip to Management service: resolves subProjectId + memberId together
        var memberInfo = await managementApiService.GetMemberByPaperIdAsync(
            request.PaperId, request.UserId, cancellationToken);
        if (memberInfo == null)
            throw new NotFoundException(MessageCode.MemberNotFound, request.UserId.ToString());

        var (subProjectId, memberId, projectId) = memberInfo.Value;

        // Get all PaperContributor records assigned to this member on this paper
        var contributors = await session.Query<PaperContributorEntity>()
            .Where(x => x.PaperId == request.PaperId && x.MemberId == memberId)
            .ToListAsync(cancellationToken);
        if (!contributors.Any())
            return new GetMySectionsResult(request.PaperId, subProjectId, memberId, [], 0, paging);

        var sectionIds = contributors
            .Where(c => c.SectionId.HasValue)
            .Select(c => c.SectionId!.Value)
            .Distinct()
            .ToList();

        var sections = await session.Query<SectionEntity>()
            .Where(s => s.PaperId == request.PaperId && sectionIds.Contains(s.Id))
            .ToListAsync(cancellationToken);

        var sectionMap = sections.ToDictionary(x => x.Id);

        var sectionPairs = contributors
            .Where(c => c.SectionId.HasValue)
            .Select(c =>
            {
                sectionMap.TryGetValue(c.SectionId!.Value, out var section);
                return new
                {
                    Contributor = c,
                    Section = section,
                    TitleKey = GetTitleKey(section, c.SectionId!.Value)
                };
            })
            .Where(x => x.Section != null)
            .ToList();

        // Keep only the newest contributor row per title (case-insensitive)
        var latestContributors = sectionPairs
            .GroupBy(x => x.TitleKey, StringComparer.OrdinalIgnoreCase)
            .Select(g => g
                .OrderByDescending(x => x.Contributor.CreatedOnUtc)
                .ThenByDescending(x => x.Contributor.LastModifiedOnUtc ?? x.Contributor.CreatedOnUtc)
                .First())
            .ToList();

        var latestSectionIds = latestContributors
            .Select(x => x.Contributor.SectionId!.Value)
            .Distinct()
            .ToList();

        // Query + paginate in one DB call; TotalItemCount is embedded in IPagedList
        var pagedSections = await session.Query<SectionEntity>()
            .Where(s => s.PaperId == request.PaperId && latestSectionIds.Contains(s.Id))
            .OrderBy(s => s.DisplayOrder)
            .ToPagedListAsync(paging.PageNumber, paging.PageSize, cancellationToken);

        // contributor lookup: sectionId -> newest contributor for that title
        var contributorMap = latestContributors.ToDictionary(x => x.Contributor.SectionId!.Value);

        var assignedSections = pagedSections
            .Select(s =>
            {
                var c = contributorMap[s.Id].Contributor;
                return new AssignedSectionDto
                {
                    Id                 = s.Id,
                    PaperId            = s.PaperId,
                    Title              = s.Title,
                    Content            = s.Content,
                    Description        = s.Description,
                    MainIdea           = s.MainIdea,
                    SectionSumary      = s.SectionSumary,
                    CreatedOnUtc       = s.CreatedOnUtc,
                    LastModifiedOnUtc  = s.LastModifiedOnUtc,
                    DisplayOrder       = s.DisplayOrder,
                    PaperContributorId = c.Id,
                    SectionRole        = c.SectionRole,
                    MemberId           = c.MemberId,
                    MarkSectionId      = c.MarkSectionId,
                    IsMainSection       = s.IsMainSection,
                    IsOldMainSection    = s.IsOldMainSection,
                    Rule                = s.Rule,
                    Version             = s.Version,
                    Packages            = s.Packages,
                    Status             = s.Status,
                };
            })
            .ToList();

        return new GetMySectionsResult(request.PaperId, subProjectId, memberId, assignedSections, pagedSections.TotalItemCount, paging);
    }

    private static string GetTitleKey(SectionEntity? section, Guid fallbackId)
    {
        var title = section?.Title?.Trim();
        return string.IsNullOrWhiteSpace(title) ? fallbackId.ToString() : title;
    }
    #endregion
}