using Management.Domain.Enums;

namespace Management.Domain.Tests.Entities;

public sealed class DatasetEntityTests
{
    [Fact]
    public void Create_ShouldInitializeEntityWithCorrectValues()
    {
        // Arrange
        var id = Guid.NewGuid();
        const string name = "Test Dataset";
        const string description = "A test dataset";

        // Act
        var entity = DatasetEntity.Create(id, name, description);

        // Assert
        entity.Id.Should().Be(id);
        entity.Name.Should().Be(name);
        entity.Description.Should().Be(description);
        entity.Status.Should().Be(DatasetStatus.Public);
        entity.FilePath.Should().BeNull();
        entity.CreatedOnUtc.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
        entity.LastModifiedOnUtc.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Create_ShouldAcceptNullDescription()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var entity = DatasetEntity.Create(id, "Name", null);

        // Assert
        entity.Description.Should().BeNull();
    }

    [Fact]
    public void Update_ShouldUpdateAllFields()
    {
        // Arrange
        var entity = DatasetEntity.Create(Guid.NewGuid(), "Old Name", "Old Desc");

        // Act
        entity.Update("New Name", "New Desc", DatasetStatus.Public);

        // Assert
        entity.Name.Should().Be("New Name");
        entity.Description.Should().Be("New Desc");
        entity.Status.Should().Be(DatasetStatus.Public);
        entity.LastModifiedOnUtc.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Update_ShouldDefaultToPublicStatus_WhenStatusIsNull()
    {
        // Arrange
        var entity = DatasetEntity.Create(Guid.NewGuid(), "Name", "Desc");

        // Act
        entity.Update("Name", "Desc", null);

        // Assert
        entity.Status.Should().Be(DatasetStatus.Public);
    }

    [Fact]
    public void UpdateFilePath_ShouldSetFilePath_WhenUrlIsValid()
    {
        // Arrange
        var entity = DatasetEntity.Create(Guid.NewGuid(), "Name", "Desc");
        const string url = "https://storage.example.com/datasets/file.csv";

        // Act
        entity.UpdateFilePath(url);

        // Assert
        entity.FilePath.Should().Be(url);
        entity.LastModifiedOnUtc.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void UpdateFilePath_ShouldNotUpdateFilePath_WhenUrlIsNull()
    {
        // Arrange
        var entity = DatasetEntity.Create(Guid.NewGuid(), "Name", "Desc");
        var originalModified = entity.LastModifiedOnUtc;

        // Act
        entity.UpdateFilePath(null);

        // Assert
        entity.FilePath.Should().BeNull();
        entity.LastModifiedOnUtc.Should().Be(originalModified);
    }

    [Fact]
    public void UpdateFilePath_ShouldNotUpdateFilePath_WhenUrlIsEmpty()
    {
        // Arrange
        var entity = DatasetEntity.Create(Guid.NewGuid(), "Name", "Desc");
        var originalModified = entity.LastModifiedOnUtc;

        // Act
        entity.UpdateFilePath(string.Empty);

        // Assert
        entity.FilePath.Should().BeNull();
        entity.LastModifiedOnUtc.Should().Be(originalModified);
    }

    [Fact]
    public void UpdateFilePath_ShouldNotUpdateFilePath_WhenUrlIsWhitespace()
    {
        // Arrange
        var entity = DatasetEntity.Create(Guid.NewGuid(), "Name", "Desc");
        var originalModified = entity.LastModifiedOnUtc;

        // Act
        entity.UpdateFilePath("   ");

        // Assert
        entity.FilePath.Should().BeNull();
        entity.LastModifiedOnUtc.Should().Be(originalModified);
    }
}
