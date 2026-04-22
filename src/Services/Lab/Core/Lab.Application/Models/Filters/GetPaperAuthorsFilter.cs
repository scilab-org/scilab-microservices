namespace Lab.Application.Models.Filters;

public sealed record class GetPaperAuthorsFilter
{
    public string? Name { get; set; }
    public string? RoleName { get; set; }
    public Guid? PaperId { get; set; }
}
