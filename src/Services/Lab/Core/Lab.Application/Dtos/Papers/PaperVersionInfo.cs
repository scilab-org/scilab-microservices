namespace Lab.Application.Dtos.Papers;

public class PaperVersionInfo
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public string? Content { get; set; }
    public List<Guid>? References { get; set; }
    public List<string>? Files { get; set; }
    public string? CreatedBy { get; set; }
    public List<VersionFileInfor>? VersionFiles { get; set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public string? LastModifiedBy { get; set; }
    public DateTimeOffset LastModifiedOnUtc { get; set; }
}