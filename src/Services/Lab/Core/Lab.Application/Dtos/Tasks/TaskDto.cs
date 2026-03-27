using Lab.Application.Dtos.Abstractions;

namespace Lab.Application.Dtos.Tasks;

public class TaskDto: TaskInfoDto, IAuditableDto
{
    public string? CreatedBy { get; set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset? LastModifiedOnUtc { get; set; }
    public string? LastModifiedBy { get; set; }
}