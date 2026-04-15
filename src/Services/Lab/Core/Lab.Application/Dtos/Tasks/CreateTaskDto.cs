using Lab.Domain.Enums;

namespace Lab.Application.Dtos.Tasks;

public class CreateTaskDto
{
    public Guid PaperId { get; set; }
    public Guid? SectionId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? AssignedToUserName  { get; set; }
    public TaskDefineStatus Status { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? NextReviewDate { get; set; }
    public DateTimeOffset? CompleteDate { get; set; }
}
