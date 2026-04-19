namespace Lab.Domain.Tests.Entities;

public sealed class PaperContributorEntityTests
{
    [Fact]
    public void Create_ShouldInitializeCorrectly()
    {
        var id = Guid.NewGuid();
        var paperId = Guid.NewGuid();
        var sectionId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var markSectionId = Guid.NewGuid();

        var entity = PaperContributorEntity.Create(id, "Author", paperId, sectionId, memberId, markSectionId);

        entity.Id.Should().Be(id);
        entity.SectionRole.Should().Be("Author");
        entity.PaperId.Should().Be(paperId);
        entity.SectionId.Should().Be(sectionId);
        entity.MemberId.Should().Be(memberId);
        entity.MarkSectionId.Should().Be(markSectionId);
        entity.TaskIds.Should().BeEmpty();
        entity.CreatedOnUtc.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Create_ShouldAllowNullSectionId()
    {
        var entity = PaperContributorEntity.Create(Guid.NewGuid(), "Reviewer",
            Guid.NewGuid(), null, Guid.NewGuid(), Guid.NewGuid());
        entity.SectionId.Should().BeNull();
    }

    [Fact]
    public void Update_ShouldUpdateProvidedValues()
    {
        var entity = PaperContributorEntity.Create(Guid.NewGuid(), "Author",
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var newSectionId = Guid.NewGuid();
        var newMemberId = Guid.NewGuid();
        var newMarkSectionId = Guid.NewGuid();

        entity.Update(sectionId: newSectionId, memberId: newMemberId,
            markSectionId: newMarkSectionId, sectionRole: "Editor");

        entity.SectionId.Should().Be(newSectionId);
        entity.MemberId.Should().Be(newMemberId);
        entity.MarkSectionId.Should().Be(newMarkSectionId);
        entity.SectionRole.Should().Be("Editor");
    }

    [Fact]
    public void Update_ShouldKeepExisting_WhenNullsPassed()
    {
        var entity = PaperContributorEntity.Create(Guid.NewGuid(), "Author",
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var originalRole = entity.SectionRole;
        entity.Update();
        entity.SectionRole.Should().Be(originalRole);
    }

    [Fact]
    public void AddTasks_ShouldAddTaskId()
    {
        var entity = PaperContributorEntity.Create(Guid.NewGuid(), "Author",
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var taskId = Guid.NewGuid();

        entity.AddTasks(taskId);

        entity.TaskIds.Should().ContainSingle().Which.Should().Be(taskId);
    }

    [Fact]
    public void AddTasks_ShouldAddMultipleTaskIds()
    {
        var entity = PaperContributorEntity.Create(Guid.NewGuid(), "Author",
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var taskId1 = Guid.NewGuid();
        var taskId2 = Guid.NewGuid();

        entity.AddTasks(taskId1);
        entity.AddTasks(taskId2);

        entity.TaskIds.Should().HaveCount(2);
    }

    [Fact]
    public void RemoveTasks_ShouldRemoveMatchingTaskId()
    {
        var entity = PaperContributorEntity.Create(Guid.NewGuid(), "Author",
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var taskId1 = Guid.NewGuid();
        var taskId2 = Guid.NewGuid();
        entity.AddTasks(taskId1);
        entity.AddTasks(taskId2);

        entity.RemoveTasks(taskId1);

        entity.TaskIds.Should().ContainSingle().Which.Should().Be(taskId2);
    }

    [Fact]
    public void RemoveTasks_ShouldNotFail_WhenTaskIdNotFound()
    {
        var entity = PaperContributorEntity.Create(Guid.NewGuid(), "Author",
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        entity.RemoveTasks(Guid.NewGuid());
        entity.TaskIds.Should().BeEmpty();
    }
}
