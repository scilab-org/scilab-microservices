using Lab.Application.Features.Paper.Commands.DeletePaper;
using Lab.Application.Features.Paper.Queries.GetPapers;
using Lab.Application.Features.Paper.Queries.GetSectionsByPaperId;
using Lab.Application.Features.Paper.Queries.GetVersionsByPaperId;
using Lab.Application.Features.Paper.Queries.GetPaperStatusHistory;
using Lab.Application.Features.Paper.Queries.GetPaperVersionFileById;
using Lab.Application.Features.Paper.Queries.GetPaperVersionFiles;
using Lab.Application.Features.Paper.Queries.GetSubmissionStatusSummary;
using Lab.Application.Features.Paper.Queries.GetCombinePaperById;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Lab.Application.Tests.Common;
using Lab.Domain.Entities;
using Lab.Domain.Enums;

namespace Lab.Application.Tests.Features.Paper;

public class PaperIntegrationTests : MartenTestBase
{
    protected override string SchemaName => "paper_tests";

    private PaperEntity SeedPaper(string title = "Test Paper")
    {
        var paper = PaperEntity.Create(Guid.NewGuid(), title);
        Session.Store(paper);
        return paper;
    }

    #region DeletePaper

    [Fact]
    public async Task DeletePaper_WithExistingPaper_ShouldRemoveFromStore()
    {
        var paper = SeedPaper();
        await Session.SaveChangesAsync();

        var handler = new DeletePaperCommandHandler(Session);
        await handler.Handle(new DeletePaperCommand(paper.Id), CancellationToken.None);

        var deleted = await Session.LoadAsync<PaperEntity>(paper.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeletePaper_WithNonExistentId_ShouldThrowClientValidationException()
    {
        var handler = new DeletePaperCommandHandler(Session);
        var act = () => handler.Handle(new DeletePaperCommand(Guid.NewGuid()), CancellationToken.None);
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    #endregion

    #region GetPapers

    [Fact]
    public async Task GetPapers_WithNoFilter_ShouldReturnAll()
    {
        SeedPaper("Paper A"); SeedPaper("Paper B");
        await Session.SaveChangesAsync();

        var handler = new GetPapersQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetPapersQuery(new GetPapersFilter(), new PaginationRequest(1, 10)), CancellationToken.None);

        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetPapers_WithTitleFilter_ShouldReturnMatching()
    {
        SeedPaper("Machine Learning"); SeedPaper("Deep Learning"); SeedPaper("Data Mining");
        await Session.SaveChangesAsync();

        var handler = new GetPapersQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetPapersQuery(new GetPapersFilter { Title = "Learning" }, new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(2);
    }

    #endregion

    #region GetVersionsByPaperId

    [Fact]
    public async Task GetVersionsByPaperId_WithExistingPaper_ShouldReturnVersions()
    {
        var paper = SeedPaper();
        var version = new PaperVersionEntity
        {
            Id = Guid.NewGuid(), PaperId = paper.Id, Name = "v1",
            Content = "content", CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow
        };
        Session.Store(version);
        await Session.SaveChangesAsync();

        var handler = new GetVersionsByPaperIdQueryHandler(Session);
        var result = await handler.Handle(new GetVersionsByPaperIdQuery(paper.Id), CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].Name.Should().Be("v1");
    }

    [Fact]
    public async Task GetVersionsByPaperId_WithNonExistentPaper_ShouldThrowNotFoundException()
    {
        var handler = new GetVersionsByPaperIdQueryHandler(Session);
        var act = () => handler.Handle(new GetVersionsByPaperIdQuery(Guid.NewGuid()), CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    #region GetPaperStatusHistory

    [Fact]
    public async Task GetPaperStatusHistory_WithHistory_ShouldReturnOrderedResults()
    {
        var paper = SeedPaper();
        Session.Store(new PaperStatusHistoryEntity
        {
            Id = Guid.NewGuid(), PaperId = paper.Id, Status = SubmissionStatus.Draft,
            CreatedOnUtc = DateTimeOffset.UtcNow.AddDays(-2)
        });
        Session.Store(new PaperStatusHistoryEntity
        {
            Id = Guid.NewGuid(), PaperId = paper.Id, Status = SubmissionStatus.Submitted,
            CreatedOnUtc = DateTimeOffset.UtcNow
        });
        await Session.SaveChangesAsync();

        var handler = new GetPaperStatusHistoryQueryHandler(Session, Mapper);
        var result = await handler.Handle(new GetPaperStatusHistoryQuery(paper.Id), CancellationToken.None);

        result.CurrentStatus.Should().Be(SubmissionStatus.Submitted);
        result.History.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetPaperStatusHistory_WithNoHistory_ShouldReturnDraftStatus()
    {
        var paper = SeedPaper();
        await Session.SaveChangesAsync();

        var handler = new GetPaperStatusHistoryQueryHandler(Session, Mapper);
        var result = await handler.Handle(new GetPaperStatusHistoryQuery(paper.Id), CancellationToken.None);

        result.CurrentStatus.Should().Be(SubmissionStatus.Draft);
    }

    [Fact]
    public async Task GetPaperStatusHistory_WithNonExistentPaper_ShouldThrowNotFoundException()
    {
        var handler = new GetPaperStatusHistoryQueryHandler(Session, Mapper);
        var act = () => handler.Handle(new GetPaperStatusHistoryQuery(Guid.NewGuid()), CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    #region GetSectionsByPaperId

    [Fact]
    public async Task GetSectionsByPaperId_WithMainSections_ShouldReturnOrderedSections()
    {
        var paperId = Guid.NewGuid();
        var paper = PaperEntity.Create(paperId, "Test");
        Session.Store(paper);
        Session.Store(new SectionEntity
        {
            Id = Guid.NewGuid(), PaperId = paperId, Title = "Intro",
            IsMainSection = true, DisplayOrder = 1, CreatedOnUtc = DateTimeOffset.UtcNow
        });
        Session.Store(new SectionEntity
        {
            Id = Guid.NewGuid(), PaperId = paperId, Title = "Method",
            IsMainSection = true, DisplayOrder = 2, CreatedOnUtc = DateTimeOffset.UtcNow
        });
        Session.Store(new SectionEntity
        {
            Id = Guid.NewGuid(), PaperId = paperId, Title = "Draft Note",
            IsMainSection = false, DisplayOrder = 3, CreatedOnUtc = DateTimeOffset.UtcNow
        });
        await Session.SaveChangesAsync();

        var handler = new GetSectionsByPaperIdQueryHandler(Session, Mapper);
        var result = await handler.Handle(new GetSectionsByPaperIdQuery(paperId), CancellationToken.None);

        result.Items.Should().HaveCount(2); // only main sections
    }

    #endregion

    #region GetSubmissionStatusSummary

    [Fact]
    public async Task GetSubmissionStatusSummary_WithEmptyPaperIds_ShouldReturnEmpty()
    {
        var handler = new GetSubmissionStatusSummaryQueryHandler(Session);
        var result = await handler.Handle(
            new GetSubmissionStatusSummaryQuery(new List<Guid>()), CancellationToken.None);

        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSubmissionStatusSummary_WithPapersNoHistory_ShouldReturnAllAsDraft()
    {
        var p1 = SeedPaper("P1"); var p2 = SeedPaper("P2");
        await Session.SaveChangesAsync();

        var handler = new GetSubmissionStatusSummaryQueryHandler(Session);
        var result = await handler.Handle(
            new GetSubmissionStatusSummaryQuery(new List<Guid> { p1.Id, p2.Id }), CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].Status.Should().Be((int)SubmissionStatus.Draft);
        result.Items[0].Count.Should().Be(2);
    }

    [Fact]
    public async Task GetSubmissionStatusSummary_WithMixedHistory_ShouldAggregateCorrectly()
    {
        var p1 = SeedPaper("P1"); var p2 = SeedPaper("P2"); var p3 = SeedPaper("P3");
        Session.Store(new PaperStatusHistoryEntity
        {
            Id = Guid.NewGuid(), PaperId = p1.Id, Status = SubmissionStatus.Submitted,
            CreatedOnUtc = DateTimeOffset.UtcNow
        });
        Session.Store(new PaperStatusHistoryEntity
        {
            Id = Guid.NewGuid(), PaperId = p2.Id, Status = SubmissionStatus.Submitted,
            CreatedOnUtc = DateTimeOffset.UtcNow
        });
        await Session.SaveChangesAsync();

        var handler = new GetSubmissionStatusSummaryQueryHandler(Session);
        var result = await handler.Handle(
            new GetSubmissionStatusSummaryQuery(new List<Guid> { p1.Id, p2.Id, p3.Id }),
            CancellationToken.None);

        var draftItem = result.Items.FirstOrDefault(x => x.Status == (int)SubmissionStatus.Draft);
        var submittedItem = result.Items.FirstOrDefault(x => x.Status == (int)SubmissionStatus.Submitted);
        draftItem.Should().NotBeNull();
        draftItem!.Count.Should().Be(1); // p3 has no history
        submittedItem.Should().NotBeNull();
        submittedItem!.Count.Should().Be(2);
    }

    #endregion

    #region GetPaperVersionFileById

    [Fact]
    public async Task GetPaperVersionFileById_WithExisting_ShouldReturnMapped()
    {
        var file = new PaperVersionFileEntity
        {
            Id = Guid.NewGuid(), PaperVersionId = Guid.NewGuid(),
            FileName = "test.pdf", FileUrl = "https://example.com/test.pdf",
            CreatedOnUtc = DateTimeOffset.UtcNow
        };
        Session.Store(file);
        await Session.SaveChangesAsync();

        var handler = new GetPaperVersionFileByIdQueryHandler(Session, Mapper);
        var result = await handler.Handle(new GetPaperVersionFileByIdQuery(file.Id), CancellationToken.None);

        result.FileName.Should().Be("test.pdf");
    }

    [Fact]
    public async Task GetPaperVersionFileById_WithNonExistent_ShouldThrowNotFoundException()
    {
        var handler = new GetPaperVersionFileByIdQueryHandler(Session, Mapper);
        var act = () => handler.Handle(new GetPaperVersionFileByIdQuery(Guid.NewGuid()), CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    #region GetPaperVersionFiles

    [Fact]
    public async Task GetPaperVersionFiles_WithExistingVersion_ShouldReturnFiles()
    {
        var paper = SeedPaper();
        var version = new PaperVersionEntity
        {
            Id = Guid.NewGuid(), PaperId = paper.Id, Name = "v1",
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow
        };
        Session.Store(version);
        Session.Store(new PaperVersionFileEntity
        {
            Id = Guid.NewGuid(), PaperVersionId = version.Id,
            FileName = "a.pdf", FileUrl = "url", CreatedOnUtc = DateTimeOffset.UtcNow
        });
        await Session.SaveChangesAsync();

        var handler = new GetPaperVersionFilesQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetPaperVersionFilesQuery(paper.Id, version.Id), CancellationToken.None);

        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetPaperVersionFiles_WithWrongPaperId_ShouldThrowClientValidationException()
    {
        var version = new PaperVersionEntity
        {
            Id = Guid.NewGuid(), PaperId = Guid.NewGuid(), Name = "v1",
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow
        };
        Session.Store(version);
        await Session.SaveChangesAsync();

        var handler = new GetPaperVersionFilesQueryHandler(Session, Mapper);
        var act = () => handler.Handle(
            new GetPaperVersionFilesQuery(Guid.NewGuid(), version.Id), CancellationToken.None);
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    [Fact]
    public async Task GetPaperVersionFiles_WithNonExistentVersion_ShouldThrowNotFoundException()
    {
        var handler = new GetPaperVersionFilesQueryHandler(Session, Mapper);
        var act = () => handler.Handle(
            new GetPaperVersionFilesQuery(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    #region GetCombinePaperById

    [Fact]
    public async Task GetCombinePaperById_WithExistingPaperAndVersion_ShouldReturnCombinedResult()
    {
        var paper = SeedPaper();
        var version = new PaperVersionEntity
        {
            Id = Guid.NewGuid(), PaperId = paper.Id, Name = "v1",
            Content = "combined", CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow
        };
        Session.Store(version);
        await Session.SaveChangesAsync();

        var handler = new GetCombinePaperByIdQueryHandler(Session);
        var result = await handler.Handle(
            new GetCombinePaperByIdQuery(paper.Id, version.Id), CancellationToken.None);

        result.Version.Should().NotBeNull();
        result.Version.Name.Should().Be("v1");
    }

    [Fact]
    public async Task GetCombinePaperById_WithNonExistentPaper_ShouldThrowNotFoundException()
    {
        var handler = new GetCombinePaperByIdQueryHandler(Session);
        var act = () => handler.Handle(
            new GetCombinePaperByIdQuery(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion
}
