namespace Management.Application.Models.Filters;

public sealed class GetAssignedPapersFilter
{
    public string? Title { get; set; }
    public string? ProjectName { get; set; }
    public string? ProjectCode { get; set; }
    public Guid? ProjectId { get; set; }
}
