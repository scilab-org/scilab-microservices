using Management.Application.Dtos.Members;
using Management.Application.Models.Results;
using Management.Application.Services;
using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Member.Queries;

/// <summary>
/// Returns all members of the sub-project that owns the given paper.
/// No userId needed — used by Lab service to fetch sub-project members.
/// </summary>
public sealed record GetSubProjectMembersByPaperIdQuery(Guid PaperId) : IQuery<GetSubProjectMembersByPaperIdResult>;

public sealed class GetSubProjectMembersByPaperIdQueryValidator : AbstractValidator<GetSubProjectMembersByPaperIdQuery>
{
    public GetSubProjectMembersByPaperIdQueryValidator()
    {
        RuleFor(x => x.PaperId)
            .NotEmpty()
            .WithMessage(MessageCode.PaperIdIsRequired);
    }
}

[ExcludeFromCodeCoverage]
public sealed class GetSubProjectMembersByPaperIdQueryHandler(
    IDocumentSession session,
    IUserApiService userApiService)
    : IQueryHandler<GetSubProjectMembersByPaperIdQuery, GetSubProjectMembersByPaperIdResult>
{
    public async Task<GetSubProjectMembersByPaperIdResult> Handle(
        GetSubProjectMembersByPaperIdQuery request,
        CancellationToken cancellationToken)
    {
        // Find the sub-project that owns this paper
        var subProject = await session.Query<ProjectEntity>()
            .Where(p => p.PaperIds.Contains(request.PaperId) && p.ParentProjectId != null)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(MessageCode.SubProjectNotFound, request.PaperId.ToString());

        var members = await session.Query<MemberEntity>()
            .Where(m => m.ProjectId == subProject.Id)
            .ToListAsync(cancellationToken);

        if (!members.Any())
            return new GetSubProjectMembersByPaperIdResult(subProject.Id, []);

        // Enrich with user info from User service in one call
        var userInfos = await userApiService.GetUsersByIdsAsync(
            members.Select(m => m.UserId), cancellationToken);

        var userMap = userInfos.ToDictionary(u => u.Id);

        var items = members.Select(m =>
        {
            userMap.TryGetValue(m.UserId.ToString(), out var u);
            return new SubProjectMemberItemDto
            {
                MemberId  = m.Id,
                UserId    = m.UserId,
                Role      = m.ProjectRole,
                Username  = u?.Username,
                Email     = u?.Email,
                FirstName = u?.FirstName,
                LastName  = u?.LastName,
                Enabled   = u?.Enabled ?? false
            };
        }).ToList();

        return new GetSubProjectMembersByPaperIdResult(subProject.Id, items);
    }
}
