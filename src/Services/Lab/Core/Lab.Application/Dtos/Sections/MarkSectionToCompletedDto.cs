namespace Lab.Application.Dtos.Sections;

public class MarkSectionToCompletedDto
{
    public Guid MemberId { get; init; }
    public Guid ProjectId { get; init; }
}