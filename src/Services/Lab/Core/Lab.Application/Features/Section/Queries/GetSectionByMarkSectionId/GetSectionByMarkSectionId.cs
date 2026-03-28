using Lab.Application.Dtos.PaperContributors;
using Lab.Application.Dtos.Sections;
using Lab.Application.Models.Results;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Marten;

namespace Lab.Application.Features.Section.Queries.GetSectionByMarkSectionId;

public record GetSectionByMarkSectionIdQuery(Guid MarkSectionId) : IQuery<GetSectionByMarkSectionIdResult>;


public sealed class GetSectionByMarkSectionIdQueryHandler(
    IDocumentSession session,
    IManagementApiService managementApiService,
    IUserApiService userApiService)
    : IQueryHandler<GetSectionByMarkSectionIdQuery, GetSectionByMarkSectionIdResult>
{
    public async Task<GetSectionByMarkSectionIdResult> Handle(
        GetSectionByMarkSectionIdQuery request,
        CancellationToken cancellationToken)
    {
        // Get all contributors of the markSectionId
        var allContributors = await session.Query<PaperContributorEntity>()
            .Where(c => c.MarkSectionId == request.MarkSectionId
                        && c.SectionRole != AuthorizeConstants.SectionRead)
            .ToListAsync(cancellationToken);

        if (!allContributors.Any())
            return new GetSectionByMarkSectionIdResult([]);

        // Get sectionIds from contributors except main section (markSectionId)
        var childSectionIds = allContributors
            .Where(c => c.SectionId.HasValue 
                        && c.SectionId.Value != request.MarkSectionId)
            .Select(c => c.SectionId!.Value)
            .Distinct().ToList();
        
        var childSectionsTask = childSectionIds.Any()
            ? session.Query<SectionEntity>()
                .Where(s => childSectionIds.Contains(s.Id))
                .ToListAsync(cancellationToken)
            : Task.FromResult<IReadOnlyList<SectionEntity>>([]);



        var mainSectionTask = session.LoadAsync<SectionEntity>(
            request.MarkSectionId, cancellationToken);

        await Task.WhenAll(childSectionsTask, mainSectionTask);

        var childSectionMap = (await childSectionsTask).ToDictionary(s => s.Id);
        var mainSection     = await mainSectionTask;

        // contributor.SectionId là id của section con họ đang giữ
        var memberIdsWithSection = allContributors
            .Where(c => c.SectionId.HasValue && c.SectionId.Value != request.MarkSectionId)
            .Select(c => c.MemberId)
            .ToHashSet();
        
        var filteredContributors = allContributors
            .Where(c =>
                string.Equals(c.SectionRole, AuthorizeConstants.PaperAuthor, StringComparison.OrdinalIgnoreCase)
                || memberIdsWithSection.Contains(c.MemberId))
            .ToList();

        if (!filteredContributors.Any())
            return new GetSectionByMarkSectionIdResult([]);

        var paperId = filteredContributors.First().PaperId;
        
        //Fetch members
        var membersTask = managementApiService
            .GetSubProjectMembersByPaperIdAsync(paperId, cancellationToken);

        // Collect userIds need to fetch
        var relevantMemberIds = filteredContributors.Select(c => c.MemberId).ToHashSet();

        var members = await membersTask;
        var memberMap = members
            .Where(m => relevantMemberIds.Contains(m.MemberId)) // ✅ chỉ giữ relevant members
            .ToDictionary(x => x.MemberId);

        var userIds = memberMap.Values
            .Select(m => m.UserId)
            .Where(id => id != Guid.Empty)
            .Distinct().ToList();

        var userMap = await userApiService.GetUsersByIdsAsync(userIds, cancellationToken);
        
        var items = filteredContributors.Select(c =>
        {
            // PaperAuthor uses main section, other roles use their specific section if they have one
            var isPaperAuthor = string.Equals(
                c.SectionRole, AuthorizeConstants.PaperAuthor, StringComparison.OrdinalIgnoreCase);

            var section = isPaperAuthor
                ? mainSection
                : (c.SectionId.HasValue ? childSectionMap.GetValueOrDefault(c.SectionId.Value) : null);

            memberMap.TryGetValue(c.MemberId, out var member);
            userMap.TryGetValue(member?.UserId ?? Guid.Empty, out var user);

            var name = user != null
                ? $"{user.FirstName} {user.LastName}".Trim()
                : member?.Username;

            return new SectionContributorDto
            {
                MemberId      = c.MemberId,
                SectionRole   = c.SectionRole,
                SectionId     = c.SectionId,
                MarkSectionId = c.MarkSectionId,

                Title                    = section?.Title,
                IsMainSection            = section?.IsMainSection ?? false,
                IsOldMainSection         = section?.IsOldMainSection ?? false,
                ParentSectionId          = section?.ParentSectionId,
                PreviousVersionSectionId = section?.PreviousVersionSectionId,
                NextVersionSectionId     = section?.NextVersionSectionId,
                CreatedBy                = section?.CreatedBy,
                CreatedOnUtc             = section?.CreatedOnUtc ?? DateTimeOffset.MinValue,
                LastModifiedOnUtc        = section?.LastModifiedOnUtc,

                Name    = name,
                Email   = user?.Email ?? member?.Email,
                Content = section?.Content
            };
        }).ToList();

        
        return new GetSectionByMarkSectionIdResult(items);
    }
}
