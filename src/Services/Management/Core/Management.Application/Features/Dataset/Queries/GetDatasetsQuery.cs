using AutoMapper;
using Management.Application.Dtos.Datasets;
using Management.Application.Models.Results;
using Management.Domain.Entities;
using Marten;
using Marten.Linq;
using MediatR;

namespace Management.Application.Features.Dataset.Queries;

public sealed record GetDatasetsQuery(Guid? ProjectId, PaginationRequest Paging) : IRequest<GetDatasetsResult>;


public sealed class GetDatasetsQueryHandler(IDocumentSession session, IMapper mapper)
    : IRequestHandler<GetDatasetsQuery, GetDatasetsResult>
{
    #region Implementations

    public async Task<GetDatasetsResult> Handle(GetDatasetsQuery query, CancellationToken cancellationToken)
    {
        IQueryable<DatasetEntity> datasetQuery = session.Query<DatasetEntity>();

        // Filter base on project Id
        if (query.ProjectId.HasValue)
        {
            var project = await session.LoadAsync<ProjectEntity>(query.ProjectId.Value, cancellationToken);
            if (project == null)
                return new GetDatasetsResult([], 0, query.Paging);

            var datasetIds = project.DatasetIds ?? [];

            datasetQuery = datasetQuery.Where(d => datasetIds.Contains(d.Id));
        }

        var totalCount = await datasetQuery.CountAsync(cancellationToken);

        var result = await datasetQuery
            .OrderByDescending(x => x.CreatedOnUtc)
            .Skip((query.Paging.PageNumber - 1) * query.Paging.PageSize)
            .Take(query.Paging.PageSize)
            .ToListAsync(cancellationToken);

        var items = mapper.Map<List<DatasetDto>>(result);

        var response = new GetDatasetsResult(items, totalCount, query.Paging);

        return response;
    }

    #endregion
}