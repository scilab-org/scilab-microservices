namespace Management.Application.Tests.Common.TestData;

public static class ProjectTestData
{
    public static CreateProjectDto CreateProjectDto(
        string? name = "Test Project",
        string? code = "TP-001",
        string? description = "A test project",
        ProjectStatus? status = ProjectStatus.Draft,
        DateTimeOffset? startDate = null,
        DateTimeOffset? endDate = null,
        string? context = null,
        List<Guid>? domainIds = null,
        string? keypoint = null)
    {
        return new CreateProjectDto
        {
            Name = name,
            Code = code,
            Description = description,
            Status = status,
            StartDate = startDate,
            EndDate = endDate,
            Context = context,
            DomainIds = domainIds ?? [],
            Keypoint = keypoint
        };
    }

    public static UpdateProjectDto UpdateProjectDto(
        string name = "Updated Project",
        string? code = "UP-001",
        string? description = "An updated project",
        ProjectStatus? status = ProjectStatus.Active,
        DateTimeOffset? startDate = null,
        DateTimeOffset? endDate = null,
        string? context = null,
        List<Guid>? domainIds = null,
        string? keypoint = null)
    {
        return new UpdateProjectDto
        {
            Name = name,
            Code = code,
            Description = description,
            Status = status,
            StartDate = startDate,
            EndDate = endDate,
            Context = context,
            DomainIds = domainIds ?? [],
            Keypoint = keypoint
        };
    }

    public static ProjectEntity CreateProjectEntity(
        Guid? id = null,
        string? name = "Test Project",
        string? code = "TP-001",
        Guid? parentProjectId = null,
        List<Guid>? paperIds = null,
        List<Guid>? conferenceJournalIds = null,
        string? context = null,
        List<Guid>? domainIds = null,
        string? keypoint = null,
        string? createdBy = null)
    {
        return ProjectEntity.Create(
            id: id ?? Guid.NewGuid(),
            name: name,
            code: code,
            parentProjectId: parentProjectId,
            paperIds: paperIds,
            conferenceJournalIds: conferenceJournalIds,
            context: context,
            domainIds: domainIds,
            keypoint: keypoint,
            createdBy: createdBy);
    }

    public static Actor SystemActor() => Actor.System("test-system");
}
