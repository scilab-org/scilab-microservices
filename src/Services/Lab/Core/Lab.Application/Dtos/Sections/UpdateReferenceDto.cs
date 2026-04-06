namespace Lab.Application.Dtos.Sections;

public class UpdateReferenceDto
{
    public Guid PaperId { get; init; }
    public List<Guid> PaperBankIds { get; init; } = null!;
}