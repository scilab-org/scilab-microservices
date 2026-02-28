using Management.Application.Models.Results;
using Management.Application.Services;
using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Project.Queries;

public sealed record GetAvailablePapersQuery(
    Guid ProjectId,
    string? SearchText = null) : IQuery<GetAvailablePapersResult>;

public class GetAvailablePapersValidator : AbstractValidator<GetAvailablePapersQuery>
{
    public GetAvailablePapersValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage(MessageCode.ProjectIdIsRequired);
    }
}

public class GetAvailablePapersQueryHandler(
    IDocumentSession session,
    ILabApiService labApiService)
    : IQueryHandler<GetAvailablePapersQuery, GetAvailablePapersResult>
{
    #region Implementations

    public async Task<GetAvailablePapersResult> Handle(
        GetAvailablePapersQuery query,
        CancellationToken cancellationToken)
    {
        // Verify project exists
        var project = await session.LoadAsync<ProjectEntity>(query.ProjectId, cancellationToken);
        if (project == null)
            throw new NotFoundException(MessageCode.ProjectIsNotExists);

        // Collect all paperIds already added to sub-projects of this project
        var existingSubProjects = await session.Query<ProjectEntity>()
            .Where(x => x.ParentProjectId == query.ProjectId)
            .ToListAsync(cancellationToken);

        var existingPaperIds = existingSubProjects
            .Where(x => x.PaperId.HasValue)
            .Select(x => x.PaperId!.Value);

        // Fetch papers from Lab service excluding already-added ones
        var papers = await labApiService.GetAvailablePapersAsync(
            existingPaperIds: existingPaperIds,
            searchText: query.SearchText,
            cancellationToken: cancellationToken);

        return new GetAvailablePapersResult(papers);
    }

    #endregion
}
