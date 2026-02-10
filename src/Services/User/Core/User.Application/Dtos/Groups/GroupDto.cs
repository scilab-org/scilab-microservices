namespace User.Application.Dtos.Groups;

public sealed class GroupDto
{
    #region Fields, Properties and Indexers

    public string Id { get; set; } = default!;

    public string? Name { get; set; }

    public string? Path { get; set; }

    public List<GroupDto>? SubGroups { get; set; }

    #endregion
}
