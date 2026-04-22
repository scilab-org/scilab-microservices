using Management.Domain.Enums;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Projects;

[ExcludeFromCodeCoverage]
public class CreateProjectDto
{
    #region Fields, Properties and Indexers

    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public ProjectStatus? Status { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public List<Guid> DomainIds { get; set; } = [];
    public string? Context { get; set; }
    public string? Keypoint { get; set; }

    #endregion
}
