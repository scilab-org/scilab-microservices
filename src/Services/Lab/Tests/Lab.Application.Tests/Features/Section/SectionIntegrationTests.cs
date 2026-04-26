using Lab.Application.Features.Section.Queries.GetSectionById;
using Lab.Application.Features.Section.Queries.GetSectionnFileById;
using Lab.Application.Features.Section.Queries.GetReferenceBySectionId;
using Lab.Application.Features.Section.Queries.GetInUseReferenceBySectionId;
using Lab.Application.Features.Section.Queries.GetPreviewReference;
using Lab.Application.Features.Section.Queries.GetSectionVersionsByMarkSectionId;
using Lab.Application.Dtos.Sections;
using Lab.Application.Tests.Common;
using Lab.Domain.Entities;
using Lab.Domain.Models;

namespace Lab.Application.Tests.Features.Section;

public class SectionIntegrationTests : MartenTestBase
{
    protected override string SchemaName => "section_tests";

    private SectionEntity SeedSection(Guid? paperId = null, string title = "Test Section",
        bool isMain = true, List<Guid>? refs = null, List<string>? files = null)
    {
        var section = new SectionEntity
        {
            Id = Guid.NewGuid(),
            PaperId = paperId ?? Guid.NewGuid(),
            Title = title,
            IsMainSection = isMain,
            DisplayOrder = 1,
            References = refs,
            Files = files,
            Content = "content",
            CreatedOnUtc = DateTimeOffset.UtcNow
        };
        Session.Store(section);
        return section;
    }

    #region GetSectionById

    [Fact]
    public async Task GetSectionById_WithExisting_ShouldReturnMappedResult()
    {
        var section = SeedSection(title: "Introduction");
        await Session.SaveChangesAsync();

        var handler = new GetSectionByIdQueryHandler(Session, Mapper);
        var result = await handler.Handle(new GetSectionByIdQuery(section.Id), CancellationToken.None);

        result.Section.Should().NotBeNull();
        result.Section.Title.Should().Be("Introduction");
    }

