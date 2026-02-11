using Management.Domain.Enums;

namespace Management.Application.Models.Filters;

public record class GetProjectsFilter
{
    public string? Name { get; set; } = null!;
    public string? Code { get; set; }
    public ProjectStatus? Status { get; set; }
    public bool? IsDeleted { get; set; } = false;
}