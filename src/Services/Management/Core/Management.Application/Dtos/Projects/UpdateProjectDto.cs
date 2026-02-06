using Management.Domain.Enums;

namespace Management.Application.Dtos.Projects;

public class UpdateProjectDto
{
    #region Fields, Properties and Indexers

    public required string Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public ProjectStatus? Status { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }

    #endregion
}