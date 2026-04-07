namespace Lab.Application.Dtos.Projects;

public sealed class UpdateProjectRulesDto
{
    public List<Guid> PaperIds { get; init; } = [];
    public string? Context { get; init; }
    public string? Domain { get; init; }
    public string? Keypoint { get; init; }
}