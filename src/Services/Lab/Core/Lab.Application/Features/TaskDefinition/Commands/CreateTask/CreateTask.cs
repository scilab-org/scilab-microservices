using Lab.Application.Dtos.Tasks;
using Lab.Application.Services;
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
        
        var paper = await session.LoadAsync<PaperEntity>(dto.PaperId, cancellationToken);
        if(paper == null)
            throw new NotFoundException(MessageCode.PaperIsNotExists);
        var memberInfo = await apiService.GetMemberByPaperIdAsync(dto.PaperId, Guid.Parse(request.UserId), cancellationToken);
        if (memberInfo == null)
            throw new NotFoundException(MessageCode.MemberNotFound, request.UserId.ToString());

        var (subProjectId, memberId) = memberInfo.Value;
            
        var paperContributor = await session.Query<PaperContributorEntity>()
            .Where(x => x.PaperId == dto.PaperId && x.MemberId == memberId)
            .FirstOrDefaultAsync(cancellationToken);
        if (paperContributor == null)
            throw new NotFoundException(MessageCode.PaperContributorNotFound, request.UserId);
        
        
        var entity = TaskEntity.Create(Guid.NewGuid(),
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
}
            
            