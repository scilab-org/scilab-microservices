using AutoMapper;
using Management.Application.Dtos.Domains;
using Management.Application.Dtos.Projects;
using Management.Application.Models.Results;
using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Project.Queries;

public sealed record GetProjectByIdQuery(Guid ProjectId, Guid UserId, List<string> Groups) : IQuery<GetProjectByIdResult>;

[ExcludeFromCodeCoverage]
public sealed class GetProjectByIdQueryHandler(IDocumentSession session, IMapper mapper)
    : IQueryHandler<GetProjectByIdQuery, GetProjectByIdResult>
{
    #region Implementations

    public async Task<GetProjectByIdResult> Handle(GetProjectByIdQuery query, CancellationToken cancellationToken)
    {
        var project = await session.LoadAsync<ProjectEntity>(query.ProjectId, cancellationToken)
                      ?? throw new NotFoundException(MessageCode.ProjectIsNotExists, query.ProjectId);
        
        if (!query.Groups.Contains(AuthorizeConstants.SystemAdmin))
        {
            var isMember = await session.Query<MemberEntity>()
                .AnyAsync(x => x.ProjectId == query.ProjectId && x.UserId == query.UserId, cancellationToken);
            if (!isMember)
                throw new NotFoundException(MessageCode.ProjectIsNotExists, query.ProjectId);
        }

        var response = mapper.Map<ProjectDto>(project);
        response.Domains = await LoadDomainsAsync(project.DomainIds, cancellationToken);

        return new GetProjectByIdResult(response);
    }

    private async Task<List<DomainDto>> LoadDomainsAsync(IEnumerable<Guid> domainIds, CancellationToken cancellationToken)
    {
        var ids = domainIds.Distinct().ToList();
        if (ids.Count == 0) return [];

        var domains = await session.Query<DomainEntity>()
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);

        return mapper.Map<List<DomainDto>>(domains);
    }

    #endregion
}
