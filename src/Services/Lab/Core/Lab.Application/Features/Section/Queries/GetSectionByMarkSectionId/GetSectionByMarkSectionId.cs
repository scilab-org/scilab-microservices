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
        // Get all contributors for this paper
        var contributors = await session.Query<PaperContributorEntity>()
            .Where(c => c.MarkSectionId == request.MarkSectionId && c.SectionRole != AuthorizeConstants.SectionRead)
            .ToListAsync(cancellationToken);
        if (!contributors.Any())
            return new GetSectionByMarkSectionIdResult([]);

        var paperId = contributors.First().PaperId;
        
        // 2. Get sections
        var sectionIds = contributors.Where(x => x.SectionId != null)
                                    .Select(x => x.SectionId!.Value).Distinct().ToList();
        
        var sections = await session.Query<SectionEntity>()
            .Where(x => sectionIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var sectionMap = sections.ToDictionary(x => x.Id);
        
        // Get members
        var members = await managementApiService
            .GetSubProjectMembersByPaperIdAsync(paperId, cancellationToken);

        var memberMap = members.ToDictionary(x => x.MemberId);

        // Get users
        var userIds = members
            .Select(m => m.UserId)
            .Where(id => id != Guid.Empty)
            .Distinct().ToList();

        var userMap = await userApiService.GetUsersByIdsAsync(userIds, cancellationToken);

        // Build response DTOs
        var items = contributors.Select(c =>
        {
            sectionMap.TryGetValue(c.SectionId ?? Guid.Empty, out var section);
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
                
                Title = section?.Title,
                IsMainSection = section?.IsMainSection ?? false,
                ParentSectionId = section?.ParentSectionId,
                PreviousVersionSectionId = section?.PreviousVersionSectionId,
                NextVersionSectionId = section?.NextVersionSectionId,
                
                Name  = name,
                Email = user?.Email ?? member?.Email,
                Content = section?.Content
            };
        }).ToList();

        return new GetSectionByMarkSectionIdResult(items);
    }
}
