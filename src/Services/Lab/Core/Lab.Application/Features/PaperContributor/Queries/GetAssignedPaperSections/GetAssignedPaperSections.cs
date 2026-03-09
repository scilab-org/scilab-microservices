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

        var (subProjectId, memberId) = memberInfo.Value;

        // Get all PaperContributor records assigned to this member on this paper
        var contributors = await session.Query<PaperContributorEntity>()
            .Where(x => x.PaperId == request.PaperId && x.MemberId == memberId)
            .ToListAsync(cancellationToken);
        if (!contributors.Any())
            return new GetMySectionsResult(request.PaperId, subProjectId, memberId, [], 0, paging);

        // Collect assigned markSectionIds
        var markSectionIds = contributors
            .Select(c => c.MarkSectionId)
            .Distinct()
            .ToList();

        // Query + paginate in one DB call; TotalItemCount is embedded in IPagedList
        var pagedSections = await session.Query<SectionEntity>()
            .Where(s => s.PaperId == request.PaperId && markSectionIds.Contains(s.Id))
            .OrderBy(s => s.DisplayOrder)
            .ToPagedListAsync(paging.PageNumber, paging.PageSize, cancellationToken);

        // contributor lookup: markSectionId -> contributor (one contributor per markSection)
        var contributorMap = contributors.ToDictionary(c => c.MarkSectionId);

        var assignedSections = pagedSections
            .Select(s =>
            {
                var c = contributorMap[s.Id];
                return new AssignedSectionDto
                {
                    Id                 = s.Id,
                    PaperId            = s.PaperId,
                    Title              = s.Title,
                    Content            = s.Content,
                    SectionSumary      = s.SectionSumary,
                    DisplayOrder       = s.DisplayOrder,
                    Numbered           = s.Numbered,
                    FilePath           = s.FilePath,
                    ParentSectionId    = s.ParentSectionId,
                    PaperContributorId = c.Id,
                    SectionRole        = c.SectionRole,
                    MemberId           = c.MemberId,
                    MarkSectionId      = c.MarkSectionId
                };
            })
            .ToList();

        return new GetMySectionsResult(request.PaperId, subProjectId, memberId, assignedSections, pagedSections.TotalItemCount, paging);
    }
    #endregion
}
