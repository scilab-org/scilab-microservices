using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Papers;

[ExcludeFromCodeCoverage]
public class AssignedPaperDto : PaperInfoDto
{
    public Guid ProjectId { get; set; }
    public string? ProjectCode { get; set; }
}