using Lab.Application.Dtos.Sections;
using Lab.Application.Models.Results;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Marten;
using Marten.Linq.SoftDeletes;

namespace Lab.Application.Features.Section.Queries.GetSectionHistory;

public sealed record GetSectionHistoryQuery(Guid MarkSectionId) : IQuery<GetSectionByMarkSectionIdResult>;

public sealed class GetSectionByMarkSectionIdQueryHandler(
    IDocumentSession session,
    IManagementApiService managementApiService,
    IUserApiService userApiService)
    : IQueryHandler<GetSectionHistoryQuery, GetSectionByMarkSectionIdResult>
{
    public async Task<GetSectionByMarkSectionIdResult> Handle(
        GetSectionHistoryQuery request,
        CancellationToken cancellationToken)
    {
        // Load contributors (exclude read-only roles)
        var contributors = await session.Query<PaperContributorEntity>()
            .Where(c => c.MaybeDeleted())
            .Where(c => c.MarkSectionId == request.MarkSectionId
                        && c.SectionRole != AuthorizeConstants.SectionRead)
            .ToListAsync(cancellationToken);

        if (!contributors.Any())
            return new GetSectionByMarkSectionIdResult([]);

        // Keep PaperAuthors + members who hold any section assignment
        var assignedMemberIds = contributors
            .Where(c => c.SectionId.HasValue)
            .Select(c => c.MemberId)
            .ToHashSet();

        var relevantContributors = contributors
            .Where(c => c.SectionRole.Equals(AuthorizeConstants.PaperAuthor, StringComparison.OrdinalIgnoreCase)
                        || assignedMemberIds.Contains(c.MemberId))
            .ToList();

        if (!relevantContributors.Any())
            return new GetSectionByMarkSectionIdResult([]);

        // Fetch sections + sub-project members in parallel
        var childSectionIds = contributors.Select(c => c.SectionId!.Value).Distinct().ToList();

        var childSectionsTask = childSectionIds.Any()
            ? session.Query<SectionEntity>().Where(s => childSectionIds.Contains(s.Id)).ToListAsync(cancellationToken)
            : Task.FromResult<IReadOnlyList<SectionEntity>>([]);

        var mainSectionTask = session.LoadAsync<SectionEntity>(request.MarkSectionId, cancellationToken);
        var membersTask     = managementApiService.GetSubProjectMembersByPaperIdAsync(relevantContributors.First().PaperId, cancellationToken);

        await Task.WhenAll(childSectionsTask, mainSectionTask, membersTask);

        var childSectionMap = (await childSectionsTask).ToDictionary(s => s.Id);
        var mainSection     = await mainSectionTask;

        // Build member/user lookup maps
        var relevantMemberIds = relevantContributors.Select(c => c.MemberId).ToHashSet();

        var memberMap = (await membersTask)
            .Where(m => relevantMemberIds.Contains(m.MemberId))
            .ToDictionary(m => m.MemberId);

        var userIds = memberMap.Values
            .Select(m => m.UserId)
            .Where(id => id != Guid.Empty)
            .Distinct().ToList();

        var userMap = await userApiService.GetUsersByIdsAsync(userIds, cancellationToken);

        // Project → exclude first-version sections → keep only the dominant previous-version group
        var items = relevantContributors
            .Select(c => ToDto(c, mainSection, childSectionMap, memberMap, userMap))
            .Where(dto => dto.PreviousVersionSectionId != null)
            .ToList();

        var dominantPreviousId = items
            .GroupBy(dto => dto.PreviousVersionSectionId)
            .MaxBy(g => g.Count())?.Key;

        return new GetSectionByMarkSectionIdResult(
            items.Where(dto => dto.PreviousVersionSectionId == dominantPreviousId).ToList());
    }

    #region Helper Methods

    private static SectionContributorDto ToDto(
        PaperContributorEntity                contributor,
        SectionEntity?                        mainSection,
        Dictionary<Guid, SectionEntity>       childSectionMap,
        Dictionary<Guid, SubProjectMemberInfo> memberMap,
        Dictionary<Guid, UserInfo>            userMap)
    {
        var isPaperAuthor = contributor.SectionRole.Equals(
            AuthorizeConstants.PaperAuthor, StringComparison.OrdinalIgnoreCase);

        var section = isPaperAuthor
            ? mainSection
            : (contributor.SectionId.HasValue
                ? childSectionMap.GetValueOrDefault(contributor.SectionId.Value)
                : null);

        memberMap.TryGetValue(contributor.MemberId, out var member);
        userMap.TryGetValue(member?.UserId ?? Guid.Empty, out var user);

        return new SectionContributorDto
        {
            MemberId      = contributor.MemberId,
            SectionRole   = contributor.SectionRole,
            SectionId     = contributor.SectionId,
            MarkSectionId = contributor.MarkSectionId,

            Title                    = section?.Title,
            IsMainSection            = section?.IsMainSection ?? false,
            IsOldMainSection         = section?.IsOldMainSection ?? false,
            Version                  = section?.Version,
            PreviousVersionSectionId = section?.PreviousVersionSectionId,
            NextVersionSectionId     = section?.NextVersionSectionId,
            CreatedBy                = section?.CreatedBy,
            CreatedOnUtc             = section?.CreatedOnUtc ?? DateTimeOffset.MinValue,
            LastModifiedOnUtc        = section?.LastModifiedOnUtc,
            MainIdea                 = section?.MainIdea,

            Name     = user != null ? $"{user.FirstName} {user.LastName}".Trim() : member?.Username,
            Email    = user?.Email ?? member?.Email,
            Content  = section?.Content,
            Packages = section?.Packages
        };
    }
    #endregion

}