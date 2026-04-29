using Lab.Application.Dtos.Journals;
using Lab.Application.Features.Journal.Commands.CreateJournal;
using Lab.Application.Features.Journal.Commands.DeleteJournal;
using Lab.Application.Features.Journal.Commands.UpdateJournal;
using Lab.Application.Features.Journal.Queries.GetJournalById;
using Lab.Application.Services;
using Lab.Application.Tests.Common;
using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Moq;

namespace Lab.Application.Tests.Features.Journal;

public class JournalCommandsIntegrationTests : MartenTestBase
{
    protected override string SchemaName => "journal_cmd_tests";

    private readonly Mock<IMinIoCloudService> _mockMinIo = new();
    private readonly Mock<IManagementApiService> _mockMgmt = new();

    #region CreateJournal

    [Fact]
    public async Task CreateJournal_WithValidData_ShouldStoreAndReturnId()
    {
        var template = new TemplateEntity
        {
            Id = Guid.NewGuid(), Code = "TPL-001",
            CreatedOnUtc = DateTimeOffset.UtcNow
        };
        Session.Store(template);
        await Session.SaveChangesAsync();

        _mockMinIo
            .Setup(x => x.UploadFilesAsync(It.IsAny<string>(), It.IsAny<List<UploadFileBytes>>(),
                It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UploadFileResult>());

        var handler = new CreateJournalCommandHandler(Session, _mockMinIo.Object);
        var dto = new CreateJournalEntityDto
        {
            Name = "IEEE Trans on SE",
            Ranking = "Q1",
            Url = "https://ieee.org",
            ISSN = "1234-5678",
            Type = ConferenceJournalType.Journal,
            TemplateIds = new List<Guid> { template.Id }
        };

        var result = await handler.Handle(new CreateJournalCommand(dto, "admin"), CancellationToken.None);

        result.Should().NotBeEmpty();
        var stored = await Session.LoadAsync<ConferenceJournalEntity>(result);
        stored.Should().NotBeNull();
        stored!.Name.Should().Be("IEEE Trans on SE");
    }

    [Fact]
    public async Task CreateJournal_WithDuplicateName_ShouldThrowClientValidationException()
    {
        Session.Store(ConferenceJournalEntity.Create(Guid.NewGuid(), "Existing",
            null, null, null, null, ConferenceJournalType.Journal, new List<Guid>(), null, null));
        await Session.SaveChangesAsync();

        _mockMinIo
            .Setup(x => x.UploadFilesAsync(It.IsAny<string>(), It.IsAny<List<UploadFileBytes>>(),
                It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UploadFileResult>());

        var handler = new CreateJournalCommandHandler(Session, _mockMinIo.Object);
        var dto = new CreateJournalEntityDto
        {
            Name = "Existing",
            Ranking = "Q1", Url = "url", ISSN = "1234",
            Type = ConferenceJournalType.Journal,
            TemplateIds = new List<Guid> { Guid.NewGuid() }
        };

        var act = () => handler.Handle(new CreateJournalCommand(dto, "admin"), CancellationToken.None);
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    #endregion

    #region UpdateJournal

    [Fact]
    public async Task UpdateJournal_WithValidData_ShouldUpdateFields()
    {
        var template = new TemplateEntity
        {
            Id = Guid.NewGuid(), Code = "TPL-002",
            CreatedOnUtc = DateTimeOffset.UtcNow
        };
        Session.Store(template);
        var journal = ConferenceJournalEntity.Create(Guid.NewGuid(), "Original",
            "Q2", "url", "issn", null, ConferenceJournalType.Journal,
            new List<Guid> { template.Id }, null, null);
        Session.Store(journal);
        await Session.SaveChangesAsync();

        _mockMinIo
            .Setup(x => x.UploadFilesAsync(It.IsAny<string>(), It.IsAny<List<UploadFileBytes>>(),
                It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UploadFileResult>());

        var handler = new UpdateJournalCommandHandler(Session, _mockMinIo.Object);
        var dto = new UpdateJournalEntityDto
        {
            Name = "Updated", Ranking = "Q1", Url = "new-url", ISSN = "new-issn",
            TemplateIds = new List<Guid> { template.Id }
        };

        var result = await handler.Handle(
            new UpdateJournalCommand(dto, journal.Id, "admin"), CancellationToken.None);

        result.Should().Be(journal.Id);
        var updated = await Session.LoadAsync<ConferenceJournalEntity>(journal.Id);
        updated!.Name.Should().Be("Updated");
    }

    [Fact]
    public async Task UpdateJournal_WithNonExistent_ShouldThrowClientValidationException()
    {
        var handler = new UpdateJournalCommandHandler(Session, _mockMinIo.Object);
        var dto = new UpdateJournalEntityDto
        {
            Name = "X", Ranking = "Q1", Url = "url", ISSN = "issn",
            TemplateIds = new List<Guid> { Guid.NewGuid() }
        };

        var act = () => handler.Handle(
            new UpdateJournalCommand(dto, Guid.NewGuid(), "admin"), CancellationToken.None);
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    #endregion

    #region DeleteJournal

    [Fact]
    public async Task DeleteJournal_WithExisting_ShouldSoftDelete()
    {
        var journal = ConferenceJournalEntity.Create(Guid.NewGuid(), "To Delete",
            null, null, null, null, ConferenceJournalType.Journal, new List<Guid>(), null, null);
        Session.Store(journal);
        await Session.SaveChangesAsync();

        _mockMgmt
            .Setup(x => x.RemoveConferenceJournalFromProjectAsync(
                It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<Guid>?)null);

        var handler = new DeleteJournalCommandHandler(Session, _mockMgmt.Object);
        await handler.Handle(new DeleteJournalCommand(journal.Id, "admin"), CancellationToken.None);

        // Marten soft-deletes the entity; a normal Load won't find it
        _mockMgmt.Verify(x => x.RemoveConferenceJournalFromProjectAsync(
            journal.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteJournal_WithNonExistent_ShouldThrowClientValidationException()
    {
        var handler = new DeleteJournalCommandHandler(Session, _mockMgmt.Object);
        var act = () => handler.Handle(
            new DeleteJournalCommand(Guid.NewGuid(), "admin"), CancellationToken.None);
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    #endregion

    #region GetJournalById (with IManagementApiService)

    [Fact]
    public async Task GetJournalById_WithExisting_ShouldReturnFullResult()
    {
        var template = new TemplateEntity
        {
            Id = Guid.NewGuid(), Code = "TPL-GET",
            CreatedOnUtc = DateTimeOffset.UtcNow
        };
        Session.Store(template);
        var journal = ConferenceJournalEntity.Create(Guid.NewGuid(), "Test Journal",
            "Q1", "url", "issn", null, ConferenceJournalType.Journal,
            new List<Guid> { template.Id }, null, null);
        Session.Store(journal);
        await Session.SaveChangesAsync();

        _mockMgmt
            .Setup(x => x.GetProjectsByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ManagementProjectInfo>());

        var handler = new GetJournalByIdQueryHandler(Session, _mockMgmt.Object, Mapper);
        var result = await handler.Handle(new GetJournalByIdQuery(journal.Id), CancellationToken.None);

        result.Journal.Should().NotBeNull();
        result.Journal.Name.Should().Be("Test Journal");
        result.Journal.Templates.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetJournalById_WithNonExistent_ShouldThrowNotFoundException()
    {
        var handler = new GetJournalByIdQueryHandler(Session, _mockMgmt.Object, Mapper);
        var act = () => handler.Handle(new GetJournalByIdQuery(Guid.NewGuid()), CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion
}