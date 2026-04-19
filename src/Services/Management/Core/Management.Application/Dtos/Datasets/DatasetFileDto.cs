using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Datasets;

[ExcludeFromCodeCoverage]
public class DatasetFileDto
{
    #region Fields, Properties and Indexers

    public string? FileId { get; set; }

    public string? OriginalFileName { get; set; }

    public string? FileName { get; set; }

    public string? PublicURL { get; set; }

    #endregion
}