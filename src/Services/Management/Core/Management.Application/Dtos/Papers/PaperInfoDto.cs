namespace Management.Application.Dtos.Papers;

/// <summary>
/// Full paper DTO — mirrors Lab.Application.Dtos.Papers.PaperDto (GET /papers/{id}).
/// Contains Paper-entity-specific fields (Template, ParsedText) in addition to the
/// shared fields present on PaperBankInfoDto.
/// </summary>
public sealed class PaperInfoDto : PaperBankInfoDto
{
    #region Fields, Properties and Indexers

    /// <summary>Template code/name used for the paper structure.</summary>
    public string? Template { get; set; }

    /// <summary>Extracted plain-text content from the paper file.</summary>
    public string? ParsedText { get; set; }

    #endregion
}
