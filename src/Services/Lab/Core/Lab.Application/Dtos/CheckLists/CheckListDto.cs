using Lab.Application.Dtos.Abstractions;

namespace Lab.Application.Dtos.CheckLists;

public class CheckListDto : IAuditableDto
{
    public Guid Id { get; set; }
    public string Section { get; set; } = null!;
    public List<CheckListItemDto> Items { get; set; } = [];
    public DateTimeOffset CreatedOnUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? LastModifiedOnUtc { get; set; }
    public string? LastModifiedBy { get; set; }
}
