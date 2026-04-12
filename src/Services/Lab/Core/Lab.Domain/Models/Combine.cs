namespace Lab.Domain.Models;

public class Combine
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Content { get; set; }
    public List<Guid>? References { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public string? LastModifiedBy { get; set; }
    public DateTimeOffset LastModifiedOnUtc { get; set; }
}