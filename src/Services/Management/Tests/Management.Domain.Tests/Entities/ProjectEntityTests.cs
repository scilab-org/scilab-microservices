using Management.Domain.Enums;

namespace Management.Domain.Tests.Entities;

public sealed class ProjectEntityTests
{
    [Fact]
    public void Create_ShouldInitializeEntityWithCorrectValues()
    {
        // Arrange
        var id = Guid.NewGuid();
        const string name = "Test Project";
        const string description = "A test project";
        const string code = "TP-001";
        var status = ProjectStatus.Active;
        var startDate = DateTimeOffset.UtcNow;
        var endDate = DateTimeOffset.UtcNow.AddMonths(6);
        var parentProjectId = Guid.NewGuid();
        const string context = "AI Research";
        const string keypoint = "Accuracy improvement";
        var domainIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        const string createdBy = "admin";

        // Act
        var entity = ProjectEntity.Create(
            id: id,
            name: name,
            description: description,
            code: code,
            status: status,
            startDate: startDate,
            endDate: endDate,
            parentProjectId: parentProjectId,
            context: context,
            domainIds: domainIds,
            keypoint: keypoint,
            createdBy: createdBy);

        // Assert
        entity.Id.Should().Be(id);
        entity.Name.Should().Be(name);
        entity.Description.Should().Be(description);
        entity.Code.Should().Be(code);
        entity.Status.Should().Be(status);
        entity.StartDate.Should().Be(startDate);
        entity.EndDate.Should().Be(endDate);
        entity.ParentProjectId.Should().Be(parentProjectId);
        entity.Context.Should().Be(context);
        entity.DomainIds.Should().BeEquivalentTo(domainIds);
        entity.Keypoint.Should().Be(keypoint);
        entity.CreatedBy.Should().Be(createdBy);
        entity.CreatedOnUtc.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
        entity.LastModifiedOnUtc.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
        entity.PaperIds.Should().BeEmpty();
        entity.DatasetIds.Should().BeEmpty();
        entity.ConferenceJournalIds.Should().BeEmpty();
    }

    [Fact]
    public void Create_ShouldInitializeWithDefaultValues_WhenOptionalParametersOmitted()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var entity = ProjectEntity.Create(id: id);

