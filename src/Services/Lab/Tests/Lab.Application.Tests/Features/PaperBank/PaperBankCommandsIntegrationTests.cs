using Lab.Application.Dtos.PaperBanks;
using Lab.Application.Features.PaperBank.Commands.CreatePaperBank;
using Lab.Application.Features.PaperBank.Commands.DeletePaperBank;
using Lab.Application.Features.PaperBank.Commands.UpdatePaperBank;
using Lab.Application.Repositories;
using Lab.Application.Services;
using Lab.Application.Tests.Common;
using Lab.Domain.Entities;
using Lab.Domain.Enums;
using MediatR;

namespace Lab.Application.Tests.Features.PaperBank;

public class PaperBankCommandsIntegrationTests : MartenTestBase
{
    protected override string SchemaName => "paper_bank_cmd_tests";

    private readonly Mock<IManagementApiService> _mockMgmt = new();
    private readonly Mock<IMinIoCloudService> _mockMinIo = new();
    private readonly Mock<IOutboxRepository> _mockOutbox = new();

    private async Task<ConferenceJournalEntity> SeedJournalAsync()
    {
        var journal = ConferenceJournalEntity.Create(
            Guid.NewGuid(), "Test Journal", "Q1", "https://j.com", "1234-5678",
            "IEEE", ConferenceJournalType.Journal, [], null, null);
        Session.Store(journal);
        await Session.SaveChangesAsync();
        return journal;
    }

    private static CreatePaperBankDto BuildCreateDto(Guid journalId) => new()
    {
        Title = "Test Paper Bank",
        Authors = "Author One",
        Publisher = "Test Publisher",
        ConferenceJournalId = journalId,
        UploadPdfFile = new UploadFileBytes
        {
            FileName = "paper.pdf",
            Bytes = new byte[] { 1, 2, 3 },
            ContentType = "application/pdf"
        },
    };

    private static UpdatePaperBankDto BuildUpdateDto(Guid journalId) => new()
    {
        Title = "Updated Paper Bank",
        ConferenceJournalId = journalId,
    };

    #region CreatePaperBank

