namespace Management.Application.Dtos.Papers;

public sealed class SubmissionStatusSummaryItem
{
    public int Status { get; set; }
    public int Count { get; set; }
}

public sealed class SubmissionStatusSummaryResult
{
    public List<SubmissionStatusSummaryItem> Items { get; set; } = [];
}
