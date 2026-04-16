using Lab.Application.Dtos.Abstractions;
using Lab.Domain.Enums;

namespace Lab.Application.Dtos.Tasks;

public class TaskInfoDto: DtoId<Guid>
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public TaskType TaskType { get; set; }
    public Guid MemberId { get; set; }
    public TaskDefineStatus Status { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? NextReviewDate { get; set; }
    public DateTimeOffset? CompleteDate { get; set; }
    public Guid PaperId { get; set; }
    public Guid SubProjectId { get; set; }
    public Guid ProjectId { get; set; }
    public string PaperTitle { get; set; } = null!;
    public Guid PaperContributorId { get; set; }
    public  Guid? SectionId { get; set; }
    public string? SectionTitle { get; set; }
    public string AssignedToUserName { get; set; } = null!;
}