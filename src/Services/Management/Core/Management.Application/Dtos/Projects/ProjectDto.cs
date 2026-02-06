using Management.Application.Dtos.Abstractions;

namespace Management.Application.Dtos.Projects;

public class ProjectDto :  ProjectInfoDto, IAuditableDto
{
    public DateTimeOffset CreatedOnUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? LastModifiedOnUtc { get; set; }
    public string? LastModifiedBy { get; set; }
}