    [Fact]
    public async Task GetSectionById_WithNonExistent_ShouldThrowNotFoundException()
    {
        var handler = new GetSectionByIdQueryHandler(Session, Mapper);
        var act = () => handler.Handle(new GetSectionByIdQuery(Guid.NewGuid()), CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    #region GetSectionFileById

    [Fact]
    public async Task GetSectionFileById_WithFiles_ShouldReturnFileList()
    {
        var section = SeedSection(files: new List<string> { "file1.tex", "file2.tex" });
        await Session.SaveChangesAsync();

        var handler = new GetSectionnFileByIdQueryHandler(Session);
        var result = await handler.Handle(new GetSectionnFileByIdQuery(section.Id), CancellationToken.None);

        result.Should().HaveCount(2);
        result.Should().Contain("file1.tex");
    }

    [Fact]
    public async Task GetSectionFileById_WithNullFiles_ShouldReturnEmptyList()
    {
        var section = SeedSection(files: null);
        await Session.SaveChangesAsync();

        var handler = new GetSectionnFileByIdQueryHandler(Session);
        var result = await handler.Handle(new GetSectionnFileByIdQuery(section.Id), CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSectionFileById_WithNonExistent_ShouldThrowNotFoundException()
    {
        var handler = new GetSectionnFileByIdQueryHandler(Session);
        var act = () => handler.Handle(new GetSectionnFileByIdQuery(Guid.NewGuid()), CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    #region GetReferenceBySectionId

    [Fact]
    public async Task GetReferenceBySectionId_WithEmptyRefs_ShouldReturnEmptyInUse()
    {
        var paperId = Guid.NewGuid();
        var paper = PaperEntity.Create(paperId, "Test Paper");
        Session.Store(paper);
        var section = SeedSection(paperId: paperId, refs: new List<Guid>());
        await Session.SaveChangesAsync();

        var handler = new GetReferenceBySectionIdQueryHandler(Session);
        var result = await handler.Handle(new GetReferenceBySectionIdQuery(section.Id), CancellationToken.None);

        result.InUse.Should().BeEmpty();
    }

    [Fact]
    public async Task GetReferenceBySectionId_WithNonExistentSection_ShouldThrowNotFoundException()
    {
        var handler = new GetReferenceBySectionIdQueryHandler(Session);
        var act = () => handler.Handle(new GetReferenceBySectionIdQuery(Guid.NewGuid()), CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    #region GetInUseReferenceBySectionId

    [Fact]
    public async Task GetInUseReference_WithEmptyRefs_ShouldReturnEmpty()
    {
        var paperId = Guid.NewGuid();
        var paper = PaperEntity.Create(paperId, "Paper");
        Session.Store(paper);
        var section = SeedSection(paperId: paperId, refs: new List<Guid>());
        await Session.SaveChangesAsync();

        var handler = new GetInUseReferenceBySectionIdQueryHandler(Session);
        var result = await handler.Handle(
            new GetInUseReferenceBySectionIdQuery(section.Id), CancellationToken.None);

        result.PaperBanks.Should().BeEmpty();
    }

    [Fact]
    public async Task GetInUseReference_WithReferences_ShouldReturnPaperBankInfo()
    {
        var paperId = Guid.NewGuid();
        var paper = PaperEntity.Create(paperId, "Paper");
        var pb = PaperBankEntity.Create(Guid.NewGuid(), "Referenced Paper");
        paper.References = new List<Reference>
        {
            new() { PaperId = pb.Id, SectionIds = new List<Guid>() }
        };
        Session.Store(paper);
        Session.Store(pb);
        var section = SeedSection(paperId: paperId, refs: new List<Guid> { pb.Id });
        await Session.SaveChangesAsync();

        var handler = new GetInUseReferenceBySectionIdQueryHandler(Session);
        var result = await handler.Handle(
            new GetInUseReferenceBySectionIdQuery(section.Id), CancellationToken.None);

        result.PaperBanks.Should().HaveCount(1);
        result.PaperBanks[0].Title.Should().Be("Referenced Paper");
    }

    [Fact]
    public async Task GetInUseReference_WithNonExistentSection_ShouldThrowNotFoundException()
    {
        var handler = new GetInUseReferenceBySectionIdQueryHandler(Session);
        var act = () => handler.Handle(
            new GetInUseReferenceBySectionIdQuery(Guid.NewGuid()), CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    #region GetPreviewReference

    [Fact]
    public async Task GetPreviewReference_WithEmptyIds_ShouldReturnEmpty()
    {
        var handler = new GetPreviewReferenceQueryHandler(Session);
        var result = await handler.Handle(
            new GetPreviewReferenceQuery(new PreviewReferenceDto { PaperBankIds = new List<Guid>() }),
            CancellationToken.None);

        result.PaperBanks.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPreviewReference_WithExistingIds_ShouldReturnPaperBankInfo()
    {
        var pb = PaperBankEntity.Create(Guid.NewGuid(), "Preview Paper",
            referenceContent: "@article{test, title={test}}");
        Session.Store(pb);
        await Session.SaveChangesAsync();

        var handler = new GetPreviewReferenceQueryHandler(Session);
        var result = await handler.Handle(
            new GetPreviewReferenceQuery(new PreviewReferenceDto { PaperBankIds = new List<Guid> { pb.Id } }),
            CancellationToken.None);

        result.PaperBanks.Should().HaveCount(1);
        result.ReferenceContent.Should().Contain("references.bib");
    }

    #endregion

    #region GetSectionVersionsByMarkSectionId

    [Fact]
    public async Task GetSectionVersions_WithNoVersions_ShouldReturnEmpty()
    {
        var section = SeedSection();
        await Session.SaveChangesAsync();

        var handler = new GetSectionVersionsByMarkSectionIdQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetSectionVersionsByMarkSectionIdQuery(section.Id), CancellationToken.None);

        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSectionVersions_WithNonExistentSection_ShouldReturnEmpty()
    {
        var handler = new GetSectionVersionsByMarkSectionIdQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetSectionVersionsByMarkSectionIdQuery(Guid.NewGuid()), CancellationToken.None);

        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSectionVersions_WithVersionChain_ShouldReturnOldMainSections()
    {
        var paperId = Guid.NewGuid();
        // Build version chain: oldV1 <- current
        var oldV1 = new SectionEntity
        {
            Id = Guid.NewGuid(), PaperId = paperId, Title = "Old Intro",
            IsMainSection = false, IsOldMainSection = true, DisplayOrder = 1,
            PreviousVersionSectionId = null, CreatedOnUtc = DateTimeOffset.UtcNow.AddDays(-2)
        };
        Session.Store(oldV1);

        var current = new SectionEntity
        {
            Id = Guid.NewGuid(), PaperId = paperId, Title = "Intro",
            IsMainSection = true, IsOldMainSection = false, DisplayOrder = 1,
            PreviousVersionSectionId = oldV1.Id, CreatedOnUtc = DateTimeOffset.UtcNow
        };
        Session.Store(current);
        await Session.SaveChangesAsync();

        var handler = new GetSectionVersionsByMarkSectionIdQueryHandler(Session, Mapper);
        var result = await handler.Handle(
            new GetSectionVersionsByMarkSectionIdQuery(current.Id), CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].Title.Should().Be("Old Intro");
    }

    #endregion
}