        // Assert
        entity.Id.Should().Be(id);
        entity.Name.Should().BeNull();
        entity.Description.Should().BeNull();
        entity.Code.Should().BeNull();
        entity.Status.Should().BeNull();
        entity.StartDate.Should().BeNull();
        entity.EndDate.Should().BeNull();
        entity.ParentProjectId.Should().BeNull();
        entity.PaperIds.Should().BeEmpty();
        entity.DatasetIds.Should().BeEmpty();
        entity.ConferenceJournalIds.Should().BeEmpty();
    }

    [Fact]
    public void Create_ShouldDeduplicatePaperIds()
    {
        // Arrange
        var id = Guid.NewGuid();
        var paperId = Guid.NewGuid();
        var paperIds = new List<Guid> { paperId, paperId, paperId };

        // Act
        var entity = ProjectEntity.Create(id: id, paperIds: paperIds);

        // Assert
        entity.PaperIds.Should().ContainSingle().Which.Should().Be(paperId);
    }

    [Fact]
    public void Create_ShouldDeduplicateConferenceJournalIds()
    {
        // Arrange
        var id = Guid.NewGuid();
        var journalId = Guid.NewGuid();
        var journalIds = new List<Guid> { journalId, journalId };

        // Act
        var entity = ProjectEntity.Create(id: id, conferenceJournalIds: journalIds);

        // Assert
        entity.ConferenceJournalIds.Should().ContainSingle().Which.Should().Be(journalId);
    }

    [Fact]
    public void Update_ShouldUpdateProvidedValues()
    {
        // Arrange
        var entity = ProjectEntity.Create(id: Guid.NewGuid(), name: "Old Name", code: "OLD");
        var originalModified = entity.LastModifiedOnUtc;

        // Act
        entity.Update(
            name: "New Name",
            description: "New Desc",
            code: "NEW",
            context: "New Context",
            domainIds: new List<Guid> { Guid.NewGuid() },
            keypoint: "New Keypoint",
            status: ProjectStatus.Completed,
            startDate: DateTimeOffset.UtcNow,
            endDate: DateTimeOffset.UtcNow.AddDays(30));

        // Assert
        entity.Name.Should().Be("New Name");
        entity.Description.Should().Be("New Desc");
        entity.Code.Should().Be("NEW");
        entity.Context.Should().Be("New Context");
        entity.Keypoint.Should().Be("New Keypoint");
        entity.Status.Should().Be(ProjectStatus.Completed);
        entity.LastModifiedOnUtc.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Update_ShouldRetainExistingValues_WhenParametersAreNull()
    {
        // Arrange
        var entity = ProjectEntity.Create(
            id: Guid.NewGuid(),
            name: "Existing Name",
            code: "EX",
            description: "Existing Desc",
            status: ProjectStatus.Active);

        // Act
        entity.Update();

        // Assert
        entity.Name.Should().Be("Existing Name");
        entity.Code.Should().Be("EX");
        entity.Description.Should().Be("Existing Desc");
        entity.Status.Should().Be(ProjectStatus.Active);
    }

    [Fact]
    public void Update_ShouldUpdateConferenceJournalIds_WhenProvided()
    {
        // Arrange
        var entity = ProjectEntity.Create(id: Guid.NewGuid());
        var newJournalIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        // Act
        entity.Update(conferenceJournalIds: newJournalIds);

        // Assert
        entity.ConferenceJournalIds.Should().BeEquivalentTo(newJournalIds);
    }

    [Fact]
    public void AddPapers_ShouldAddNewPaperIds()
    {
        // Arrange
        var entity = ProjectEntity.Create(id: Guid.NewGuid());
        var paperId1 = Guid.NewGuid();
        var paperId2 = Guid.NewGuid();

        // Act
        entity.AddPapers(new[] { paperId1, paperId2 });

        // Assert
        entity.PaperIds.Should().HaveCount(2);
        entity.PaperIds.Should().Contain(paperId1);
        entity.PaperIds.Should().Contain(paperId2);
    }

    [Fact]
    public void AddPapers_ShouldNotAddDuplicates()
    {
        // Arrange
        var paperId = Guid.NewGuid();
        var entity = ProjectEntity.Create(id: Guid.NewGuid(), paperIds: new List<Guid> { paperId });

        // Act
        entity.AddPapers(new[] { paperId });

        // Assert
        entity.PaperIds.Should().ContainSingle().Which.Should().Be(paperId);
    }

    [Fact]
    public void AddPapers_ShouldDeduplicateInput()
    {
        // Arrange
        var entity = ProjectEntity.Create(id: Guid.NewGuid());
        var paperId = Guid.NewGuid();

        // Act
        entity.AddPapers(new[] { paperId, paperId, paperId });

        // Assert
        entity.PaperIds.Should().ContainSingle().Which.Should().Be(paperId);
    }

    [Fact]
    public void AddPapers_ShouldUpdateLastModifiedOnUtc()
    {
        // Arrange
        var entity = ProjectEntity.Create(id: Guid.NewGuid());

        // Act
        entity.AddPapers(new[] { Guid.NewGuid() });

        // Assert
        entity.LastModifiedOnUtc.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void RemovePapers_ShouldRemoveExistingPaperIds()
    {
        // Arrange
        var paperId1 = Guid.NewGuid();
        var paperId2 = Guid.NewGuid();
        var entity = ProjectEntity.Create(id: Guid.NewGuid(), paperIds: new List<Guid> { paperId1, paperId2 });

        // Act
        var removed = entity.RemovePapers(new[] { paperId1 });

        // Assert
        removed.Should().ContainSingle().Which.Should().Be(paperId1);
        entity.PaperIds.Should().ContainSingle().Which.Should().Be(paperId2);
    }

    [Fact]
    public void RemovePapers_ShouldReturnEmptyList_WhenPaperIdsNotFound()
    {
        // Arrange
        var entity = ProjectEntity.Create(id: Guid.NewGuid());

        // Act
        var removed = entity.RemovePapers(new[] { Guid.NewGuid() });

        // Assert
        removed.Should().BeEmpty();
    }

    [Fact]
    public void RemovePapers_ShouldNotUpdateLastModifiedOnUtc_WhenNoPapersRemoved()
    {
        // Arrange
        var entity = ProjectEntity.Create(id: Guid.NewGuid());
        var originalModified = entity.LastModifiedOnUtc;

        // Act
        entity.RemovePapers(new[] { Guid.NewGuid() });

        // Assert
        entity.LastModifiedOnUtc.Should().Be(originalModified);
    }

    [Fact]
    public void RemovePapers_ShouldUpdateLastModifiedOnUtc_WhenPapersRemoved()
    {
        // Arrange
        var paperId = Guid.NewGuid();
        var entity = ProjectEntity.Create(id: Guid.NewGuid(), paperIds: new List<Guid> { paperId });

        // Act
        entity.RemovePapers(new[] { paperId });

        // Assert
        entity.LastModifiedOnUtc.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void RemovePapers_ShouldDeduplicateInput()
    {
        // Arrange
        var paperId = Guid.NewGuid();
        var entity = ProjectEntity.Create(id: Guid.NewGuid(), paperIds: new List<Guid> { paperId });

        // Act
        var removed = entity.RemovePapers(new[] { paperId, paperId });

        // Assert
        removed.Should().ContainSingle().Which.Should().Be(paperId);
    }

    [Fact]
    public void AddConferenceJournals_ShouldAddNewJournalIds()
    {
        // Arrange
        var entity = ProjectEntity.Create(id: Guid.NewGuid());
        var journalId1 = Guid.NewGuid();
        var journalId2 = Guid.NewGuid();

        // Act
        entity.AddConferenceJournals(new[] { journalId1, journalId2 });

        // Assert
        entity.ConferenceJournalIds.Should().HaveCount(2);
        entity.ConferenceJournalIds.Should().Contain(journalId1);
        entity.ConferenceJournalIds.Should().Contain(journalId2);
    }

    [Fact]
    public void AddConferenceJournals_ShouldNotAddDuplicates()
    {
        // Arrange
        var journalId = Guid.NewGuid();
        var entity = ProjectEntity.Create(id: Guid.NewGuid(), conferenceJournalIds: new List<Guid> { journalId });

        // Act
        entity.AddConferenceJournals(new[] { journalId });

        // Assert
        entity.ConferenceJournalIds.Should().ContainSingle().Which.Should().Be(journalId);
    }

    [Fact]
    public void AddConferenceJournals_ShouldUpdateLastModifiedOnUtc()
    {
        // Arrange
        var entity = ProjectEntity.Create(id: Guid.NewGuid());

        // Act
        entity.AddConferenceJournals(new[] { Guid.NewGuid() });

        // Assert
        entity.LastModifiedOnUtc.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void RemoveConferenceJournals_ShouldRemoveExistingJournalIds()
    {
        // Arrange
        var journalId1 = Guid.NewGuid();
        var journalId2 = Guid.NewGuid();
        var entity = ProjectEntity.Create(id: Guid.NewGuid(), conferenceJournalIds: new List<Guid> { journalId1, journalId2 });

        // Act
        var removed = entity.RemoveConferenceJournals(new[] { journalId1 });

        // Assert
        removed.Should().ContainSingle().Which.Should().Be(journalId1);
        entity.ConferenceJournalIds.Should().ContainSingle().Which.Should().Be(journalId2);
    }

    [Fact]
    public void RemoveConferenceJournals_ShouldReturnEmptyList_WhenJournalIdsNotFound()
    {
        // Arrange
        var entity = ProjectEntity.Create(id: Guid.NewGuid());

        // Act
        var removed = entity.RemoveConferenceJournals(new[] { Guid.NewGuid() });

        // Assert
        removed.Should().BeEmpty();
    }

    [Fact]
    public void RemoveConferenceJournals_ShouldNotUpdateLastModifiedOnUtc_WhenNoJournalsRemoved()
    {
        // Arrange
        var entity = ProjectEntity.Create(id: Guid.NewGuid());
        var originalModified = entity.LastModifiedOnUtc;

        // Act
        entity.RemoveConferenceJournals(new[] { Guid.NewGuid() });

        // Assert
        entity.LastModifiedOnUtc.Should().Be(originalModified);
    }

    [Fact]
    public void RemoveConferenceJournals_ShouldUpdateLastModifiedOnUtc_WhenJournalsRemoved()
    {
        // Arrange
        var journalId = Guid.NewGuid();
        var entity = ProjectEntity.Create(id: Guid.NewGuid(), conferenceJournalIds: new List<Guid> { journalId });

        // Act
        entity.RemoveConferenceJournals(new[] { journalId });

        // Assert
        entity.LastModifiedOnUtc.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
    }
}
