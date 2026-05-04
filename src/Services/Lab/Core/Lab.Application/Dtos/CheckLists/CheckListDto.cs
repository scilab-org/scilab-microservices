using Lab.Application.Dtos.Abstractions;

namespace Lab.Application.Dtos.CheckLists;

public class CheckListDto : IAuditableDto
{
    public Guid Id { get; set; }
    public string Section { get; set; } = null!;
    public string RuleName { get; set; } = null!;
    public string Item { get; set; } = null!;
    public int Weight { get; set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? LastModifiedOnUtc { get; set; }
    public string? LastModifiedBy { get; set; }
}
