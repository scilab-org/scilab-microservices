namespace Management.Domain.Tests.Entities;

public sealed class MemberEntityTests
{
    [Fact]
    public void Create_ShouldInitializeEntityWithCorrectValues()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        const string projectRole = "project:project-manager";
        var joinedAt = DateTimeOffset.UtcNow;

        // Act
        var entity = MemberEntity.Create(id, userId, projectId, projectRole, joinedAt);

        // Assert
        entity.Id.Should().Be(id);
        entity.UserId.Should().Be(userId);
        entity.ProjectId.Should().Be(projectId);
        entity.ProjectRole.Should().Be(projectRole);
        entity.JoinedAt.Should().Be(joinedAt);
        entity.CreatedOnUtc.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
        entity.LastModifiedOnUtc.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void UpdateProjectRole_ShouldUpdateRoleAndLastModified()
    {
        // Arrange
        var entity = MemberEntity.Create(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "project:member", DateTimeOffset.UtcNow);
        const string newRole = "project:project-manager";

        // Act
        entity.UpdateProjectRole(newRole);

        // Assert
        entity.ProjectRole.Should().Be(newRole);
        entity.LastModifiedOnUtc.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
    }
}
