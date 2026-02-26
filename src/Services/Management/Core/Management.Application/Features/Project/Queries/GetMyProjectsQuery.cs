using AutoMapper;
using Management.Application.Dtos.Projects;
using Management.Application.Models.Filters;
using Management.Application.Models.Results;
using Management.Domain.Entities;
using Marten;
using Marten.Pagination;

namespace Management.Application.Features.Project.Queries;

public sealed record GetMyProjectsQuery(
    Guid UserId,
    PaginationRequest Paging,
    GetMyProjectsFilter Filter) : IQuery<GetProjectsResult>;

public sealed class GetMyProjectsValidator : AbstractValidator<GetMyProjectsQuery>
{
    public GetMyProjectsValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(MessageCode.UserIdIsRequired);
    }
}

public sealed class GetMyProjectsQueryHandler(IDocumentSession session, IMapper mapper)
    : IQueryHandler<GetMyProjectsQuery, GetProjectsResult>
{
    #region Implementations

    public async Task<GetProjectsResult> Handle(GetMyProjectsQuery request, CancellationToken cancellationToken)
    {
        var userId = request.UserId;
        var paging = request.Paging;
        var filter = request.Filter;

        // Get all project IDs where the user is a member
        var memberProjectIds = await session.Query<MemberEntity>()
            .Where(x => x.UserId == userId)
            .Select(x => x.ProjectId)
            .ToListAsync(cancellationToken);

        var query = session.Query<ProjectEntity>()
            .Where(x => memberProjectIds.Contains(x.Id))
            .AsQueryable();

        if (!filter.Name.IsNullOrWhiteSpace())
        {
            var name = filter.Name.Trim();
            query = query.Where(x => x.Name != null && x.Name.Contains(name));
        }

        if (!filter.Code.IsNullOrWhiteSpace())
        {
            var code = filter.Code.Trim();
            query = query.Where(x => x.Code != null && x.Code.Contains(code));
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(x => x.Status == filter.Status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var result = await query
            .OrderByDescending(x => x.CreatedOnUtc)
            .ToPagedListAsync(paging.PageNumber, paging.PageSize, cancellationToken);

        var items = mapper.Map<List<ProjectDto>>(result.ToList());

        return new GetProjectsResult(items, totalCount, paging);
    }

    #endregion
}
