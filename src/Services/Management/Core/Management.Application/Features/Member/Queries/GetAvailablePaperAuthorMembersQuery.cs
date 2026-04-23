using Management.Application.Dtos.Members;
using Management.Application.Models.Results;
using Management.Application.Services;
using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Member.Queries;

[ExcludeFromCodeCoverage]
public sealed record GetAvailablePaperAuthorMembersQuery(
    Guid SubProjectId,
    Guid PaperId,
    PaginationRequest Paging) : IQuery<GetProjectMembersResult>;

[ExcludeFromCodeCoverage]
public sealed class GetAvailablePaperAuthorMembersQueryValidator : AbstractValidator<GetAvailablePaperAuthorMembersQuery>
{
    public GetAvailablePaperAuthorMembersQueryValidator()
    {
        RuleFor(x => x.SubProjectId)
            .NotEmpty()
            .WithMessage(MessageCode.MemberProjectIdIsRequired);

        RuleFor(x => x.PaperId)
            .NotEmpty()
            .WithMessage(MessageCode.PaperIdIsRequired);
    }
}

[ExcludeFromCodeCoverage]
public sealed class GetAvailablePaperAuthorMembersQueryHandler(
    IDocumentSession session,
    IUserApiService userApiService,
    ILabApiService labApiService)
    : IQueryHandler<GetAvailablePaperAuthorMembersQuery, GetProjectMembersResult>
{
    public async Task<GetProjectMembersResult> Handle(
        GetAvailablePaperAuthorMembersQuery request,
        CancellationToken cancellationToken)
    {
        var subProject = await session.LoadAsync<ProjectEntity>(request.SubProjectId, cancellationToken);
        if (subProject is null)
            throw new NotFoundException(MessageCode.SubProjectNotFound);

        var members = await session.Query<MemberEntity>()
            .Where(x => x.ProjectId == subProject.Id)
            .ToListAsync(cancellationToken);

        if (!members.Any())
            return new GetProjectMembersResult([], 0, request.Paging);

        var paperAuthors = await labApiService.GetPaperAuthorsAsync(request.PaperId, cancellationToken);
        var paperAuthorMemberIds = paperAuthors
            .Select(x => x.MemberId)
            .Where(x => x != Guid.Empty)
            .ToHashSet();

        var availableMembers = members
            .Where(x => !paperAuthorMemberIds.Contains(x.Id))
            .ToList();

        if (!availableMembers.Any())
            return new GetProjectMembersResult([], 0, request.Paging);

        var userInfos = await userApiService.GetUsersByIdsAsync(
            availableMembers.Select(x => x.UserId),
            cancellationToken);

        var joined = availableMembers
            .Join(userInfos,
                m => m.UserId.ToString(),
                u => u.Id,
                (m, u) => new ProjectMemberDto
                {
                    MemberId = m.Id,
                    UserId = m.UserId,
                    SubProjectId = m.ProjectId,
                    Username = u.Username,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Orcid = u.OcrId,
                    Enabled = u.Enabled,
                    Role = m.ProjectRole,
                    JoinedAt = m.JoinedAt
                })
            .OrderBy(dto => dto.JoinedAt)
            .ToList();

        var totalCount = joined.Count;
        var paged = joined
            .Skip((request.Paging.PageNumber - 1) * request.Paging.PageSize)
            .Take(request.Paging.PageSize)
            .ToList();

        return new GetProjectMembersResult(paged, totalCount, request.Paging);
    }
}
