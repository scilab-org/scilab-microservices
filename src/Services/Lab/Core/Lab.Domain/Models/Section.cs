namespace Lab.Domain.Models;

public class Section
{
    public string? Title { get; set; } = null;
    public string? SectionRule { get; set; }  = null;
    public int DisplayOrder { get; set; }
}