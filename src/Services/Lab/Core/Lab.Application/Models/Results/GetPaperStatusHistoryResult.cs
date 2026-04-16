using Lab.Application.Dtos.Papers;
using Lab.Domain.Enums;

namespace Lab.Application.Models.Results;

public class GetPaperStatusHistoryResult
{
    public Guid PaperId { get; init; }
    public SubmissionStatus CurrentStatus { get; init; }
    public List<PaperStatusHistoryDto> History { get; init; } = [];
}
