using Lab.Application.Dtos.Abstractions;

namespace Lab.Application.Dtos.PaperContributors;

/// <summary>A member assigned to a section, including their role and user info.</summary>
public class SectionMemberDto : DtoId<Guid>
{
    public Guid PaperContributorId { get; set; }
    public Guid MemberId { get; set; }
    public Guid UserId { get; set; }
    public string SectionRole { get; set; } = null!;
    public Guid MarkSectionId { get; set; }
    public Guid? SectionId { get; set; }
    // User info enriched from Management/User service
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

/// <summary>A sub-project member not yet assigned to the section.</summary>
public class AvailableSectionMemberDto
{
    public Guid MemberId { get; set; }
    public Guid UserId { get; set; }
    public string Role { get; set; } = null!;
    // User info enriched from Management/User service
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
