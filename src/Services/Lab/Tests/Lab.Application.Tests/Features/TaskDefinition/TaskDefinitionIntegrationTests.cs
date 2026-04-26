using Common.Constants;
using Lab.Application.Dtos.Tasks;
using Lab.Application.Features.TaskDefinition.Commands.CreateTask;
using Lab.Application.Features.TaskDefinition.Commands.DeleteTask;
using Lab.Application.Features.TaskDefinition.Commands.UpdateTask;
using Lab.Application.Features.TaskDefinition.Queries.GetMyTask;
using Lab.Application.Features.TaskDefinition.Queries.GetTasksByPaperId;
using Lab.Application.Models.Filters;
using Lab.Application.Tests.Common;

namespace Lab.Application.Tests.Features.TaskDefinition;

public class TaskDefinitionIntegrationTests : MartenTestBase
{
    protected override string SchemaName => "task_definition_tests";

    private readonly Mock<IManagementApiService> _mockMgmt = new();
    private readonly Mock<IUserApiService> _mockUser = new();

    // ─── Helpers ──────────────────────────────────────────────────────────────

    private PaperEntity SeedPaper(string title = "Test Paper")
    {
        var paper = PaperEntity.Create(Guid.NewGuid(), title);
        Session.Store(paper);
        return paper;
    }

    private TaskEntity SeedTask(
        Guid? memberId = null,
        string name = "Test Task",
        string? createdBy = "author",
        TaskDefineStatus status = TaskDefineStatus.ToDo,
        TaskType type = TaskType.Other,
        string? assignedToUserName = null)
    {
        var task = TaskEntity.Create(
            id: Guid.NewGuid(),
            memberId: memberId ?? Guid.NewGuid(),
            name: name,
            createdBy: createdBy,
            status: status,
            taskType: type,
            assignedToUserName: assignedToUserName);
        Session.Store(task);
        return task;
    }

    private PaperContributorEntity SeedContributor(Guid paperId, Guid memberId, Guid sectionId, string role = AuthorizeConstants.PaperAuthor)
    {
        var contributor = PaperContributorEntity.Create(
            Guid.NewGuid(), role, paperId, sectionId, memberId, Guid.NewGuid());
        Session.Store(contributor);
        return contributor;
    }

