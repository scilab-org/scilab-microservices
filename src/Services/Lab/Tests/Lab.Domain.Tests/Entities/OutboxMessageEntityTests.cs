namespace Lab.Domain.Tests.Entities;

public sealed class OutboxMessageEntityTests
{
    [Fact]
    public void Create_ShouldInitializeCorrectly()
    {
        var id = Guid.NewGuid();
        var occurred = DateTimeOffset.UtcNow;

        var entity = OutboxMessageEntity.Create(id, "TestEvent", "{}", occurred);

        entity.Id.Should().Be(id);
        entity.EventType.Should().Be("TestEvent");
        entity.Content.Should().Be("{}");
        entity.OccurredOnUtc.Should().Be(occurred);
        entity.MaxAttempts.Should().Be(AppConstants.MaxAttempts);
        entity.AttemptCount.Should().Be(0);
        entity.ProcessedOnUtc.Should().BeNull();
        entity.ClaimedOnUtc.Should().BeNull();
        entity.NextAttemptOnUtc.Should().BeNull();
        entity.LastErrorMessage.Should().BeNull();
    }

    [Fact]
    public void CompleteProcessing_ShouldSetFields()
    {
        var entity = OutboxMessageEntity.Create(Guid.NewGuid(), "E", "{}", DateTimeOffset.UtcNow);
        entity.Claim(DateTimeOffset.UtcNow);
        entity.NextAttemptOnUtc = DateTimeOffset.UtcNow.AddMinutes(5);
        var processed = DateTimeOffset.UtcNow;

        entity.CompleteProcessing(processed, "some error");

        entity.ProcessedOnUtc.Should().Be(processed);
        entity.LastErrorMessage.Should().Be("some error");
        entity.ClaimedOnUtc.Should().BeNull();
        entity.NextAttemptOnUtc.Should().BeNull();
    }

    [Fact]
    public void CompleteProcessing_ShouldHaveNullError_WhenNotProvided()
    {
        var entity = OutboxMessageEntity.Create(Guid.NewGuid(), "E", "{}", DateTimeOffset.UtcNow);
        entity.CompleteProcessing(DateTimeOffset.UtcNow);
        entity.LastErrorMessage.Should().BeNull();
    }

    [Fact]
    public void MarkForRetry_ShouldSetFields()
    {
        var entity = OutboxMessageEntity.Create(Guid.NewGuid(), "E", "{}", DateTimeOffset.UtcNow);
        entity.Claim(DateTimeOffset.UtcNow);
        entity.ProcessedOnUtc = DateTimeOffset.UtcNow;
        var next = DateTimeOffset.UtcNow.AddMinutes(5);

        entity.MarkForRetry("error msg", next);

        entity.LastErrorMessage.Should().Be("error msg");
        entity.NextAttemptOnUtc.Should().Be(next);
        entity.ClaimedOnUtc.Should().BeNull();
        entity.ProcessedOnUtc.Should().BeNull();
    }

    [Fact]
    public void Claim_ShouldSetClaimedOnUtc()
    {
        var entity = OutboxMessageEntity.Create(Guid.NewGuid(), "E", "{}", DateTimeOffset.UtcNow);
        var claimed = DateTimeOffset.UtcNow;

        entity.Claim(claimed);

        entity.ClaimedOnUtc.Should().Be(claimed);
    }

    [Fact]
    public void SetRetryProperties_ShouldSetAllFields()
    {
        var entity = OutboxMessageEntity.Create(Guid.NewGuid(), "E", "{}", DateTimeOffset.UtcNow);
        var next = DateTimeOffset.UtcNow.AddMinutes(10);

        entity.SetRetryProperties(2, 5, next, "retry error");

        entity.AttemptCount.Should().Be(2);
        entity.MaxAttempts.Should().Be(5);
        entity.NextAttemptOnUtc.Should().Be(next);
        entity.LastErrorMessage.Should().Be("retry error");
    }

    [Fact]
    public void IncreaseAttemptCount_ShouldIncrement()
    {
        var entity = OutboxMessageEntity.Create(Guid.NewGuid(), "E", "{}", DateTimeOffset.UtcNow);
        entity.IncreaseAttemptCount();
        entity.AttemptCount.Should().Be(1);
        entity.IncreaseAttemptCount();
        entity.AttemptCount.Should().Be(2);
    }

    [Fact]
    public void RecordFailedAttempt_ShouldSetNextAttempt_WhenBelowMaxAttempts()
    {
        var entity = OutboxMessageEntity.Create(Guid.NewGuid(), "E", "{}", DateTimeOffset.UtcNow);
        var now = DateTimeOffset.UtcNow;

        entity.RecordFailedAttempt("err", now);

        entity.AttemptCount.Should().Be(1);
        entity.NextAttemptOnUtc.Should().NotBeNull();
        entity.NextAttemptOnUtc.Should().BeAfter(now);
        entity.LastErrorMessage.Should().Be("err");
    }

