using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Models.Filters;

[ExcludeFromCodeCoverage]
public class GetAvailableSubProjectMembersFilter
{
    public string? SearchEmail { get; set; }
    public string? ProjectRole { get; set; }
}