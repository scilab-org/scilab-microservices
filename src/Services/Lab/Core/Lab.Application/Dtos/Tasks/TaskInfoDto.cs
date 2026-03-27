using Lab.Application.Dtos.Abstractions;
using Lab.Domain.Enums;

namespace Lab.Application.Dtos.Tasks;

public class TaskInfoDto: DtoId<Guid>
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? AssignedToUserName  { get; set; }
    public TaskDefineStatus Status { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? NextReviewDate { get; set; }
    public DateTimeOffset? CompleteDate { get; set; }
    public Guid PaperId { get; set; }
    public string PaperTitle { get; set; } = null!;
    public Guid PaperContributorId { get; set; }
}