using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Papers;

[ExcludeFromCodeCoverage]
public sealed class SubmissionStatusSummaryItem
{
    public int Status { get; set; }
    public int Count { get; set; }
}

[ExcludeFromCodeCoverage]
public sealed class SubmissionStatusSummaryResult
{
    public List<SubmissionStatusSummaryItem> Items { get; set; } = [];
}
