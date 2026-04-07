using Lab.Application.Dtos.PaperBanks;

namespace Lab.Application.Models.Results;

public class GetInUseReferenceBySectionIdResult
{
    public string? ReferenceContent { get; set; }

    public List<PaperBankInfoDto> PaperBanks { get; set; } = new();
}