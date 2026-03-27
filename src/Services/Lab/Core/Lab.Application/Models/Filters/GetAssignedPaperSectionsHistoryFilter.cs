namespace Lab.Application.Models.Filters;

public class GetAssignedPaperSectionsHistoryFilter
{
    public string? SectionRole { get; set; }
    public DateTimeOffset? FromDate { get; set; }
    public DateTimeOffset? ToDate { get; set; }
}
