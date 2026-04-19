using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Models.Filters;

[ExcludeFromCodeCoverage]
public class GetProjectMembersFilter
{
    public string? SearchEmail { get; set; }
    public string? ProjectRole { get; set; }
}