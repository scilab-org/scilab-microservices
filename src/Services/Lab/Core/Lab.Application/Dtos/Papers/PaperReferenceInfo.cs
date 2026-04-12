namespace Lab.Application.Dtos.Papers;

public class PaperReferenceInfo
{
    public Guid PaperBankId { get; set; }
    public List<Guid> SectionIds { get; set; } = new();
}