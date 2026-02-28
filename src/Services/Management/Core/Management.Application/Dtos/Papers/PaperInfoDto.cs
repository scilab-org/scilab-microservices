namespace Management.Application.Dtos.Papers;

public sealed class PaperInfoDto
{
    #region Fields, Properties and Indexers

    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Abstract { get; set; }
    public string? Doi { get; set; }
    public string? FilePath { get; set; }
    public string? Status { get; set; }
    public DateTimeOffset? PublicationDate { get; set; }
    public string? PaperType { get; set; }
    public string? JournalName { get; set; }
    public string? ConferenceName { get; set; }

    #endregion
}
