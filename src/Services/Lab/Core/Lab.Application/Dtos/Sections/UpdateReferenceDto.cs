namespace Lab.Application.Dtos.Sections;

public class UpdateReferenceDto
{
    public Guid PaperId { get; init; }
    public string Content { get; init; } = null!;
    public List<Guid> PaperBankIds { get; init; } = null!;
}