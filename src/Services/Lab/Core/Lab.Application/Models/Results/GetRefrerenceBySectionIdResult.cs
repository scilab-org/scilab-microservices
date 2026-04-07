using Lab.Domain.Enums;

namespace Lab.Application.Models.Results;

public class GetRefrerenceBySectionIdResult
{
    public List<ReferencePaperBankDto> InUse { get; set; } = new();

    public List<PaperBankReferenceDto> OtherReference { get; set; } = new();
}

public class PaperBankReferenceDto
{
    public ReferencePaperBankDto PaperBank { get; set; } = null!;

    public List<ReferenceSectionDto> Sections { get; set; } = new();
}

public class ReferencePaperBankDto
{
    public Guid Id { get; set; }

    public string? Title { get; set; }

    public string? Authors { get; set; }

    public string? Publisher { get; set; }

    public string? Abstract { get; set; }

    public string? Doi { get; set; }

    public string? FilePath { get; set; }

    public PaperStatus? Status { get; set; }

    public bool? IsIngested { get; set; }

    public bool? IsAutoTagged { get; set; }

    public DateTimeOffset? PublicationDate { get; set; }

    public string? PaperType { get; set; }

    public string? JournalName { get; set; }

    public string? Pages {get; set;}

    public string? Number { get; set; }

    public string? Volume {get; set;}

    public string? ConferenceName { get; set; }

    public string? ReferenceContent { get; set; }

    public List<string> TagNames { get; set; } = new();
}

public class ReferenceSectionDto
{
    public Guid Id { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public float DisplayOrder { get; set; }

    public Guid PaperId { get; set; }

    public string? CreatedBy { get; set; }
}