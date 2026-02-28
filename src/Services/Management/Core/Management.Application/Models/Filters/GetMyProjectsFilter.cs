using Management.Domain.Enums;

namespace Management.Application.Models.Filters;

public class GetMyProjectsFilter
{
    public string? Name { get; set; } = null!;
    public string? Code { get; set; }
    public ProjectStatus? Status { get; set; }
}