using Lab.Application.Dtos.PaperContributors;
using Lab.Application.Dtos.Sections;
using Lab.Application.Models.Results;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Marten;
using Marten.Linq.SoftDeletes;

namespace Lab.Application.Features.Section.Queries.GetSectionByMarkSectionId;

public record GetSectionByMarkSectionIdQuery(Guid MarkSectionId, Guid CurrentUserId) : IQuery<GetSectionByMarkSectionIdResult>;


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
        var mainSectionTask = session.LoadAsync<SectionEntity>(
            request.MarkSectionId, cancellationToken);

        // Get all contributors of the markSectionId
        var allContributors = await session.Query<PaperContributorEntity>()
            .Where(c => c.MaybeDeleted())
            .Where(c => c.MarkSectionId == request.MarkSectionId
                        && c.SectionRole != AuthorizeConstants.SectionRead)
            .ToListAsync(cancellationToken);

        var mainSection = await mainSectionTask;
        var mainSectionItem = new SectionContributorDto
        {
            MemberId      = Guid.Empty,
            SectionRole   = null!,
            SectionId     = request.MarkSectionId,
            MarkSectionId = request.MarkSectionId,

            Title                    = mainSection?.Title,
            IsMainSection            = mainSection?.IsMainSection ?? true,
            IsOldMainSection         = mainSection?.IsOldMainSection ?? false,
            Version                  = mainSection?.Version,
            ParentSectionId          = mainSection?.ParentSectionId,
            PreviousVersionSectionId = mainSection?.PreviousVersionSectionId,
            NextVersionSectionId     = mainSection?.NextVersionSectionId,
            CreatedBy                = mainSection?.CreatedBy,
            CreatedOnUtc             = mainSection?.CreatedOnUtc ?? DateTimeOffset.MinValue,
            LastModifiedOnUtc        = mainSection?.LastModifiedOnUtc,

            Name     = null,
            Email    = null,
            Content  = mainSection?.Content,
            Packages = mainSection?.Packages
        };

        if (!allContributors.Any())
            return new GetSectionByMarkSectionIdResult([mainSectionItem]);

        var paperId = allContributors.First().PaperId;
        var currentMemberInfo = await managementApiService
            .GetMemberByPaperIdAsync(paperId, request.CurrentUserId, cancellationToken);

        var currentMemberId = currentMemberInfo?.MemberId;
        // var currentEditingSectionIds = currentMemberId.HasValue
        //     ? allContributors
        //         .Where(c => c.MemberId == currentMemberId.Value
        //                     && c.SectionId.HasValue
        //                     && c.SectionId.Value != request.MarkSectionId)
        //         .Select(c => c.SectionId!.Value)
        //         .ToHashSet()
        //     : [];

        // Get sectionIds from contributors except main section (markSectionId)
        // and exclude sections currently assigned to current user
        var childSectionIds = allContributors
            .Where(c => c.SectionId.HasValue
                        && c.SectionId.Value != request.MarkSectionId)
                        // && !currentEditingSectionIds.Contains(c.SectionId.Value))
            .Select(c => c.SectionId!.Value)
            .Distinct().ToList();

        if (!childSectionIds.Any())
            return new GetSectionByMarkSectionIdResult([mainSectionItem]);

        var childSectionsTask = session.Query<SectionEntity>()
            .Where(s => childSectionIds.Contains(s.Id))
            .ToListAsync(cancellationToken);

        var childSectionMap = (await childSectionsTask).ToDictionary(s => s.Id);

        var filteredContributors = allContributors
            .Where(c => c.SectionId.HasValue
                        && childSectionIds.Contains(c.SectionId.Value))
            .ToList();

        if (!filteredContributors.Any())
            return new GetSectionByMarkSectionIdResult([mainSectionItem]);

        //Fetch members
        var membersTask = managementApiService
            .GetSubProjectMembersByPaperIdAsync(paperId, cancellationToken);

        // Collect userIds need to fetch
        var relevantMemberIds = filteredContributors.Select(c => c.MemberId).ToHashSet();

        var members = await membersTask;
        var memberMap = members
            .Where(m => relevantMemberIds.Contains(m.MemberId))
            .ToDictionary(x => x.MemberId);

        var userIds = memberMap.Values
            .Select(m => m.UserId)
            .Where(id => id != Guid.Empty)
            .Distinct().ToList();

        var userMap = await userApiService.GetUsersByIdsAsync(userIds, cancellationToken);

        var childItems = filteredContributors.Select(c =>
        {
            var section = c.SectionId.HasValue
                ? childSectionMap.GetValueOrDefault(c.SectionId.Value)
                : null;

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
                Version                  = section?.Version,
                ParentSectionId          = section?.ParentSectionId,
                PreviousVersionSectionId = section?.PreviousVersionSectionId,
                NextVersionSectionId     = section?.NextVersionSectionId,
                CreatedBy                = section?.CreatedBy,
                CreatedOnUtc             = section?.CreatedOnUtc ?? DateTimeOffset.MinValue,
                LastModifiedOnUtc        = section?.LastModifiedOnUtc,

                Name     = name,
                Email    = user?.Email ?? member?.Email,
                Content  = section?.Content,
                Packages = section?.Packages
            };
        })
        .ToList();

        var items = new List<SectionContributorDto> { mainSectionItem };
        items.AddRange(childItems);


        return new GetSectionByMarkSectionIdResult(items);
    }
}