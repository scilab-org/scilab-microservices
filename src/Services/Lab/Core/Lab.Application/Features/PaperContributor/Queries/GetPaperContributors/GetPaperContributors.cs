using Lab.Application.Dtos.PaperContributors;
using Lab.Application.Models.Results;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Marten;

namespace Lab.Application.Features.PaperContributor.Queries.GetPaperContributors;

public record GetPaperContributorsQuery(Guid PaperId) : IQuery<GetPaperContributorsResult>;

public sealed class GetPaperContributorsQueryValidator : AbstractValidator<GetPaperContributorsQuery>
{
    public GetPaperContributorsQueryValidator()
    {
        RuleFor(x => x.PaperId)
            .NotEmpty()
            .WithMessage(MessageCode.PaperIdIsRequired);
    }
}

public sealed class GetPaperContributorsQueryHandler(
    IDocumentSession session,
    IManagementApiService managementApiService,
    IUserApiService userApiService)
    : IQueryHandler<GetPaperContributorsQuery, GetPaperContributorsResult>
{
    public async Task<GetPaperContributorsResult> Handle(
        GetPaperContributorsQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Get all contributors for this paper
        var contributors = await session.Query<PaperContributorEntity>()
            .Where(c => c.PaperId == request.PaperId && c.SectionRole != AuthorizeConstants.PaperAuthor)
            .ToListAsync(cancellationToken);

        if (!contributors.Any())
            return new GetPaperContributorsResult([]);

        // 2. Resolve MemberId -> UserId via Management service
        var allMembers = await managementApiService.GetSubProjectMembersByPaperIdAsync(
            request.PaperId, cancellationToken);

        // Build lookup: MemberId -> SubProjectMemberInfo
        var memberMap = allMembers.ToDictionary(m => m.MemberId);

        // 3. Collect all UserIds and fetch user info from UserService
        var userIds = allMembers
            .Select(m => m.UserId)
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();

        var userInfoMap = await userApiService.GetUsersByIdsAsync(userIds, cancellationToken);

        // 4. Build response DTOs
        var items = contributors.Select(c =>
        {
            memberMap.TryGetValue(c.MemberId, out var memberInfo);
            var userId = memberInfo?.UserId ?? Guid.Empty;
            userInfoMap.TryGetValue(userId, out var userInfo);

            // Prefer UserService name/email; fall back to Management info
            var name = userInfo != null
                ? $"{userInfo.FirstName} {userInfo.LastName}".Trim()
                : memberInfo?.Username;
            var email = userInfo?.Email ?? memberInfo?.Email;

            return new PaperContributorDto
            {
                Id            = c.Id,
                PaperId       = c.PaperId,
                MemberId      = c.MemberId,
                MarkSectionId = c.MarkSectionId,
                SectionId     = c.SectionId,
                SectionRole   = c.SectionRole,
                UserId        = userId,
                ContributorName  = string.IsNullOrWhiteSpace(name) ? memberInfo?.Username : name,
                ContributorEmail = email,
                FirstName     = userInfo?.FirstName ?? memberInfo?.FirstName,
                LastName      = userInfo?.LastName  ?? memberInfo?.LastName
            };
        }).ToList();

        return new GetPaperContributorsResult(items);
    }
}

