using AutoMapper;
using Management.Application.Dtos.Projects;
using Management.Application.Models.Results;
using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Project.Queries;

public sealed record GetProjectByIdQuery(Guid ProjectId) : IQuery<GetProjectByIdResult>;

public sealed class GetProjectByIdQueryHandler(IDocumentSession session, IMapper mapper)
    : IQueryHandler<GetProjectByIdQuery, GetProjectByIdResult>
{
    #region Implementations

    public async Task<GetProjectByIdResult> Handle(GetProjectByIdQuery query, CancellationToken cancellationToken)
    {
        var result = await session.LoadAsync<ProjectEntity>(query.ProjectId)
            ?? throw new NotFoundException(MessageCode.ProjectIsNotExists, query.ProjectId);

        var response = mapper.Map<ProjectDto>(result);
        return new GetProjectByIdResult(response);
    }

    #endregion
}