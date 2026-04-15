using Lab.Application.Dtos.Tasks;
using Lab.Application.Services;
using Lab.Domain.Constants;
using Lab.Domain.Entities;
using Marten;

namespace Lab.Application.Features.TaskDefinition.Commands.CreateTask;

public record CreateTaskCommand(CreateTaskDto Dto, string UserId, string UserName) : ICommand<Guid>;

public class CreateTaskValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskValidator()
    {
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest)
            .DependentRules(() =>
            {
                RuleFor(x => x.Dto.Name)
                    .NotEmpty()
                    .WithMessage(MessageCode.TaskNameIsRequired)
                    .NotNull()
                    .WithMessage(MessageCode.TaskNameIsRequired);
            });
    }
}

public class CreateTaskHandler(
    IDocumentSession session,
    IManagementApiService apiService) : ICommandHandler<CreateTaskCommand, Guid>
{
    #region Implementation

    public async Task<Guid> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        await ValidatePaperAndSectionAsync(dto, cancellationToken);

        var (subProjectId, memberId) = await ResolveMemberAsync(request, cancellationToken);

        var isAuthor = await ResolveIsAuthorAsync(subProjectId, cancellationToken);

        var paperContributor = await ResolveContributorAsync(dto, memberId, isAuthor, request.UserId, cancellationToken);

        var entity = TaskEntity.Create(
            Guid.NewGuid(),
            dto.Name,
            dto.Description,
            dto.AssignedToUserName,
            dto.Status,
            dto.StartDate,
            dto.NextReviewDate,
            dto.CompleteDate,
            request.UserName);

        session.Store(entity);

        paperContributor.AddTasks(entity.Id);
        session.Store(paperContributor);

        await session.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    #endregion

    #region Private Helpers

    /// <summary>
    /// Validates that the paper exists and the section (if provided) belongs to that paper.
    /// </summary>
    private async Task ValidatePaperAndSectionAsync(CreateTaskDto dto, CancellationToken cancellationToken)
    {
        var paper = await session.LoadAsync<PaperEntity>(dto.PaperId, cancellationToken);
        if (paper == null)
            throw new NotFoundException(MessageCode.PaperIsNotExists);

        if (!dto.SectionId.HasValue) return;

        var section = await session.LoadAsync<SectionEntity>(dto.SectionId.Value, cancellationToken);
        if (section == null || section.PaperId != dto.PaperId)
            throw new NotFoundException(MessageCode.SectionIsNotExists, dto.SectionId.Value);
    }

    /// <summary>
    /// Resolves the current user's subProjectId and memberId via the management API.
    /// </summary>
    private async Task<(Guid SubProjectId, Guid MemberId)> ResolveMemberAsync(
        CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var memberInfo = await apiService.GetMemberByPaperIdAsync(
            request.Dto.PaperId, Guid.Parse(request.UserId), cancellationToken);

        if (memberInfo == null)
            throw new NotFoundException(MessageCode.MemberNotFound, request.UserId);

        return memberInfo.Value;
    }

    /// <summary>
    /// Returns true if the current user holds the PaperAuthor role in the subproject.
    /// </summary>
    private async Task<bool> ResolveIsAuthorAsync(Guid subProjectId, CancellationToken cancellationToken)
    {
        var paperRole = await apiService.GetMyProjectRoleAsync(subProjectId, cancellationToken);
        if (paperRole == null)
            throw new NoPermissionException(MessageCode.AccessDenied);

        return paperRole == AuthorizeConstants.PaperAuthor;
    }

    /// <summary>
    /// Finds an existing PaperContributor or — for authors — creates one on-the-fly.
    /// Non-authors must have an existing contributor that matches PaperId + MemberId + SectionId.
    /// </summary>
    private async Task<PaperContributorEntity> ResolveContributorAsync(
        CreateTaskDto dto, Guid memberId, bool isAuthor, string userId, CancellationToken cancellationToken)
    {
        var existing = await session.Query<PaperContributorEntity>()
            .Where(x => x.PaperId == dto.PaperId
                        && x.MemberId == memberId
                        && (!dto.SectionId.HasValue || x.MarkSectionId == dto.SectionId.Value))
            .FirstOrDefaultAsync(cancellationToken);

        if (existing != null)
        {
            // Non-author: verify the contributor is actually tied to the requested section
            if (!isAuthor && (!dto.SectionId.HasValue || existing.SectionId != dto.SectionId))
                throw new NoPermissionException(MessageCode.AccessDenied);

            return existing;
        }

        // No contributor found — only authors may create one on-the-fly
        if (!isAuthor)
            throw new NoPermissionException(MessageCode.AccessDenied);

        if (!dto.SectionId.HasValue)
            throw new NotFoundException(MessageCode.PaperContributorNotFound, userId);

        return await CreateContributorForAuthorAsync(dto, memberId, cancellationToken);
    }

    /// <summary>
    /// Creates a new PaperContributor for the given section and — mirroring CreatePaperContributor —
    /// also auto-assigns the reference section if the member is not already linked to it.
    /// </summary>
    private async Task<PaperContributorEntity> CreateContributorForAuthorAsync(
        CreateTaskDto dto, Guid memberId, CancellationToken cancellationToken)
    {
        var mainContributor = PaperContributorEntity.Create(
            id: Guid.NewGuid(),
            sectionRole: AuthorizeConstants.SectionEdit,
            paperId: dto.PaperId,
            sectionId: dto.SectionId!.Value,
            memberId: memberId,
            markSectionId: dto.SectionId.Value);

        session.Store(mainContributor);

        // Auto-assign the reference section (same logic as CreatePaperContributor)
        await TryAssignReferenceSectionAsync(dto.PaperId, memberId, cancellationToken);

        return mainContributor;
    }

    /// <summary>
    /// Mirrors the CreatePaperContributor logic: if a reference section exists on the paper
    /// and the member is not yet assigned to it, create an additional contributor record.
    /// </summary>
    private async Task TryAssignReferenceSectionAsync(
        Guid paperId, Guid memberId, CancellationToken cancellationToken)
    {
        var candidateSections = await session.Query<SectionEntity>()
            .Where(s => s.PaperId == paperId && s.IsMainSection == true && s.IsOldMainSection == false)
            .ToListAsync(cancellationToken);

        var referenceSection = candidateSections
            .FirstOrDefault(s => SectionConstants.IsReferenceSection(s.Title));

        if (referenceSection == null) return;

        var alreadyAssigned = await session.Query<PaperContributorEntity>()
            .AnyAsync(pc => pc.PaperId == paperId
                            && pc.MemberId == memberId
                            && (pc.SectionId == referenceSection.Id || pc.MarkSectionId == referenceSection.Id),
                cancellationToken);

        if (alreadyAssigned) return;

        var referenceContributor = PaperContributorEntity.Create(
            id: Guid.NewGuid(),
            sectionRole: AuthorizeConstants.SectionEdit,
            paperId: paperId,
            sectionId: referenceSection.Id,
            memberId: memberId,
            markSectionId: referenceSection.Id);

        session.Store(referenceContributor);
    }

    #endregion
}
