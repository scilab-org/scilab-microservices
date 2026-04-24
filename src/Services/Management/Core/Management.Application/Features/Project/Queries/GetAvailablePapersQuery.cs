using Management.Application.Models.Filters;
using Management.Application.Models.Results;
using Management.Application.Services;
using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Project.Queries;

public sealed record GetAvailablePapersQuery(
    Guid ProjectId, GetPaperBanksFilter Filter, PaginationRequest Paging) : IQuery<GetAvailablePapersResult>;

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
        var filter = query.Filter;
        var paging = query.Paging;

        // Fetch papers from Lab service via GET /paper-bank, excluding already-added ones
        var (items, totalCount) = await labApiService.GetAvailablePapersAsync(
            existingPaperIds: existingPaperIds,
            title: filter.Title,
            author: filter.Author,
            publisher: filter.Publisher,
            @abstract: filter.Abstract,
            doi: filter.Doi,
            status: filter.Status,
            fromPublicationDate: filter.FromPublicationDate,
            toPublicationDate: filter.ToPublicationDate,
            paperType: filter.PaperType,
            journalName: filter.JournalName,
            conferenceName: filter.ConferenceName,
            keywords: filter.Keyword,
            pageNumber: paging.PageNumber,
            pageSize: paging.PageSize,
            cancellationToken: cancellationToken);

        return new GetAvailablePapersResult(items, totalCount, paging);
    }

    #endregion
}