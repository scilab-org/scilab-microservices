namespace Management.Application.Dtos.Members;

public class MemberDto
{
    public Guid UserId { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectRole { get; set; } = string.Empty;
    public DateTimeOffset JoinedAt { get; set; }
}