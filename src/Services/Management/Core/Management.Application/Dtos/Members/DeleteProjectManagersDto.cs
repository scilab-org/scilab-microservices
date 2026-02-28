namespace Management.Application.Dtos.Members;

public sealed class DeleteProjectManagersDto
{
    public List<Guid> MemberIds { get; set; } = new();
}

