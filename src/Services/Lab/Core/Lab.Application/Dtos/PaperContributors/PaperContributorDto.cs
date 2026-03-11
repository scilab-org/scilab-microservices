namespace Lab.Application.Dtos.PaperContributors;

/// <summary>A contributor of a paper, enriched with user Name and Email from UserService.</summary>
public class PaperContributorDto
{
    public Guid Id { get; set; }
    public Guid PaperId { get; set; }
    public Guid MemberId { get; set; }
    public Guid MarkSectionId { get; set; }
    public Guid? SectionId { get; set; }
    public string SectionRole { get; set; } = null!;

    // Enriched from UserService via Management
    public Guid UserId { get; set; }
    public string? ContributorName { get; set; }
    public string? ContributorEmail { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

