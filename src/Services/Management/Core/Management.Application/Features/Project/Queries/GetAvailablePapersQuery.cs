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

        var existingPaperIds = project.PaperIds.Distinct();

        // Fetch papers from Lab service excluding already-added ones
        var papers = await labApiService.GetAvailablePapersAsync(
            existingPaperIds: existingPaperIds,
            searchText: query.SearchText,
            cancellationToken: cancellationToken);

        return new GetAvailablePapersResult(papers);
    }

    #endregion
}
