using Management.Application.Dtos.Abstractions;
using Management.Domain.Enums;

namespace Management.Application.Dtos.Projects;

public class ProjectInfoDto : DtoId<Guid>
{
    #region Fields, Properties and Indexers

    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public ProjectStatus? Status { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }

    #endregion
}