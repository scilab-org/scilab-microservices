namespace Lab.Domain.Models;

public class ParsedChunk
{
    #region Fields, Properties and Indexers

    public string Text { get; set; } = null!;
    public List<string>? Headings { get; set; }

    #endregion
}