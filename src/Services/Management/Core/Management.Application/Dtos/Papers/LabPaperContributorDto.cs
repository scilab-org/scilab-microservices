using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Papers;

[ExcludeFromCodeCoverage]
public sealed class LabPaperContributorDto
{
    public Guid Id { get; set; }
    public Guid PaperId { get; set; }
    public Guid MemberId { get; set; }
    public Guid MarkSectionId { get; set; }
    public Guid? SectionId { get; set; }
    public string? SectionRole { get; set; }
    public Guid UserId { get; set; }
}
