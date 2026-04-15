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

        // 1. Validate paper exists
        var paper = await session.LoadAsync<PaperEntity>(dto.PaperId, cancellationToken);
        if (paper == null)
            throw new NotFoundException(MessageCode.PaperIsNotExists);

        // 2. Resolve member — user must be a member of the subproject that owns this paper
        var memberInfo = await apiService.GetMemberByPaperIdAsync(
            dto.PaperId, Guid.Parse(request.UserId), cancellationToken);
        if (memberInfo == null)
            throw new NotFoundException(MessageCode.MemberNotFound, request.UserId);

        var (subProjectId, memberId) = memberInfo.Value;

        // 3. Only PaperAuthor can create tasks
        var paperRole = await apiService.GetMyProjectRoleAsync(subProjectId, cancellationToken);
        if (paperRole == null || paperRole != AuthorizeConstants.PaperAuthor)
            throw new NoPermissionException(MessageCode.AccessDenied);

        // 4. Create the task entity
        var entity = TaskEntity.Create(
            id: Guid.NewGuid(),
            memberId: memberId,
            name: dto.Name,
            description: dto.Description,
            assignedToUserName: dto.AssignedToUserName,
            status: dto.Status,
            startDate: dto.StartDate,
            nextReviewDate: dto.NextReviewDate,
            completeDate: dto.CompleteDate,
            createdBy: request.UserName);

        session.Store(entity);

        // 5. Add task to Member in Management service
        await apiService.AddMemberTasksAsync(subProjectId, memberId, [entity.Id], cancellationToken);

        // 6. If SectionId is provided → find or create a PaperContributor and link the task
        if (dto.SectionId.HasValue)
        {
            // Validate section belongs to the paper
            var section = await session.LoadAsync<SectionEntity>(dto.SectionId.Value, cancellationToken);
            if (section == null || section.PaperId != dto.PaperId)
                throw new NotFoundException(MessageCode.SectionIsNotExists, dto.SectionId.Value);

            var contributor = await ResolveOrCreateContributorAsync(dto, memberId, cancellationToken);
            session.Store(contributor);

            // Auto-assign reference section (mirrors CreatePaperContributor logic)
            await TryAssignReferenceSectionAsync(dto.PaperId, memberId, cancellationToken);
        }
        

        await session.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    #endregion

    #region Private Helpers

    /// <summary>
    /// Finds an existing PaperContributor for this paper/member/section, or creates one if missing.
    /// </summary>
    private async Task<PaperContributorEntity> ResolveOrCreateContributorAsync(
        CreateTaskDto dto, Guid memberId, CancellationToken cancellationToken)
    {
        var existing = await session.Query<PaperContributorEntity>()
            .Where(x => x.PaperId == dto.PaperId
                        && x.MemberId == memberId
                        && x.MarkSectionId == dto.SectionId!.Value)
            .FirstOrDefaultAsync(cancellationToken);

        if (existing != null)
            return existing;

        // Create a new contributor for this author + section
        var newContributor = PaperContributorEntity.Create(
            id: Guid.NewGuid(),
            sectionRole: AuthorizeConstants.SectionEdit,
            paperId: dto.PaperId,
            sectionId: dto.SectionId!.Value,
            memberId: memberId,
            markSectionId: dto.SectionId.Value);

        session.Store(newContributor);
        return newContributor;
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
