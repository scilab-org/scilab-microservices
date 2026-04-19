namespace Management.Application.Tests.Common.TestData;

public static class MemberTestData
{
    public static MemberEntity CreateMemberEntity(
        Guid? id = null,
        Guid? userId = null,
        Guid? projectId = null,
        string projectRole = "project:member",
        DateTimeOffset? joinedAt = null)
    {
        return MemberEntity.Create(
            id: id ?? Guid.NewGuid(),
            userId: userId ?? Guid.NewGuid(),
            projectId: projectId ?? Guid.NewGuid(),
            projectRole: projectRole,
            joinedAt: joinedAt ?? DateTimeOffset.UtcNow);
    }

    public static AddProjectMembersDto CreateAddProjectMembersDto(
        List<ProjectMemberEntry>? members = null)
    {
        return new AddProjectMembersDto
        {
            Members = members ?? new List<ProjectMemberEntry>
            {
                new() { UserId = Guid.NewGuid(), GroupName = AuthorizeConstants.ProjectMember }
            }
        };
    }

    public static AddProjectManagersDto CreateAddProjectManagersDto(
        Guid? userId = null)
    {
        return new AddProjectManagersDto
        {
            UserId = userId ?? Guid.NewGuid()
        };
    }

    public static UserInfoDto CreateUserInfoDto(
        Guid? id = null,
        string? username = "testuser",
        string? email = "test@example.com",
        string? firstName = "Test",
        string? lastName = "User",
        bool enabled = true)
    {
        return new UserInfoDto
        {
            Id = (id ?? Guid.NewGuid()).ToString(),
            Username = username,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Enabled = enabled
        };
    }
}
