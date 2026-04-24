using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Papers;

[ExcludeFromCodeCoverage]
public class PaperBankInfoDto
{
    #region Fields, Properties and Indexers

    public Guid Id { get; set; }
    public Guid? SubProjectId { get; set; }
    public string? Title { get; set; }
    public string? Authors { get; set; }
    public string? Publisher { get; set; }
    public string? Abstract { get; set; }
    public string? Doi { get; set; }
    public string? FilePath { get; set; }
    public int Status { get; set; }
    public int? SubmissionStatus { get; set; }
    public DateTimeOffset? PublicationDate { get; set; }
    public string? PaperType { get; set; }
    public string? JournalName { get; set; }
    public string? Pages { get; set; }
    public string? Number { get; set; }
    public string? Volume { get; set; }
    public string? ConferenceName { get; set; }
    public string? ReferenceContent { get; set; }
    public List<string> Keywords { get; set; } = new();
    public string? CreatedBy { get; set; }
    #endregion
}