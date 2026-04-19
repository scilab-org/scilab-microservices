namespace Lab.Domain.Tests.Entities;

public sealed class TaskEntityTests
{
    [Fact]
    public void Create_ShouldInitializeWithCorrectValues()
    {
        var id = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var start = DateTimeOffset.UtcNow;

        var entity = TaskEntity.Create(id, memberId, "Research Task",
            description: "Desc", assignedToUserName: "user1",
            status: TaskDefineStatus.InProgress, taskType: TaskType.Research,
            startDate: start, nextReviewDate: start.AddDays(7),
            completeDate: start.AddDays(30), createdBy: "admin");

        entity.Id.Should().Be(id);
        entity.MemberId.Should().Be(memberId);
        entity.Name.Should().Be("Research Task");
        entity.Description.Should().Be("Desc");
        entity.AssignedToUserName.Should().Be("user1");
        entity.Status.Should().Be(TaskDefineStatus.InProgress);
        entity.TaskType.Should().Be(TaskType.Research);
        entity.StartDate.Should().Be(start);
        entity.CreatedBy.Should().Be("admin");
    }

    [Fact]
    public void Create_ShouldUseDefaults()
    {
        var entity = TaskEntity.Create(Guid.NewGuid(), Guid.NewGuid(), "Task");

        entity.Status.Should().Be(TaskDefineStatus.ToDo);
        entity.TaskType.Should().Be(TaskType.Other);
        entity.Description.Should().BeNull();
    }

    [Fact]
    public void Update_ShouldUpdateProvidedValues()
    {
        var entity = TaskEntity.Create(Guid.NewGuid(), Guid.NewGuid(), "Old");
        var newMemberId = Guid.NewGuid();

        entity.Update(name: "New", description: "New Desc",
            memberId: newMemberId, status: TaskDefineStatus.Completed,
            startDate: DateTimeOffset.UtcNow,
            nextReviewDate: DateTimeOffset.UtcNow.AddDays(1),
            completeDate: DateTimeOffset.UtcNow);

        entity.Name.Should().Be("New");
        entity.Description.Should().Be("New Desc");
        entity.MemberId.Should().Be(newMemberId);
        entity.Status.Should().Be(TaskDefineStatus.Completed);
    }

    [Fact]
    public void Update_ShouldKeepExisting_WhenNullsPassed()
    {
        var entity = TaskEntity.Create(Guid.NewGuid(), Guid.NewGuid(), "Task");
        entity.Update();
        entity.Name.Should().Be("Task");
    }
}
