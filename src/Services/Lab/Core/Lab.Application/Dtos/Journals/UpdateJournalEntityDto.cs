namespace Lab.Application.Dtos.Journals;

/// <summary>
/// DTO for updating an existing Journal Entity
/// </summary>
public class UpdateJournalEntityDto
{
    public DateTimeOffset? StartAt { get; set; }
    public DateTimeOffset? EndAt { get; set; }
}