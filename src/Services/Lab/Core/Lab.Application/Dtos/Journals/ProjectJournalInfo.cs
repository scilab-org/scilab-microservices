namespace Lab.Application.Dtos.Journals;

public class ProjectJournalInfo
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string Code { get; init; } = null!;
}