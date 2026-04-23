using Management.Application.Dtos.Papers;
using Management.Application.Models.Filters;
using Management.Application.Models.Results;
using Management.Application.Services;
using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Project.Queries;

public sealed record GetProjectPapersQuery(
    Guid ProjectId,
    PaginationRequest Paging,
    GetPaperBanksFilter Filter) : IQuery<GetProjectPapersResult>;

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

        var paperIds = project.PaperIds.Distinct().ToList();
        if (!paperIds.Any())
            return new GetProjectPapersResult(new List<PaperBankInfoDto>(), 0, query.Paging);

        // Fetch paper details from Lab service with title/keywords filter and paging
        var (items, totalCount) = await labApiService.GetPaperBanksByIdsPagedAsync(
            paperIds: paperIds,
            title: query.Filter.Title,
            keywords: query.Filter.Keyword,
            pageNumber: query.Paging.PageNumber,
            pageSize: query.Paging.PageSize,
            cancellationToken: cancellationToken);

        return new GetProjectPapersResult(items, totalCount, query.Paging);
    }

    #endregion
}