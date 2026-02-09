using System.ComponentModel.DataAnnotations;
using Common.Constants;
using Lab.Domain.Enums;

namespace Lab.Api.Models.Papers;

public class CreatePaperRequest
{
    #region Fields, Properties and Indexers

    public string Title { get; set; } = null!;
    public string? Abstract { get; set; }
    public string? Doi { get; set; }
    public IFormFile? File { get; set; }
    public PaperStatus? Status { get; set; }
    public DateTimeOffset? PublicationDate { get; set; }
    public string? PaperType { get; set; }
    public string? JournalName { get; set; }
    public string? ConferenceName { get; set; }

    #endregion
}