using Lab.Application.Dtos.PaperContributors;
using Lab.Application.Features.PaperContributor.Commands.CreatePaperContributor;
using Lab.Application.Features.PaperContributor.Commands.DeletePaperContributor;
using Lab.Application.Features.PaperContributor.Commands.UpdatePaperContributor;
using Lab.Application.Tests.Common;
using Common.Constants;
using Lab.Domain.Entities;
using Lab.Domain.Enums;

namespace Lab.Application.Tests.Features.PaperContributor;

public class PaperContributorIntegrationTests : MartenTestBase
{
    protected override string SchemaName => "paper_contributor_tests";

    [Fact]
    public async Task CreatePaperContributor_WithPaperAuthorRole_ShouldStoreAndReturnIds()
    {
        // Arrange — PaperAuthor role skips reference section logic
        var paperId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var markSectionId = Guid.NewGuid();
        var dto = new CreatePaperContributorDto
        {
            SectionRole = AuthorizeConstants.PaperAuthor,
            PaperId = paperId,
            MemberIds = new List<Guid> { memberId },
            MarkSectionId = markSectionId
        };
        var handler = new CreatePaperContributorCommandHandler(Session);

        // Act
        var result = await handler.Handle(new CreatePaperContributorCommand(dto), CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        var stored = await Session.LoadAsync<PaperContributorEntity>(result.First());
        stored.Should().NotBeNull();
        stored!.PaperId.Should().Be(paperId);
        stored.MemberId.Should().Be(memberId);
        stored.SectionRole.Should().Be(AuthorizeConstants.PaperAuthor);
    }

    [Fact]
    public async Task CreatePaperContributor_WithMultipleMembers_ShouldCreateOnePerMember()
    {
        var dto = new CreatePaperContributorDto
        {
            SectionRole = AuthorizeConstants.PaperAuthor,
            PaperId = Guid.NewGuid(),
            MemberIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() },
            MarkSectionId = Guid.NewGuid()
        };
        var handler = new CreatePaperContributorCommandHandler(Session);

        var result = await handler.Handle(new CreatePaperContributorCommand(dto), CancellationToken.None);

        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task UpdatePaperContributor_WithValidData_ShouldUpdateFields()
    {
        // Arrange — seed a contributor
        var entity = PaperContributorEntity.Create(
            Guid.NewGuid(), "OldRole", Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        Session.Store(entity);
        await Session.SaveChangesAsync();

        var newMemberId = Guid.NewGuid();
        var handler = new UpdatePaperContributorCommandHandler(Session);
        var command = new UpdatePaperContributorCommand(entity.Id,
            new UpdatePaperContributorDto { MemberId = newMemberId, SectionRole = "NewRole" });

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(entity.Id);
        var updated = await Session.LoadAsync<PaperContributorEntity>(entity.Id);
        updated!.MemberId.Should().Be(newMemberId);
        updated.SectionRole.Should().Be("NewRole");
    }

    [Fact]
    public async Task UpdatePaperContributor_WithNonExistentId_ShouldThrowException()
    {
        var handler = new UpdatePaperContributorCommandHandler(Session);
        var command = new UpdatePaperContributorCommand(Guid.NewGuid(),
            new UpdatePaperContributorDto { MemberId = Guid.NewGuid() });

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task DeletePaperContributor_WithExistingEntity_ShouldRemoveFromStore()
    {
        var entity = PaperContributorEntity.Create(
            Guid.NewGuid(), "Role", Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        Session.Store(entity);
        await Session.SaveChangesAsync();

        var handler = new DeletePaperContributorCommandHandler(Session);
        await handler.Handle(new DeletePaperContributorCommand(entity.Id), CancellationToken.None);

        var deleted = await Session.Query<PaperContributorEntity>()
            .FirstOrDefaultAsync(x => x.Id == entity.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeletePaperContributor_WithNonExistentId_ShouldThrowNotFoundException()
    {
        var handler = new DeletePaperContributorCommandHandler(Session);
        var act = () => handler.Handle(new DeletePaperContributorCommand(Guid.NewGuid()), CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreatePaperContributor_WithNonPaperAuthorRole_AndReferenceSection_ShouldCreateRefContributor()
    {
        // Arrange — non-PaperAuthor role; seed a "References" main section
        var paperId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var referenceSection = SectionEntity.Create(
            Guid.NewGuid(), "content", paperId, 1, SectionStatus.NotStarted,
            isMainSection: true, title: "References");
        Session.Store(referenceSection);
        await Session.SaveChangesAsync();

        var dto = new CreatePaperContributorDto
        {
            SectionRole = AuthorizeConstants.SectionEdit,
            PaperId = paperId,
            MemberIds = new List<Guid> { memberId },
            MarkSectionId = Guid.NewGuid()
        };
        var handler = new CreatePaperContributorCommandHandler(Session);

        // Act
        var result = await handler.Handle(new CreatePaperContributorCommand(dto), CancellationToken.None);

        // Assert - main contributor + reference contributor
        result.Should().HaveCount(1);
        var allContributors = await Session.Query<PaperContributorEntity>()
            .Where(x => x.PaperId == paperId && x.MemberId == memberId)
            .ToListAsync();
        allContributors.Should().HaveCount(2); // main + reference
    }

    [Fact]
    public async Task CreatePaperContributor_WithNonPaperAuthorRole_AlreadyAssignedToReference_ShouldNotDuplicate()
    {
        var paperId = Guid.NewGuid();
        var memberId = Guid.NewGuid();

        var referenceSection = SectionEntity.Create(
            Guid.NewGuid(), "content", paperId, 1, SectionStatus.NotStarted,
            isMainSection: true, title: "References");
        Session.Store(referenceSection);

        // Pre-assign the member to the reference section
        var existing = PaperContributorEntity.Create(
            Guid.NewGuid(), AuthorizeConstants.SectionEdit, paperId,
            referenceSection.Id, memberId, referenceSection.Id);
        Session.Store(existing);
        await Session.SaveChangesAsync();

        var dto = new CreatePaperContributorDto
        {
            SectionRole = AuthorizeConstants.SectionEdit,
            PaperId = paperId,
            MemberIds = new List<Guid> { memberId },
            MarkSectionId = Guid.NewGuid()
        };
        var handler = new CreatePaperContributorCommandHandler(Session);
        var result = await handler.Handle(new CreatePaperContributorCommand(dto), CancellationToken.None);

        result.Should().HaveCount(1);
        var allContributors = await Session.Query<PaperContributorEntity>()
            .Where(x => x.PaperId == paperId && x.MemberId == memberId)
            .ToListAsync();
        // Should still be 2 (old existing + new main, but NOT a new duplicate reference)
        allContributors.Should().HaveCount(2);
    }
}