    [Fact]
    public async Task CreatePaperBank_WithValidData_ShouldStoreAndReturnId()
    {
        var journal = await SeedJournalAsync();

        _mockMinIo.Setup(x => x.UploadFilesAsync(
                It.IsAny<string>(), It.IsAny<List<UploadFileBytes>>(),
                It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UploadFileResult>
            {
                new() { PublicURL = "https://storage.test/paper.pdf", OriginalFileName = "paper.pdf" }
            });

        _mockOutbox.Setup(x => x.AddMessageAsync(It.IsAny<OutboxMessageEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var dto = BuildCreateDto(journal.Id);
        var handler = new CreatePaperBankCommandHandler(Session, _mockMinIo.Object, _mockOutbox.Object);

        var result = await handler.Handle(new CreatePaperBankCommand(dto), CancellationToken.None);

        result.Should().NotBeEmpty();
        var stored = await Session.LoadAsync<PaperBankEntity>(result);
        stored.Should().NotBeNull();
        stored!.Title.Should().Be("Test Paper Bank");
        stored.ConferenceJournalId.Should().Be(journal.Id);
        stored.IngestStatus.Should().Be(IngestStatus.Pending);
    }

    [Fact]
    public async Task CreatePaperBank_WithNonExistentJournal_ShouldThrowNotFoundException()
    {
        _mockMinIo.Setup(x => x.UploadFilesAsync(
                It.IsAny<string>(), It.IsAny<List<UploadFileBytes>>(),
                It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UploadFileResult>());
        _mockOutbox.Setup(x => x.AddMessageAsync(It.IsAny<OutboxMessageEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var dto = BuildCreateDto(Guid.NewGuid()); // journal not in DB
        var handler = new CreatePaperBankCommandHandler(Session, _mockMinIo.Object, _mockOutbox.Object);

        var act = () => handler.Handle(new CreatePaperBankCommand(dto), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreatePaperBank_WithKeywords_ShouldNormalizeAndStore()
    {
        var journal = await SeedJournalAsync();

        _mockMinIo.Setup(x => x.UploadFilesAsync(
                It.IsAny<string>(), It.IsAny<List<UploadFileBytes>>(),
                It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UploadFileResult>());
        _mockOutbox.Setup(x => x.AddMessageAsync(It.IsAny<OutboxMessageEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var dto = new CreatePaperBankDto
        {
            Title = "Keyword Paper",
            ConferenceJournalId = journal.Id,
            Keywords = new List<string> { "Machine Learning", "AI" },
            UploadPdfFile = new UploadFileBytes
            {
                FileName = "paper.pdf",
                Bytes = new byte[] { 1 },
                ContentType = "application/pdf"
            },
        };

        var handler = new CreatePaperBankCommandHandler(Session, _mockMinIo.Object, _mockOutbox.Object);
        var result = await handler.Handle(new CreatePaperBankCommand(dto), CancellationToken.None);

        var stored = await Session.LoadAsync<PaperBankEntity>(result);
        stored.Should().NotBeNull();
        stored!.Keywords.Should().Contain("machine learning");
        stored.Keywords.Should().Contain("ai");
    }

    #endregion

    #region DeletePaperBank

    [Fact]
    public async Task DeletePaperBank_WithExistingEntity_ShouldDeleteAndReturnUnit()
    {
        var journal = await SeedJournalAsync();
        var entity = PaperBankEntity.Create(Guid.NewGuid(), "Paper To Delete", conferenceJournalId: journal.Id);
        Session.Store(entity);
        await Session.SaveChangesAsync();

        _mockMgmt.Setup(x => x.DeleteProjectPaperByBankIdAsync(entity.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Guid>());

        var handler = new DeletePaperBankCommandHandler(Session, _mockMgmt.Object);
        var result = await handler.Handle(new DeletePaperBankCommand(entity.Id), CancellationToken.None);

        result.Should().Be(Unit.Value);
        var stored = await Session.LoadAsync<PaperBankEntity>(entity.Id);
        stored.Should().BeNull();
    }

    [Fact]
    public async Task DeletePaperBank_WithNonExistentId_ShouldThrowClientValidationException()
    {
        _mockMgmt.Setup(x => x.DeleteProjectPaperByBankIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Guid>());

        var handler = new DeletePaperBankCommandHandler(Session, _mockMgmt.Object);
        var act = () => handler.Handle(new DeletePaperBankCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<ClientValidationException>();
    }

    #endregion

    #region UpdatePaperBank

    [Fact]
    public async Task UpdatePaperBank_WithValidData_ShouldUpdateAndReturnId()
    {
        var journal = await SeedJournalAsync();
        var entity = PaperBankEntity.Create(Guid.NewGuid(), "Old Title", conferenceJournalId: journal.Id);
        Session.Store(entity);
        await Session.SaveChangesAsync();

        _mockMinIo.Setup(x => x.UploadFilesAsync(
                It.IsAny<string>(), It.IsAny<List<UploadFileBytes>>(),
                It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UploadFileResult>());

        var dto = BuildUpdateDto(journal.Id);
        var handler = new UpdatePaperCommandBankHandler(Session, _mockMinIo.Object);
        var result = await handler.Handle(new UpdatePaperBankCommand(entity.Id, dto), CancellationToken.None);

        result.Should().Be(entity.Id);
        var updated = await Session.LoadAsync<PaperBankEntity>(entity.Id);
        updated!.Title.Should().Be("Updated Paper Bank");
    }

    [Fact]
    public async Task UpdatePaperBank_WithNonExistentId_ShouldThrowClientValidationException()
    {
        var journal = await SeedJournalAsync();
        _mockMinIo.Setup(x => x.UploadFilesAsync(
                It.IsAny<string>(), It.IsAny<List<UploadFileBytes>>(),
                It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UploadFileResult>());

        var dto = BuildUpdateDto(journal.Id);
        var handler = new UpdatePaperCommandBankHandler(Session, _mockMinIo.Object);
        var act = () => handler.Handle(new UpdatePaperBankCommand(Guid.NewGuid(), dto), CancellationToken.None);

        await act.Should().ThrowAsync<ClientValidationException>();
    }

    [Fact]
    public async Task UpdatePaperBank_WithNonExistentJournal_ShouldThrowClientValidationException()
    {
        var journal = await SeedJournalAsync();
        var entity = PaperBankEntity.Create(Guid.NewGuid(), "Paper", conferenceJournalId: journal.Id);
        Session.Store(entity);
        await Session.SaveChangesAsync();

        _mockMinIo.Setup(x => x.UploadFilesAsync(
                It.IsAny<string>(), It.IsAny<List<UploadFileBytes>>(),
                It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UploadFileResult>());

        var dto = new UpdatePaperBankDto { Title = "Updated", ConferenceJournalId = Guid.NewGuid() };
        var handler = new UpdatePaperCommandBankHandler(Session, _mockMinIo.Object);
        var act = () => handler.Handle(new UpdatePaperBankCommand(entity.Id, dto), CancellationToken.None);

        await act.Should().ThrowAsync<ClientValidationException>();
    }

    [Fact]
    public async Task UpdatePaperBank_WithInvalidGapTypeId_ShouldThrowClientValidationException()
    {
        var journal = await SeedJournalAsync();
        var entity = PaperBankEntity.Create(Guid.NewGuid(), "Paper", conferenceJournalId: journal.Id);
        Session.Store(entity);
        await Session.SaveChangesAsync();

        _mockMinIo.Setup(x => x.UploadFilesAsync(
                It.IsAny<string>(), It.IsAny<List<UploadFileBytes>>(),
                It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UploadFileResult>());

        var dto = new UpdatePaperBankDto
        {
            Title = "Updated",
            ConferenceJournalId = journal.Id,
            GapTypeIds = new List<Guid> { Guid.NewGuid() } // not in DB
        };
        var handler = new UpdatePaperCommandBankHandler(Session, _mockMinIo.Object);
        var act = () => handler.Handle(new UpdatePaperBankCommand(entity.Id, dto), CancellationToken.None);

        await act.Should().ThrowAsync<ClientValidationException>();
    }

    #endregion
}
