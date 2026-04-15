namespace Management.Application.Dtos.Members;

public class MemberTaskRequestDto
{
    public List<Guid> TaskIds { get; set; } = new();
}
