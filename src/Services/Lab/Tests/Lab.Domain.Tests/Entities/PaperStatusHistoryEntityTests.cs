namespace Lab.Domain.Tests.Entities;

public sealed class PaperStatusHistoryEntityTests
{
    [Fact]
    public void Create_ShouldInitializeCorrectly()
    {
        var paperId = Guid.NewGuid();
        var actorId = Guid.NewGuid();
        var pdfId = Guid.NewGuid();

        var entity = PaperStatusHistoryEntity.Create(paperId, SubmissionStatus.Submitted,
            actorId, "admin", "Submitted for review", "major", pdfId);

        entity.Id.Should().NotBe(Guid.Empty);
        entity.PaperId.Should().Be(paperId);
        entity.Status.Should().Be(SubmissionStatus.Submitted);
        entity.ActorId.Should().Be(actorId);
        entity.ActorUserName.Should().Be("admin");
        entity.Note.Should().Be("Submitted for review");
        entity.RevisionType.Should().Be("major");
        entity.PdfFileId.Should().Be(pdfId);
        entity.CreatedBy.Should().Be("admin");
        entity.CreatedOnUtc.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Create_ShouldUseDefaults()
    {
        var entity = PaperStatusHistoryEntity.Create(Guid.NewGuid(), SubmissionStatus.Draft,
            Guid.NewGuid(), "user1");

        entity.Note.Should().BeNull();
        entity.RevisionType.Should().BeNull();
        entity.PdfFileId.Should().BeNull();
    }

    [Fact]
    public void Create_ShouldGenerateUniqueIds()
    {
        var e1 = PaperStatusHistoryEntity.Create(Guid.NewGuid(), SubmissionStatus.Draft, Guid.NewGuid(), "u");
        var e2 = PaperStatusHistoryEntity.Create(Guid.NewGuid(), SubmissionStatus.Draft, Guid.NewGuid(), "u");
        e1.Id.Should().NotBe(e2.Id);
    }
}
