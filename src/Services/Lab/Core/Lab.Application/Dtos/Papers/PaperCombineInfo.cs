namespace Lab.Application.Dtos.Papers;

public class PaperCombineInfo
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public string? Content { get; set; }
    public List<Guid>? References { get; set; }
    public bool IsSave { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public string? LastModifiedBy { get; set; }
    public DateTimeOffset LastModifiedOnUtc { get; set; }
}