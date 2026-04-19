using Management.Domain.Enums;

using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Models.Filters;

[ExcludeFromCodeCoverage]
public class GetMyProjectsFilter
{
    public string? Name { get; set; } = null!;
    public string? Code { get; set; }
    public ProjectStatus? Status { get; set; }
}