namespace Management.Application.Dtos.Members;

public class AddProjectManagersDto
{
    public List<Guid> UserIds { get; set; } = new();
}