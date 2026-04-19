using Management.Application.Dtos.Abstractions;

using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Projects;

[ExcludeFromCodeCoverage]
public class ProjectDto :  ProjectInfoDto, IAuditableDto
{
    public DateTimeOffset CreatedOnUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? LastModifiedOnUtc { get; set; }
    public string? LastModifiedBy { get; set; }
}