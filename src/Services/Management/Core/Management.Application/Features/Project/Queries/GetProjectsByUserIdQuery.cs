using AutoMapper;
using Management.Application.Dtos.Projects;
using Management.Application.Models.Results;
using Management.Domain.Entities;
using Marten;
using Marten.Pagination;

namespace Management.Application.Features.Project.Queries;

public sealed record GetProjectsByUserIdQuery(
    Guid UserId,
    PaginationRequest Paging) : IQuery<GetProjectsResult>;

public sealed class GetProjectsByUserIdValidator : AbstractValidator<GetProjectsByUserIdQuery>
{
    public GetProjectsByUserIdValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(MessageCode.UserIdIsRequired);
    }
}

public sealed class GetProjectsByUserIdQueryHandler(IDocumentSession session, IMapper mapper)
    : IQueryHandler<GetProjectsByUserIdQuery, GetProjectsResult>
{
    #region Implementations

    public async Task<GetProjectsResult> Handle(GetProjectsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var userId = request.UserId;
        var paging = request.Paging;

        // Get all project IDs where the user is a member
        var memberProjectIds = await session.Query<MemberEntity>()
            .Where(x => x.UserId == userId)
            .Select(x => x.ProjectId)
            .ToListAsync(cancellationToken);

        var query = session.Query<ProjectEntity>()
            .Where(x => memberProjectIds.Contains(x.Id));

        var totalCount = await query.CountAsync(cancellationToken);

        var result = await query
            .OrderByDescending(x => x.CreatedOnUtc)
            .ToPagedListAsync(paging.PageNumber, paging.PageSize, cancellationToken);

        var items = mapper.Map<List<ProjectDto>>(result.ToList());

        return new GetProjectsResult(items, totalCount, paging);
    }

    #endregion
}



