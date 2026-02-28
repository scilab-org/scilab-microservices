using Management.Application.Models.Results;
using Management.Application.Services;
using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Project.Queries;

public sealed record GetProjectPapersQuery(Guid ProjectId) : IQuery<GetProjectPapersResult>;

public class GetProjectPapersValidator : AbstractValidator<GetProjectPapersQuery>
{
    public GetProjectPapersValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage(MessageCode.ProjectIdIsRequired);
    }
}

public class GetProjectPapersQueryHandler(
    IDocumentSession session,
    ILabApiService labApiService)
    : IQueryHandler<GetProjectPapersQuery, GetProjectPapersResult>
{
    #region Implementations

    public async Task<GetProjectPapersResult> Handle(
        GetProjectPapersQuery query,
        CancellationToken cancellationToken)
    {
        // Verify project exists
        var project = await session.LoadAsync<ProjectEntity>(query.ProjectId, cancellationToken);
        if (project == null)
            throw new NotFoundException(MessageCode.ProjectIsNotExists);

        // Get all sub-projects (papers) belonging to this project
        var subProjects = await session.Query<ProjectEntity>()
            .Where(x => x.ParentProjectId == query.ProjectId)
            .ToListAsync(cancellationToken);

        var paperIds = subProjects
            .Where(x => x.PaperId.HasValue)
            .Select(x => x.PaperId!.Value)
            .Distinct();

        // Fetch full paper details from Lab service
        var papers = await labApiService.GetPapersByIdsAsync(
            paperIds: paperIds,
            cancellationToken: cancellationToken);

        return new GetProjectPapersResult(papers);
    }

    #endregion
}
