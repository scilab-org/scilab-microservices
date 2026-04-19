using Lab.Application.Services;

namespace Lab.Application.Tests.Services;

public sealed class ServiceRecordTests
{
    [Fact]
    public void SubProjectMemberInfo_ShouldRetainValues()
    {
        var memberId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var info = new SubProjectMemberInfo(memberId, userId, "Author", "user", "user@test.com", "First", "Last");

        info.MemberId.Should().Be(memberId);
        info.UserId.Should().Be(userId);
        info.Role.Should().Be("Author");
        info.Username.Should().Be("user");
        info.Email.Should().Be("user@test.com");
        info.FirstName.Should().Be("First");
        info.LastName.Should().Be("Last");
    }

    [Fact]
    public void ManagementMemberInfo_ShouldRetainValues()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var info = new ManagementMemberInfo(id, userId, projectId, "Manager");

        info.Id.Should().Be(id);
        info.UserId.Should().Be(userId);
        info.ProjectId.Should().Be(projectId);
        info.ProjectRole.Should().Be("Manager");
    }

    [Fact]
    public void UserInfo_ShouldRetainValues()
    {
        var id = Guid.NewGuid();
        var info = new UserInfo(id, "user", "user@test.com", "First", "Last");

        info.Id.Should().Be(id);
        info.Username.Should().Be("user");
        info.Email.Should().Be("user@test.com");
        info.FirstName.Should().Be("First");
        info.LastName.Should().Be("Last");
    }
}
