namespace Lab.Domain.Models;

public class ParsedText
{
    #region Fields, Properties and Indexers

    public List<ParsedChunk> Chunks { get; set; } = new();

    #endregion
}