    [Fact]
    public void RecordFailedAttempt_ShouldExceedMaxAttempts_WhenAtMax()
    {
        var entity = OutboxMessageEntity.Create(Guid.NewGuid(), "E", "{}", DateTimeOffset.UtcNow);
        entity.SetRetryProperties(AppConstants.MaxAttempts - 1, AppConstants.MaxAttempts, null, null);
        var now = DateTimeOffset.UtcNow;

        entity.RecordFailedAttempt("final error", now);

        entity.AttemptCount.Should().Be(AppConstants.MaxAttempts);
        entity.NextAttemptOnUtc.Should().BeNull();
        entity.LastErrorMessage.Should().Contain("Max attempts");
        entity.LastErrorMessage.Should().Contain("final error");
    }

    [Fact]
    public void CanRetry_ShouldReturnTrue_WhenBelowMaxAndNoNextAttempt()
    {
        var entity = OutboxMessageEntity.Create(Guid.NewGuid(), "E", "{}", DateTimeOffset.UtcNow);
        entity.CanRetry(DateTimeOffset.UtcNow).Should().BeTrue();
    }

    [Fact]
    public void CanRetry_ShouldReturnTrue_WhenBelowMaxAndPastNextAttempt()
    {
        var entity = OutboxMessageEntity.Create(Guid.NewGuid(), "E", "{}", DateTimeOffset.UtcNow);
        entity.NextAttemptOnUtc = DateTimeOffset.UtcNow.AddMinutes(-1);
        entity.CanRetry(DateTimeOffset.UtcNow).Should().BeTrue();
    }

    [Fact]
    public void CanRetry_ShouldReturnFalse_WhenBeforeNextAttempt()
    {
        var entity = OutboxMessageEntity.Create(Guid.NewGuid(), "E", "{}", DateTimeOffset.UtcNow);
        entity.NextAttemptOnUtc = DateTimeOffset.UtcNow.AddMinutes(10);
        entity.CanRetry(DateTimeOffset.UtcNow).Should().BeFalse();
    }

    [Fact]
    public void CanRetry_ShouldReturnFalse_WhenAtMaxAttempts()
    {
        var entity = OutboxMessageEntity.Create(Guid.NewGuid(), "E", "{}", DateTimeOffset.UtcNow);
        entity.SetRetryProperties(AppConstants.MaxAttempts, AppConstants.MaxAttempts, null, null);
        entity.CanRetry(DateTimeOffset.UtcNow).Should().BeFalse();
    }

    [Fact]
    public void IsPermanentlyFailed_ShouldReturnTrue_WhenAtMaxAttempts()
    {
        var entity = OutboxMessageEntity.Create(Guid.NewGuid(), "E", "{}", DateTimeOffset.UtcNow);
        entity.SetRetryProperties(AppConstants.MaxAttempts, AppConstants.MaxAttempts, null, null);
        entity.IsPermanentlyFailed().Should().BeTrue();
    }

    [Fact]
    public void IsPermanentlyFailed_ShouldReturnFalse_WhenBelowMaxAttempts()
    {
        var entity = OutboxMessageEntity.Create(Guid.NewGuid(), "E", "{}", DateTimeOffset.UtcNow);
        entity.IsPermanentlyFailed().Should().BeFalse();
    }

    [Fact]
    public void IsPermanentlyFailed_ShouldReturnTrue_WhenAboveMaxAttempts()
    {
        var entity = OutboxMessageEntity.Create(Guid.NewGuid(), "E", "{}", DateTimeOffset.UtcNow);
        entity.SetRetryProperties(10, AppConstants.MaxAttempts, null, null);
        entity.IsPermanentlyFailed().Should().BeTrue();
    }

    [Fact]
    public void RecordFailedAttempt_ShouldRespectMaxDelay()
    {
        // High attempt count to trigger max delay cap
        var entity = OutboxMessageEntity.Create(Guid.NewGuid(), "E", "{}", DateTimeOffset.UtcNow);
        entity.SetRetryProperties(0, 100, null, null); // high max so it doesn't exceed
        var now = DateTimeOffset.UtcNow;

        // Simulate many attempts to exceed 5 min max
        for (var i = 0; i < 20; i++)
            entity.RecordFailedAttempt("err", now);

        // Delay should be capped at 5 minutes plus jitter (max 1s)
        entity.NextAttemptOnUtc.Should().NotBeNull();
        var maxExpected = now + TimeSpan.FromMinutes(5) + TimeSpan.FromMilliseconds(1000);
        entity.NextAttemptOnUtc!.Value.Should().BeBefore(maxExpected.AddSeconds(1));
    }
}
