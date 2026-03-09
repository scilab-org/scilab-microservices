using Management.Application.Dtos.Members;
using Management.Application.Services;
using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Member.Commands;

public sealed record AddSubProjectMembersCommand(Guid SubProjectId, AddProjectMembersDto Dto, Guid UserId) : ICommand<List<Guid>>;

public class AddSubProjectMembersValidator : AbstractValidator<AddSubProjectMembersCommand>
{
    public AddSubProjectMembersValidator()
    {
        RuleFor(x => x.SubProjectId)
            .NotEmpty()
            .WithMessage(MessageCode.MemberProjectIdIsRequired);

        RuleForEach(x => x.Dto.Members).ChildRules(member =>
        {
            member.RuleFor(m => m.UserId)
                .NotEmpty()
                .WithMessage(MessageCode.UserIdsAreRequired);
        });
    }
}

public class AddSubProjectMembersCommandHandler(
    IDocumentSession session,
    IUserApiService userApiService,
    ILabApiService labApiService)
    : ICommandHandler<AddSubProjectMembersCommand, List<Guid>>
{
    #region Implementations

    public async Task<List<Guid>> Handle(AddSubProjectMembersCommand command, CancellationToken cancellationToken)
    {
        var dto = command.Dto;

        var subProject = await session.LoadAsync<ProjectEntity>(command.SubProjectId, cancellationToken);
        if (subProject == null)
            throw new NotFoundException(MessageCode.SubProjectNotFound);

        // Check current user is ProjectManager or Author in parent project
        var isManagerAuthor = await session.Query<MemberEntity>()
            .AnyAsync(x =>
                    x.ProjectId == subProject.ParentProjectId &&
                    x.UserId == command.UserId &&
                    (x.ProjectRole == AuthorizeConstants.ProjectManager
                  || x.ProjectRole == AuthorizeConstants.ProjectAuthor),
                cancellationToken);

        if (!isManagerAuthor)
            throw new NoPermissionException(MessageCode.AccessDenied);

        // Deduplicate by UserId
        var memberMap = dto.Members
            .Where(m => m.UserId != Guid.Empty)
            .GroupBy(m => m.UserId)
            .ToDictionary(g => g.Key, g => g.Last().GroupName);

        var distinctUserIds = memberMap.Keys.ToList();

        var validUserIds = await userApiService.GetExistingUserIdsAsync(distinctUserIds, cancellationToken);
        if (validUserIds.Count == 0)
            throw new NotFoundException(MessageCode.UserNotFound);

        // Load existing members to prevent duplicates
        var existingUserIds = (await session.Query<MemberEntity>()
                .Where(x => x.ProjectId == subProject.Id)
                .Select(x => x.UserId)
                .ToListAsync(cancellationToken))
            .ToHashSet();

        var createdIds  = new List<Guid>();
        // track author members: (memberId, userId) for post-save Lab calls
        var authorMembers = new List<(Guid MemberId, Guid UserId)>();

        foreach (var userId in validUserIds)
        {
            if (existingUserIds.Contains(userId)) continue;

            var groupName = memberMap.GetValueOrDefault(userId, AuthorizeConstants.ProjectMember);

            var member = MemberEntity.Create(
                id: Guid.NewGuid(),
                userId: userId,
                projectId: subProject.Id,
                projectRole: groupName,
                joinedAt: DateTimeOffset.UtcNow);

            session.Store(member);
            createdIds.Add(member.Id);

            if (string.Equals(groupName, AuthorizeConstants.ProjectAuthor, StringComparison.OrdinalIgnoreCase))
                authorMembers.Add((member.Id, userId));
        }

        if (!createdIds.Any())
            throw new ClientValidationException(MessageCode.AllMembersAlreadyExist);

        await session.SaveChangesAsync(cancellationToken);

        // For each author member, fetch sections from Lab and create a PaperContributor per section
        if (authorMembers.Count > 0 && subProject.PaperIds.Count > 0)
        {
            var paperId  = subProject.PaperIds.First();
            var sections = await labApiService.GetSectionsByPaperIdAsync(paperId, cancellationToken);

            foreach (var (memberId, _) in authorMembers)
            {
                foreach (var section in sections)
                {
                    await labApiService.CreatePaperContributorAsync(
                        sectionRole   : AuthorizeConstants.PaperAuthor,
                        paperId       : paperId,
                        memberId      : memberId,
                        markSectionId : section.Id,
                        sectionId     : section.Id,
                        cancellationToken: cancellationToken);
                }
            }
        }

        return createdIds;
    }

    #endregion
}