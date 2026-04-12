namespace User.Domain.Tests.Entities;

public sealed class OutboxMessageEntityTests
{
    private static readonly DateTimeOffset Now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_ShouldInitializeEntityWithCorrectValues()
    {
        // Arrange
        var id = Guid.NewGuid();
        const string eventType = "UserCreated";
        const string content = "{\"userId\":\"abc\"}";

        // Act
        var entity = OutboxMessageEntity.Create(id, eventType, content, Now);

        // Assert
        entity.Id.Should().Be(id);
        entity.EventType.Should().Be(eventType);
        entity.Content.Should().Be(content);
        entity.OccurredOnUtc.Should().Be(Now);
        entity.AttemptCount.Should().Be(0);
        entity.MaxAttempts.Should().Be(AppConstants.MaxAttempts);
        entity.ProcessedOnUtc.Should().BeNull();
        entity.ClaimedOnUtc.Should().BeNull();
        entity.NextAttemptOnUtc.Should().BeNull();
        entity.LastErrorMessage.Should().BeNull();
    }

    [Fact]
    public void CompleteProcessing_ShouldSetProcessedOnUtcAndClearClaimFields()
    {
        // Arrange
        var entity = OutboxMessageEntity.Create(Guid.NewGuid(), "UserCreated", "{}", Now);
        entity.Claim(Now);

        // Act
        entity.CompleteProcessing(Now.AddMinutes(1));

        // Assert
        entity.ProcessedOnUtc.Should().Be(Now.AddMinutes(1));
        entity.ClaimedOnUtc.Should().BeNull();
        entity.NextAttemptOnUtc.Should().BeNull();
        entity.LastErrorMessage.Should().BeNull();
    }

    [Fact]
    public void CompleteProcessing_ShouldSetLastErrorMessage_WhenProvided()
    {
        // Arrange
        var entity = OutboxMessageEntity.Create(Guid.NewGuid(), "UserCreated", "{}", Now);

        // Act
        entity.CompleteProcessing(Now.AddMinutes(1), "partial error");

        // Assert
        entity.LastErrorMessage.Should().Be("partial error");
    }

    [Fact]
    public void Claim_ShouldSetClaimedOnUtc()
    {
        // Arrange
        var entity = OutboxMessageEntity.Create(Guid.NewGuid(), "UserCreated", "{}", Now);

        // Act
        entity.Claim(Now);

        // Assert
        entity.ClaimedOnUtc.Should().Be(Now);
    }

    [Fact]
    public void SetRetryProperties_ShouldUpdateAllRetryFields()
    {
        // Arrange
        var entity = OutboxMessageEntity.Create(Guid.NewGuid(), "UserCreated", "{}", Now);
        var nextAttempt = Now.AddMinutes(10);

        // Act
        entity.SetRetryProperties(2, 5, nextAttempt, "transient error");

        // Assert
        entity.AttemptCount.Should().Be(2);
        entity.MaxAttempts.Should().Be(5);
        entity.NextAttemptOnUtc.Should().Be(nextAttempt);
        entity.LastErrorMessage.Should().Be("transient error");
    }

    [Fact]
    public void IncreaseAttemptCount_ShouldIncrementByOne()
    {
        // Arrange
        var entity = OutboxMessageEntity.Create(Guid.NewGuid(), "UserCreated", "{}", Now);

        // Act
        entity.IncreaseAttemptCount();
        entity.IncreaseAttemptCount();

        // Assert
        entity.AttemptCount.Should().Be(2);
    }

    [Fact]
    public void RecordFailedAttempt_ShouldIncrementAttemptCountAndSetNextAttempt_WhenBelowMaxAttempts()
    {
        // Arrange
        var entity = OutboxMessageEntity.Create(Guid.NewGuid(), "UserCreated", "{}", Now);

        // Act
        entity.RecordFailedAttempt("network error", Now);

        // Assert
        entity.AttemptCount.Should().Be(1);
        entity.LastErrorMessage.Should().Be("network error");
        entity.NextAttemptOnUtc.Should().NotBeNull();
        entity.NextAttemptOnUtc!.Value.Should().BeAfter(Now);
    }

    [Fact]
    public void RecordFailedAttempt_ShouldSetMaxAttemptsExceededMessage_WhenAtMaxAttempts()
    {
        // Arrange
        var entity = OutboxMessageEntity.Create(Guid.NewGuid(), "UserCreated", "{}", Now);
        // Exhaust all but last attempt
        for (var i = 0; i < AppConstants.MaxAttempts - 1; i++)
        {
            entity.RecordFailedAttempt("transient", Now);
        }

        // Act — this is the final attempt
        entity.RecordFailedAttempt("final error", Now);

        // Assert
        entity.AttemptCount.Should().Be(AppConstants.MaxAttempts);
        entity.NextAttemptOnUtc.Should().BeNull();
        entity.LastErrorMessage.Should().Contain("Max attempts");
        entity.LastErrorMessage.Should().Contain("final error");
    }

    [Fact]
    public void CanRetry_ShouldReturnTrue_WhenBelowMaxAttemptsAndNoNextAttemptTime()
    {
        // Arrange
        var entity = OutboxMessageEntity.Create(Guid.NewGuid(), "UserCreated", "{}", Now);

        // Act & Assert
        entity.CanRetry(Now).Should().BeTrue();
    }

    [Fact]
    public void CanRetry_ShouldReturnFalse_WhenAtMaxAttempts()
    {
        // Arrange
        var entity = OutboxMessageEntity.Create(Guid.NewGuid(), "UserCreated", "{}", Now);
        entity.SetRetryProperties(AppConstants.MaxAttempts, AppConstants.MaxAttempts, null, "error");

        // Act & Assert
        entity.CanRetry(Now).Should().BeFalse();
    }

    [Fact]
    public void CanRetry_ShouldReturnFalse_WhenNextAttemptIsInFuture()
    {
        // Arrange
        var entity = OutboxMessageEntity.Create(Guid.NewGuid(), "UserCreated", "{}", Now);
        entity.SetRetryProperties(1, AppConstants.MaxAttempts, Now.AddMinutes(30), "error");

        // Act & Assert
        entity.CanRetry(Now).Should().BeFalse();
    }

    [Fact]
    public void CanRetry_ShouldReturnTrue_WhenNextAttemptTimeHasPassed()
    {
        // Arrange
        var entity = OutboxMessageEntity.Create(Guid.NewGuid(), "UserCreated", "{}", Now);
        entity.SetRetryProperties(1, AppConstants.MaxAttempts, Now.AddMinutes(-1), "error");

        // Act & Assert
        entity.CanRetry(Now).Should().BeTrue();
    }

    [Fact]
    public void IsPermanentlyFailed_ShouldReturnFalse_WhenBelowMaxAttempts()
    {
        // Arrange
        var entity = OutboxMessageEntity.Create(Guid.NewGuid(), "UserCreated", "{}", Now);

        // Act & Assert
        entity.IsPermanentlyFailed().Should().BeFalse();
    }

    [Fact]
    public void IsPermanentlyFailed_ShouldReturnTrue_WhenAtMaxAttempts()
    {
        // Arrange
        var entity = OutboxMessageEntity.Create(Guid.NewGuid(), "UserCreated", "{}", Now);
        entity.SetRetryProperties(AppConstants.MaxAttempts, AppConstants.MaxAttempts, null, "fatal");

        // Act & Assert
        entity.IsPermanentlyFailed().Should().BeTrue();
    }
}