    // ─── CreateTask ───────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateTask_WithNonExistentPaper_ShouldThrowNotFoundException()
    {
        var handler = new CreateTaskHandler(Session, _mockMgmt.Object, _mockUser.Object);
        var dto = new CreateTaskDto { PaperId = Guid.NewGuid(), Name = "Task", MemberId = Guid.NewGuid() };

        var act = () => handler.Handle(new CreateTaskCommand(dto, Guid.NewGuid().ToString(), "user"), CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateTask_WhenUserNotMember_ShouldThrowNotFoundException()
    {
        var paper = SeedPaper();
        await Session.SaveChangesAsync();

        _mockMgmt
            .Setup(x => x.GetMemberByPaperIdAsync(paper.Id, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ValueTuple<Guid, Guid, Guid>?)null);

        var handler = new CreateTaskHandler(Session, _mockMgmt.Object, _mockUser.Object);
        var userId = Guid.NewGuid();
        var dto = new CreateTaskDto { PaperId = paper.Id, Name = "Task", MemberId = Guid.NewGuid() };

        var act = () => handler.Handle(new CreateTaskCommand(dto, userId.ToString(), "user"), CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateTask_WhenNotPaperAuthorRole_ShouldThrowNoPermissionException()
    {
        var paper = SeedPaper();
        await Session.SaveChangesAsync();

        var subProjectId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _mockMgmt
            .Setup(x => x.GetMemberByPaperIdAsync(paper.Id, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((subProjectId, memberId, Guid.NewGuid()));
        _mockMgmt
            .Setup(x => x.GetMyProjectRoleAsync(subProjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync("project:member");

        var handler = new CreateTaskHandler(Session, _mockMgmt.Object, _mockUser.Object);
        var dto = new CreateTaskDto { PaperId = paper.Id, Name = "Task", MemberId = Guid.NewGuid() };

        var act = () => handler.Handle(new CreateTaskCommand(dto, userId.ToString(), "user"), CancellationToken.None);
        await act.Should().ThrowAsync<NoPermissionException>();
    }

    [Fact]
    public async Task CreateTask_WhenRoleIsNull_ShouldThrowNoPermissionException()
    {
        var paper = SeedPaper();
        await Session.SaveChangesAsync();

        var subProjectId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _mockMgmt
            .Setup(x => x.GetMemberByPaperIdAsync(paper.Id, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((subProjectId, memberId, Guid.NewGuid()));
        _mockMgmt
            .Setup(x => x.GetMyProjectRoleAsync(subProjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        var handler = new CreateTaskHandler(Session, _mockMgmt.Object, _mockUser.Object);
        var dto = new CreateTaskDto { PaperId = paper.Id, Name = "Task", MemberId = Guid.NewGuid() };

        var act = () => handler.Handle(new CreateTaskCommand(dto, userId.ToString(), "user"), CancellationToken.None);
        await act.Should().ThrowAsync<NoPermissionException>();
    }

    [Fact]
    public async Task CreateTask_WhenAssignedMemberNotFound_ShouldThrowNotFoundException()
    {
        var paper = SeedPaper();
        await Session.SaveChangesAsync();

        var subProjectId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var assignedMemberId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _mockMgmt
            .Setup(x => x.GetMemberByPaperIdAsync(paper.Id, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((subProjectId, memberId, Guid.NewGuid()));
        _mockMgmt
            .Setup(x => x.GetMyProjectRoleAsync(subProjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthorizeConstants.PaperAuthor);
        _mockMgmt
            .Setup(x => x.GetMemberByIdAsync(assignedMemberId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ManagementMemberInfo?)null);

        var handler = new CreateTaskHandler(Session, _mockMgmt.Object, _mockUser.Object);
        var dto = new CreateTaskDto { PaperId = paper.Id, Name = "Task", MemberId = assignedMemberId };

        var act = () => handler.Handle(new CreateTaskCommand(dto, userId.ToString(), "user"), CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateTask_WithValidData_ShouldStoreAndReturnId()
    {
        var paper = SeedPaper();
        await Session.SaveChangesAsync();

        var subProjectId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var assignedMemberId = Guid.NewGuid();
        var assignedUserId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _mockMgmt
            .Setup(x => x.GetMemberByPaperIdAsync(paper.Id, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((subProjectId, memberId, Guid.NewGuid()));
        _mockMgmt
            .Setup(x => x.GetMyProjectRoleAsync(subProjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthorizeConstants.PaperAuthor);
        _mockMgmt
            .Setup(x => x.GetMemberByIdAsync(assignedMemberId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ManagementMemberInfo(assignedMemberId, assignedUserId, subProjectId, AuthorizeConstants.PaperMember));
        _mockUser
            .Setup(x => x.GetUsersByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, UserInfo>
            {
                [assignedUserId] = new(assignedUserId, "assignee_user", "a@b.com", "First", "Last")
            });

        var handler = new CreateTaskHandler(Session, _mockMgmt.Object, _mockUser.Object);
        var dto = new CreateTaskDto
        {
            PaperId = paper.Id,
            Name = "New Task",
            MemberId = assignedMemberId,
            Status = TaskDefineStatus.ToDo,
            Type = TaskType.Other
        };

        var result = await handler.Handle(new CreateTaskCommand(dto, userId.ToString(), "author_user"), CancellationToken.None);

        result.Should().NotBeEmpty();
        var stored = await Session.LoadAsync<TaskEntity>(result);
        stored.Should().NotBeNull();
        stored!.Name.Should().Be("New Task");
        stored.AssignedToUserName.Should().Be("assignee_user");
    }

    [Fact]
    public async Task CreateTask_WhenUserNotInUserService_ShouldStoreWithNullUsername()
    {
        var paper = SeedPaper();
        await Session.SaveChangesAsync();

        var subProjectId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var assignedMemberId = Guid.NewGuid();
        var assignedUserId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _mockMgmt
            .Setup(x => x.GetMemberByPaperIdAsync(paper.Id, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((subProjectId, memberId, Guid.NewGuid()));
        _mockMgmt
            .Setup(x => x.GetMyProjectRoleAsync(subProjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthorizeConstants.PaperAuthor);
        _mockMgmt
            .Setup(x => x.GetMemberByIdAsync(assignedMemberId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ManagementMemberInfo(assignedMemberId, assignedUserId, subProjectId, AuthorizeConstants.PaperMember));
        _mockUser
            .Setup(x => x.GetUsersByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, UserInfo>());

        var handler = new CreateTaskHandler(Session, _mockMgmt.Object, _mockUser.Object);
        var dto = new CreateTaskDto { PaperId = paper.Id, Name = "Task No User", MemberId = assignedMemberId };

        var result = await handler.Handle(new CreateTaskCommand(dto, userId.ToString(), "author_user"), CancellationToken.None);

        var stored = await Session.LoadAsync<TaskEntity>(result);
        stored!.AssignedToUserName.Should().BeNull();
    }

    // ─── DeleteTask ───────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteTask_WithNonExistentId_ShouldThrowNotFoundException()
    {
        var handler = new DeleteTaskCommandHandler(Session, _mockMgmt.Object);
        var act = () => handler.Handle(new DeleteTaskCommand(Guid.NewGuid(), Guid.NewGuid().ToString(), "user"), CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteTask_WhenNotCreator_ShouldThrowNoPermissionException()
    {
        var task = SeedTask(createdBy: "creator_user");
        await Session.SaveChangesAsync();

        var handler = new DeleteTaskCommandHandler(Session, _mockMgmt.Object);
        var act = () => handler.Handle(new DeleteTaskCommand(task.Id, Guid.NewGuid().ToString(), "other_user"), CancellationToken.None);
        await act.Should().ThrowAsync<NoPermissionException>();
    }

    [Fact]
    public async Task DeleteTask_NonWritingType_ShouldDeleteTask()
    {
        var task = SeedTask(createdBy: "creator_user", type: TaskType.Other);
        await Session.SaveChangesAsync();

        var handler = new DeleteTaskCommandHandler(Session, _mockMgmt.Object);
        await handler.Handle(new DeleteTaskCommand(task.Id, Guid.NewGuid().ToString(), "creator_user"), CancellationToken.None);

        var deleted = await Session.LoadAsync<TaskEntity>(task.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeleteTask_WritingType_WithNoContributor_ShouldDeleteTask()
    {
        var task = SeedTask(createdBy: "creator_user", type: TaskType.Writing);
        await Session.SaveChangesAsync();

        var handler = new DeleteTaskCommandHandler(Session, _mockMgmt.Object);
        await handler.Handle(new DeleteTaskCommand(task.Id, Guid.NewGuid().ToString(), "creator_user"), CancellationToken.None);

        var deleted = await Session.LoadAsync<TaskEntity>(task.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeleteTask_WritingType_WithContributor_ShouldRemoveTaskFromContributorAndDelete()
    {
        var paperId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var task = SeedTask(memberId: memberId, createdBy: "creator_user", type: TaskType.Writing);
        var contributor = SeedContributor(paperId, memberId, Guid.NewGuid(), AuthorizeConstants.PaperAuthor);
        contributor.AddTasks(task.Id);
        Session.Store(contributor);
        await Session.SaveChangesAsync();

        var handler = new DeleteTaskCommandHandler(Session, _mockMgmt.Object);
        await handler.Handle(new DeleteTaskCommand(task.Id, Guid.NewGuid().ToString(), "creator_user"), CancellationToken.None);

        var deleted = await Session.LoadAsync<TaskEntity>(task.Id);
        deleted.Should().BeNull();
        var updatedContributor = await Session.LoadAsync<PaperContributorEntity>(contributor.Id);
        updatedContributor!.TaskIds.Should().NotContain(task.Id);
    }

    // ─── UpdateTask ───────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateTask_WithNonExistentId_ShouldThrowNotFoundException()
    {
        var handler = new UpdateTaskCommandHandler(Session, _mockMgmt.Object);
        var act = () => handler.Handle(
            new UpdateTaskCommand(Guid.NewGuid(), new UpdateTaskDto { Name = "X", Status = TaskDefineStatus.ToDo }, "user", Guid.NewGuid().ToString()),
            CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateTask_WhenAssigneeUpdates_ShouldUpdateWithoutRoleCheck()
    {
        var task = SeedTask(assignedToUserName: "assignee_user", status: TaskDefineStatus.ToDo);
        await Session.SaveChangesAsync();

        var handler = new UpdateTaskCommandHandler(Session, _mockMgmt.Object);
        var dto = new UpdateTaskDto { Name = "Updated Name", Status = TaskDefineStatus.InProgress };

        var result = await handler.Handle(
            new UpdateTaskCommand(task.Id, dto, "assignee_user", Guid.NewGuid().ToString()),
            CancellationToken.None);

        result.Should().Be(task.Id);
        var updated = await Session.LoadAsync<TaskEntity>(task.Id);
        updated!.Name.Should().Be("Updated Name");
        updated.Status.Should().Be(TaskDefineStatus.InProgress);
        _mockMgmt.Verify(x => x.GetMemberByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateTask_WhenNotAssigneeAndIsAuthor_ShouldUpdate()
    {
        var memberId = Guid.NewGuid();
        var subProjectId = Guid.NewGuid();
        var task = SeedTask(memberId: memberId, assignedToUserName: "someone_else");
        await Session.SaveChangesAsync();

        _mockMgmt
            .Setup(x => x.GetMemberByIdAsync(memberId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ManagementMemberInfo(memberId, Guid.NewGuid(), subProjectId, AuthorizeConstants.PaperAuthor));
        _mockMgmt
            .Setup(x => x.GetMyProjectRoleAsync(subProjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthorizeConstants.PaperAuthor);

        var handler = new UpdateTaskCommandHandler(Session, _mockMgmt.Object);
        var dto = new UpdateTaskDto { Name = "Author Updated", Status = TaskDefineStatus.InProgress };

        var result = await handler.Handle(
            new UpdateTaskCommand(task.Id, dto, "non_assignee_user", Guid.NewGuid().ToString()),
            CancellationToken.None);

        result.Should().Be(task.Id);
        var updated = await Session.LoadAsync<TaskEntity>(task.Id);
        updated!.Name.Should().Be("Author Updated");
    }

    [Fact]
    public async Task UpdateTask_WhenNotAssigneeAndMemberNotFound_ShouldThrowNoPermissionException()
    {
        var memberId = Guid.NewGuid();
        var task = SeedTask(memberId: memberId, assignedToUserName: "someone_else");
        await Session.SaveChangesAsync();

        _mockMgmt
            .Setup(x => x.GetMemberByIdAsync(memberId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ManagementMemberInfo?)null);

        var handler = new UpdateTaskCommandHandler(Session, _mockMgmt.Object);
        var dto = new UpdateTaskDto { Name = "X", Status = TaskDefineStatus.ToDo };

        var act = () => handler.Handle(
            new UpdateTaskCommand(task.Id, dto, "other_user", Guid.NewGuid().ToString()),
            CancellationToken.None);
        await act.Should().ThrowAsync<NoPermissionException>();
    }

    [Fact]
    public async Task UpdateTask_WhenNotAssigneeAndNotAuthorRole_ShouldThrowNoPermissionException()
    {
        var memberId = Guid.NewGuid();
        var subProjectId = Guid.NewGuid();
        var task = SeedTask(memberId: memberId, assignedToUserName: "someone_else");
        await Session.SaveChangesAsync();

        _mockMgmt
            .Setup(x => x.GetMemberByIdAsync(memberId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ManagementMemberInfo(memberId, Guid.NewGuid(), subProjectId, "project:member"));
        _mockMgmt
            .Setup(x => x.GetMyProjectRoleAsync(subProjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync("project:member");

        var handler = new UpdateTaskCommandHandler(Session, _mockMgmt.Object);
        var dto = new UpdateTaskDto { Name = "X", Status = TaskDefineStatus.ToDo };

        var act = () => handler.Handle(
            new UpdateTaskCommand(task.Id, dto, "other_user", Guid.NewGuid().ToString()),
            CancellationToken.None);
        await act.Should().ThrowAsync<NoPermissionException>();
    }

    [Fact]
    public async Task UpdateTask_WhenCompletedStatus_ShouldSetCompleteDate()
    {
        var task = SeedTask(assignedToUserName: "assignee_user", status: TaskDefineStatus.InProgress);
        await Session.SaveChangesAsync();

        var handler = new UpdateTaskCommandHandler(Session, _mockMgmt.Object);
        var dto = new UpdateTaskDto { Name = task.Name, Status = TaskDefineStatus.Completed };

        await handler.Handle(
            new UpdateTaskCommand(task.Id, dto, "assignee_user", Guid.NewGuid().ToString()),
            CancellationToken.None);

        var updated = await Session.LoadAsync<TaskEntity>(task.Id);
        updated!.CompleteDate.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateTask_WhenStatusChangedFromCompleted_ShouldClearCompleteDate()
    {
        var task = SeedTask(assignedToUserName: "assignee_user", status: TaskDefineStatus.Completed);
        task.CompleteDate = DateTimeOffset.UtcNow;
        Session.Store(task);
        await Session.SaveChangesAsync();

        var handler = new UpdateTaskCommandHandler(Session, _mockMgmt.Object);
        var dto = new UpdateTaskDto { Name = task.Name, Status = TaskDefineStatus.InProgress };

        await handler.Handle(
            new UpdateTaskCommand(task.Id, dto, "assignee_user", Guid.NewGuid().ToString()),
            CancellationToken.None);

        var updated = await Session.LoadAsync<TaskEntity>(task.Id);
        updated!.CompleteDate.Should().BeNull();
    }

    // ─── GetMyTask ────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetMyTask_WhenUserNotResolved_ShouldReturnEmpty()
    {
        _mockUser
            .Setup(x => x.GetUsersByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, UserInfo>());

        var handler = new GetMyTaskQueryHandler(Session, Mapper, _mockUser.Object, _mockMgmt.Object);
        var result = await handler.Handle(
            new GetMyTaskQuery(Guid.NewGuid().ToString(), new GetTaskFilter(), new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetMyTask_WithNoMatchingTasks_ShouldReturnEmpty()
    {
        var userId = Guid.NewGuid();
        _mockUser
            .Setup(x => x.GetUsersByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, UserInfo>
            {
                [userId] = new(userId, "my_user", "me@test.com", "A", "B")
            });

        var handler = new GetMyTaskQueryHandler(Session, Mapper, _mockUser.Object, _mockMgmt.Object);
        var result = await handler.Handle(
            new GetMyTaskQuery(userId.ToString(), new GetTaskFilter(), new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetMyTask_WithMatchingTasks_ShouldReturnItems()
    {
        var userId = Guid.NewGuid();
        SeedTask(assignedToUserName: "my_user", name: "Task A");
        SeedTask(assignedToUserName: "my_user", name: "Task B");
        SeedTask(assignedToUserName: "other_user", name: "Not Mine");
        await Session.SaveChangesAsync();

        _mockUser
            .Setup(x => x.GetUsersByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, UserInfo>
            {
                [userId] = new(userId, "my_user", "me@test.com", "A", "B")
            });

        var handler = new GetMyTaskQueryHandler(Session, Mapper, _mockUser.Object, _mockMgmt.Object);
        var result = await handler.Handle(
            new GetMyTaskQuery(userId.ToString(), new GetTaskFilter(), new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetMyTask_WithStatusFilter_ShouldReturnFiltered()
    {
        var userId = Guid.NewGuid();
        SeedTask(assignedToUserName: "my_user", status: TaskDefineStatus.ToDo, name: "T1");
        SeedTask(assignedToUserName: "my_user", status: TaskDefineStatus.InProgress, name: "T2");
        await Session.SaveChangesAsync();

        _mockUser
            .Setup(x => x.GetUsersByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, UserInfo>
            {
                [userId] = new(userId, "my_user", null, null, null)
            });

        var handler = new GetMyTaskQueryHandler(Session, Mapper, _mockUser.Object, _mockMgmt.Object);
        var result = await handler.Handle(
            new GetMyTaskQuery(userId.ToString(), new GetTaskFilter { Status = TaskDefineStatus.ToDo }, new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetMyTask_WithDateFilters_ShouldReturnFiltered()
    {
        var userId = Guid.NewGuid();
        var past = DateTimeOffset.UtcNow.AddDays(-10);
        var future = DateTimeOffset.UtcNow.AddDays(10);

        var taskEarly = TaskEntity.Create(Guid.NewGuid(), Guid.NewGuid(), "Early Task",
            assignedToUserName: "my_user", startDate: past.AddDays(-5));
        var taskRecent = TaskEntity.Create(Guid.NewGuid(), Guid.NewGuid(), "Recent Task",
            assignedToUserName: "my_user", startDate: past.AddDays(2));
        Session.Store(taskEarly);
        Session.Store(taskRecent);
        await Session.SaveChangesAsync();

        _mockUser
            .Setup(x => x.GetUsersByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, UserInfo>
            {
                [userId] = new(userId, "my_user", null, null, null)
            });

        var handler = new GetMyTaskQueryHandler(Session, Mapper, _mockUser.Object, _mockMgmt.Object);
        var result = await handler.Handle(
            new GetMyTaskQuery(
                userId.ToString(),
                new GetTaskFilter { DateField = DateTaskFilterField.StartDate, FromDate = past },
                new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].Name.Should().Be("Recent Task");
    }

    [Fact]
    public async Task GetMyTask_WithPaperIdFilter_ShouldReturnFiltered()
    {
        var userId = Guid.NewGuid();
        var paperId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var paper = PaperEntity.Create(paperId, "My Paper");
        Session.Store(paper);

        var task = SeedTask(memberId: memberId, assignedToUserName: "my_user");
        var contributor = SeedContributor(paperId, memberId, Guid.NewGuid());
        contributor.AddTasks(task.Id);
        Session.Store(contributor);

        var otherMemberId = Guid.NewGuid();
        var otherTask = SeedTask(memberId: otherMemberId, assignedToUserName: "my_user");
        await Session.SaveChangesAsync();

        _mockUser
            .Setup(x => x.GetUsersByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, UserInfo>
            {
                [userId] = new(userId, "my_user", null, null, null)
            });

        var handler = new GetMyTaskQueryHandler(Session, Mapper, _mockUser.Object, _mockMgmt.Object);
        var result = await handler.Handle(
            new GetMyTaskQuery(
                userId.ToString(),
                new GetTaskFilter { PaperId = paperId },
                new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
    }

    // ─── GetTasksByPaperId ────────────────────────────────────────────────────

    [Fact]
    public async Task GetTasksByPaperId_WhenNotMember_ShouldThrowNoPermissionException()
    {
        var paperId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _mockMgmt
            .Setup(x => x.GetMemberByPaperIdAsync(paperId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ValueTuple<Guid, Guid, Guid>?)null);

        var handler = new GetTasksByPaperIdQueryHandler(Session, Mapper, _mockMgmt.Object, _mockUser.Object);
        var act = () => handler.Handle(
            new GetTasksByPaperIdQuery(paperId, userId.ToString(), new GetTaskByPaperIdFilter(), new PaginationRequest(1, 10)),
            CancellationToken.None);
        await act.Should().ThrowAsync<NoPermissionException>();
    }

    [Fact]
    public async Task GetTasksByPaperId_WhenNoMembers_ShouldReturnEmpty()
    {
        var paperId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _mockMgmt
            .Setup(x => x.GetMemberByPaperIdAsync(paperId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()));
        _mockMgmt
            .Setup(x => x.GetSubProjectMembersByPaperIdAsync(paperId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SubProjectMemberInfo>());

        var handler = new GetTasksByPaperIdQueryHandler(Session, Mapper, _mockMgmt.Object, _mockUser.Object);
        var result = await handler.Handle(
            new GetTasksByPaperIdQuery(paperId, userId.ToString(), new GetTaskByPaperIdFilter(), new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTasksByPaperId_WithTasks_ShouldReturnPagedResults()
    {
        var paperId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var memberUserId = Guid.NewGuid();

        var paper = PaperEntity.Create(paperId, "Paper For Tasks");
        Session.Store(paper);
        var task1 = SeedTask(memberId: memberId, name: "T1");
        var task2 = SeedTask(memberId: memberId, name: "T2");
        await Session.SaveChangesAsync();

        _mockMgmt
            .Setup(x => x.GetMemberByPaperIdAsync(paperId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid.NewGuid(), memberId, Guid.NewGuid()));
        _mockMgmt
            .Setup(x => x.GetSubProjectMembersByPaperIdAsync(paperId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SubProjectMemberInfo>
            {
                new(memberId, memberUserId, AuthorizeConstants.PaperAuthor)
            });

        var handler = new GetTasksByPaperIdQueryHandler(Session, Mapper, _mockMgmt.Object, _mockUser.Object);
        var result = await handler.Handle(
            new GetTasksByPaperIdQuery(paperId, userId.ToString(), new GetTaskByPaperIdFilter(), new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetTasksByPaperId_WithStatusFilter_ShouldReturnFiltered()
    {
        var paperId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        SeedTask(memberId: memberId, status: TaskDefineStatus.ToDo, name: "Todo");
        SeedTask(memberId: memberId, status: TaskDefineStatus.InProgress, name: "InProg");
        await Session.SaveChangesAsync();

        _mockMgmt
            .Setup(x => x.GetMemberByPaperIdAsync(paperId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid.NewGuid(), memberId, Guid.NewGuid()));
        _mockMgmt
            .Setup(x => x.GetSubProjectMembersByPaperIdAsync(paperId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SubProjectMemberInfo>
            {
                new(memberId, Guid.NewGuid(), AuthorizeConstants.PaperAuthor)
            });

        var handler = new GetTasksByPaperIdQueryHandler(Session, Mapper, _mockMgmt.Object, _mockUser.Object);
        var result = await handler.Handle(
            new GetTasksByPaperIdQuery(
                paperId, userId.ToString(),
                new GetTaskByPaperIdFilter { Status = TaskDefineStatus.ToDo },
                new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].Name.Should().Be("Todo");
    }

    [Fact]
    public async Task GetTasksByPaperId_WithAssignedToUserNameFilter_ShouldReturnFiltered()
    {
        var paperId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        SeedTask(memberId: memberId, assignedToUserName: "alice", name: "Alice Task");
        SeedTask(memberId: memberId, assignedToUserName: "bob", name: "Bob Task");
        await Session.SaveChangesAsync();

        _mockMgmt
            .Setup(x => x.GetMemberByPaperIdAsync(paperId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid.NewGuid(), memberId, Guid.NewGuid()));
        _mockMgmt
            .Setup(x => x.GetSubProjectMembersByPaperIdAsync(paperId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SubProjectMemberInfo>
            {
                new(memberId, Guid.NewGuid(), AuthorizeConstants.PaperAuthor)
            });

        var handler = new GetTasksByPaperIdQueryHandler(Session, Mapper, _mockMgmt.Object, _mockUser.Object);
        var result = await handler.Handle(
            new GetTasksByPaperIdQuery(
                paperId, userId.ToString(),
                new GetTaskByPaperIdFilter { AssignedToUserName = "alice" },
                new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].Name.Should().Be("Alice Task");
    }

    // ─── CreateTask with SectionId ────────────────────────────────────────────

    private void SetupAuthMocks(Guid paperId, Guid userId, Guid subProjectId, Guid memberId, Guid assignedMemberId, Guid assignedUserId)
    {
        _mockMgmt
            .Setup(x => x.GetMemberByPaperIdAsync(paperId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((subProjectId, memberId, Guid.NewGuid()));
        _mockMgmt
            .Setup(x => x.GetMyProjectRoleAsync(subProjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthorizeConstants.PaperAuthor);
        _mockMgmt
            .Setup(x => x.GetMemberByIdAsync(assignedMemberId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ManagementMemberInfo(assignedMemberId, assignedUserId, subProjectId, AuthorizeConstants.PaperMember));
        _mockUser
            .Setup(x => x.GetUsersByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, UserInfo>
            {
                [assignedUserId] = new(assignedUserId, "assignee_user", "a@b.com", "First", "Last")
            });
    }

    [Fact]
    public async Task CreateTask_WithSectionId_SectionNotInDb_ShouldThrowNotFoundException()
    {
        var paper = SeedPaper();
        await Session.SaveChangesAsync();

        var subProjectId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var assignedMemberId = Guid.NewGuid();
        var assignedUserId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        SetupAuthMocks(paper.Id, userId, subProjectId, memberId, assignedMemberId, assignedUserId);

        var handler = new CreateTaskHandler(Session, _mockMgmt.Object, _mockUser.Object);
        var dto = new CreateTaskDto
        {
            PaperId = paper.Id,
            Name = "Task with missing section",
            MemberId = assignedMemberId,
            SectionId = Guid.NewGuid() // section not in DB
        };

        var act = () => handler.Handle(new CreateTaskCommand(dto, userId.ToString(), "author"), CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateTask_WithSectionId_SectionBelongsToDifferentPaper_ShouldThrowNotFoundException()
    {
        var paper = SeedPaper();
        var otherPaperId = Guid.NewGuid();
        var section = SectionEntity.Create(
            id: Guid.NewGuid(),
            content: "",
            title: "Intro",
            description: "",
            status: SectionStatus.NotStarted,
            mainIdea: "",
            rule: "",
            displayOrder: 1,
            isMainSection: true,
            version: "V1",
            paperId: otherPaperId, // belongs to different paper
            createdBy: "test");
        Session.Store(section);
        await Session.SaveChangesAsync();

        var subProjectId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var assignedMemberId = Guid.NewGuid();
        var assignedUserId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        SetupAuthMocks(paper.Id, userId, subProjectId, memberId, assignedMemberId, assignedUserId);

        var handler = new CreateTaskHandler(Session, _mockMgmt.Object, _mockUser.Object);
        var dto = new CreateTaskDto
        {
            PaperId = paper.Id,
            Name = "Task with wrong paper section",
            MemberId = assignedMemberId,
            SectionId = section.Id
        };

        var act = () => handler.Handle(new CreateTaskCommand(dto, userId.ToString(), "author"), CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateTask_WithSectionId_ExistingContributor_ShouldAddTaskToContributor()
    {
        var paper = SeedPaper();
        var subProjectId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var assignedMemberId = Guid.NewGuid();
        var assignedUserId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var section = SectionEntity.Create(
            id: Guid.NewGuid(),
            content: "",
            title: "Methods",
            description: "",
            status: SectionStatus.NotStarted,
            mainIdea: "",
            rule: "",
            displayOrder: 1,
            isMainSection: true,
            version: "V1",
            paperId: paper.Id,
            createdBy: "test");
        Session.Store(section);

        // Pre-seed an existing contributor for this member+section
        var existingContributor = PaperContributorEntity.Create(
            id: Guid.NewGuid(),
            sectionRole: AuthorizeConstants.SectionEdit,
            paperId: paper.Id,
            sectionId: section.Id,
            memberId: assignedMemberId,
            markSectionId: section.Id);
        Session.Store(existingContributor);
        await Session.SaveChangesAsync();

        SetupAuthMocks(paper.Id, userId, subProjectId, memberId, assignedMemberId, assignedUserId);

        var handler = new CreateTaskHandler(Session, _mockMgmt.Object, _mockUser.Object);
        var dto = new CreateTaskDto
        {
            PaperId = paper.Id,
            Name = "Assigned Task",
            MemberId = assignedMemberId,
            SectionId = section.Id
        };

        var taskId = await handler.Handle(new CreateTaskCommand(dto, userId.ToString(), "author"), CancellationToken.None);

        taskId.Should().NotBeEmpty();
        var updatedContributor = await Session.LoadAsync<PaperContributorEntity>(existingContributor.Id);
        updatedContributor!.TaskIds.Should().Contain(taskId);
    }

    [Fact]
    public async Task CreateTask_WithSectionId_NoExistingContributor_ShouldCreateContributorAndLinkTask()
    {
        var paper = SeedPaper();
        var subProjectId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var assignedMemberId = Guid.NewGuid();
        var assignedUserId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var section = SectionEntity.Create(
            id: Guid.NewGuid(),
            content: "",
            title: "Discussion",
            description: "",
            status: SectionStatus.NotStarted,
            mainIdea: "",
            rule: "",
            displayOrder: 1,
            isMainSection: true,
            version: "V1",
            paperId: paper.Id,
            createdBy: "test");
        Session.Store(section);
        await Session.SaveChangesAsync();

        SetupAuthMocks(paper.Id, userId, subProjectId, memberId, assignedMemberId, assignedUserId);

        var handler = new CreateTaskHandler(Session, _mockMgmt.Object, _mockUser.Object);
        var dto = new CreateTaskDto
        {
            PaperId = paper.Id,
            Name = "New Sectioned Task",
            MemberId = assignedMemberId,
            SectionId = section.Id
        };

        var taskId = await handler.Handle(new CreateTaskCommand(dto, userId.ToString(), "author"), CancellationToken.None);

        taskId.Should().NotBeEmpty();

        var contributors = await Session.Query<PaperContributorEntity>()
            .Where(x => x.PaperId == paper.Id && x.MemberId == assignedMemberId)
            .ToListAsync();

        contributors.Should().NotBeEmpty();
        contributors.Should().Contain(c => c.TaskIds.Contains(taskId));
    }
}
