using Management.Application.Dtos.Papers;
using Management.Application.Services;
using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Project.Queries;

public record GetProjectSubmissionStatusSummaryQuery(Guid ProjectId)
    : IQuery<SubmissionStatusSummaryResult>;

[ExcludeFromCodeCoverage]
public class GetProjectSubmissionStatusSummaryQueryHandler(
    IDocumentSession session,
    ILabApiService labApiService)
    : IQueryHandler<GetProjectSubmissionStatusSummaryQuery, SubmissionStatusSummaryResult>
{
    public async Task<SubmissionStatusSummaryResult> Handle(
        GetProjectSubmissionStatusSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var project = await session.LoadAsync<ProjectEntity>(request.ProjectId, cancellationToken)
                      ?? throw new NotFoundException(MessageCode.ProjectIsNotExists);

        var subProjects = await session.Query<ProjectEntity>()
            .Where(x => x.ParentProjectId == request.ProjectId)
            .Select(x => x.PaperIds)
            .ToListAsync(cancellationToken);

        var allPaperIds = subProjects
            .SelectMany(ids => ids)
            .Distinct()
            .ToList();

        if (allPaperIds.Count == 0)
            return new SubmissionStatusSummaryResult();

        return await labApiService.GetSubmissionStatusSummaryAsync(allPaperIds, cancellationToken);
    }
}
