using Common.Constants;
using Lab.Application.Features.PaperContributor.Queries.GetAssignedPaperSections;
using Lab.Application.Features.PaperContributor.Queries.GetAssignedPaperSectionsHistory;
using Lab.Application.Features.PaperContributor.Queries.GetAvailableMemberSection;
using Lab.Application.Features.PaperContributor.Queries.GetMemberSection;
using Lab.Application.Features.PaperContributor.Queries.GetPaperContributors;
using Lab.Application.Models.Filters;
using Lab.Application.Services;
using Lab.Application.Tests.Common;
using Lab.Domain.Entities;
using Lab.Domain.Enums;

namespace Lab.Application.Tests.Features.PaperContributor;

public class PaperContributorQueriesIntegrationTests : MartenTestBase
{
    protected override string SchemaName => "paper_contributor_query_tests";

    private readonly Mock<IManagementApiService> _mockMgmt = new();
    private readonly Mock<IUserApiService> _mockUser = new();

    private static SubProjectMemberInfo MakeMember(Guid memberId, Guid userId, string role = "SectionAuthor")
        => new(memberId, userId, role, $"user_{userId:N}", $"{userId:N}@test.com", "First", "Last");

    private async Task<SectionEntity> SeedSectionAsync(Guid paperId, string title = "Intro")
    {
        var section = SectionEntity.Create(
            Guid.NewGuid(), "", paperId, 1.0f, SectionStatus.NotStarted,
            isMainSection: true, title: title);
        Session.Store(section);
        await Session.SaveChangesAsync();
        return section;
    }

    private async Task<PaperContributorEntity> SeedContributorAsync(Guid paperId, Guid memberId, Guid? sectionId = null)
    {
        var markSectionId = sectionId ?? Guid.NewGuid();
        var contributor = PaperContributorEntity.Create(
            Guid.NewGuid(), "SectionAuthor", paperId, sectionId, memberId, markSectionId);
        Session.Store(contributor);
        await Session.SaveChangesAsync();
        return contributor;
    }

    #region GetAssignedPaperSections

    [Fact]
    public async Task GetAssignedPaperSections_MemberNotFound_ShouldThrowNotFoundException()
    {
        var paperId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _mockMgmt.Setup(x => x.GetMemberByPaperIdAsync(paperId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ValueTuple<Guid, Guid, Guid>?)null);

        var handler = new GetAssignedPaperSectionsQueryHandler(Session, _mockMgmt.Object);
        var act = () => handler.Handle(
            new GetAssignedPaperSectionsQuery(paperId, userId, new PaginationRequest()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetAssignedPaperSections_NoContributors_ShouldReturnEmptyResult()
    {
        var paperId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var subProjectId = Guid.NewGuid();

        _mockMgmt.Setup(x => x.GetMemberByPaperIdAsync(paperId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((subProjectId, memberId, Guid.NewGuid()));

        var handler = new GetAssignedPaperSectionsQueryHandler(Session, _mockMgmt.Object);
        var result = await handler.Handle(
            new GetAssignedPaperSectionsQuery(paperId, userId, new PaginationRequest()), CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.Paging.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetAssignedPaperSections_WithContributorsAndSections_ShouldReturnAssignedSections()
    {
        var paperId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var subProjectId = Guid.NewGuid();

        var section = await SeedSectionAsync(paperId, "Introduction");
        await SeedContributorAsync(paperId, memberId, section.Id);

        _mockMgmt.Setup(x => x.GetMemberByPaperIdAsync(paperId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((subProjectId, memberId, Guid.NewGuid()));

        var handler = new GetAssignedPaperSectionsQueryHandler(Session, _mockMgmt.Object);
        var result = await handler.Handle(
            new GetAssignedPaperSectionsQuery(paperId, userId, new PaginationRequest()), CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].Title.Should().Be("Introduction");
    }

    #endregion

    #region GetAssignedPaperSectionsHistory

    [Fact]
    public async Task GetAssignedPaperSectionsHistory_MemberNotFound_ShouldThrowNotFoundException()
    {
        var paperId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _mockMgmt.Setup(x => x.GetMemberByPaperIdAsync(paperId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ValueTuple<Guid, Guid, Guid>?)null);

        var handler = new GetAssignedPaperSectionsHistoryQueryHandler(Session, _mockMgmt.Object);
        var act = () => handler.Handle(
            new GetAssignedPaperSectionsHistoryQuery(paperId, userId, new GetAssignedPaperSectionsHistoryFilter(), new PaginationRequest()),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetAssignedPaperSectionsHistory_NoContributors_ShouldReturnEmptyResult()
    {
        var paperId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var memberId = Guid.NewGuid();

        _mockMgmt.Setup(x => x.GetMemberByPaperIdAsync(paperId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid.NewGuid(), memberId, Guid.NewGuid()));

        var handler = new GetAssignedPaperSectionsHistoryQueryHandler(Session, _mockMgmt.Object);
        var result = await handler.Handle(
            new GetAssignedPaperSectionsHistoryQuery(paperId, userId, new GetAssignedPaperSectionsHistoryFilter(), new PaginationRequest()),
            CancellationToken.None);

        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAssignedPaperSectionsHistory_WithContributors_NoOldMainSections_ShouldReturnEmpty()
    {
        var paperId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var memberId = Guid.NewGuid();

        // Create a section that is NOT IsOldMainSection
        var section = SectionEntity.Create(
            Guid.NewGuid(), "", paperId, 1.0f, SectionStatus.NotStarted,
            isMainSection: true, isOldMainSection: false, title: "Intro");
        Session.Store(section);
        await Session.SaveChangesAsync();

        await SeedContributorAsync(paperId, memberId, section.Id);

        _mockMgmt.Setup(x => x.GetMemberByPaperIdAsync(paperId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid.NewGuid(), memberId, Guid.NewGuid()));

        var handler = new GetAssignedPaperSectionsHistoryQueryHandler(Session, _mockMgmt.Object);
        var result = await handler.Handle(
            new GetAssignedPaperSectionsHistoryQuery(paperId, userId, new GetAssignedPaperSectionsHistoryFilter(), new PaginationRequest()),
            CancellationToken.None);

        // No IsOldMainSection sections in history chain → empty
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAssignedPaperSectionsHistory_FilterBySectionRole_ShouldFilterContributors()
    {
        var paperId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var memberId = Guid.NewGuid();

        // Create contributor with different role → should be filtered out
        var section = await SeedSectionAsync(paperId, "Methods");
        var contributor = PaperContributorEntity.Create(
            Guid.NewGuid(), "SectionRead", paperId, section.Id, memberId, section.Id);
        Session.Store(contributor);
        await Session.SaveChangesAsync();

        _mockMgmt.Setup(x => x.GetMemberByPaperIdAsync(paperId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid.NewGuid(), memberId, Guid.NewGuid()));

        var filter = new GetAssignedPaperSectionsHistoryFilter { SectionRole = "SectionAuthor" };
        var handler = new GetAssignedPaperSectionsHistoryQueryHandler(Session, _mockMgmt.Object);
        var result = await handler.Handle(
            new GetAssignedPaperSectionsHistoryQuery(paperId, userId, filter, new PaginationRequest()),
            CancellationToken.None);

        result.Items.Should().BeEmpty();
    }

    #endregion

    #region GetAvailableMemberSection

    [Fact]
    public async Task GetAvailableMemberSection_NoAssignedMembers_ShouldReturnAllMembers()
    {
        var sectionId = Guid.NewGuid();
        var paperId = Guid.NewGuid();
        var memberId1 = Guid.NewGuid();
        var memberId2 = Guid.NewGuid();
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        var allMembers = new List<SubProjectMemberInfo>
        {
            MakeMember(memberId1, userId1),
            MakeMember(memberId2, userId2),
        };

        _mockMgmt.Setup(x => x.GetSubProjectMembersByPaperIdAsync(paperId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(allMembers);

        var handler = new GetAvailableMemberSectionQueryHandler(Session, _mockMgmt.Object);
        var result = await handler.Handle(
            new GetAvailableMemberSectionQuery(sectionId, paperId), CancellationToken.None);

        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAvailableMemberSection_WithAssignedMember_ShouldExcludeAssigned()
    {
        var markSectionId = Guid.NewGuid();
        var paperId = Guid.NewGuid();
        var memberId1 = Guid.NewGuid();
        var memberId2 = Guid.NewGuid();
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        // Assign memberId1 to the section
        var contributor = PaperContributorEntity.Create(
            Guid.NewGuid(), "SectionAuthor", paperId, null, memberId1, markSectionId);
        Session.Store(contributor);
        await Session.SaveChangesAsync();

        var allMembers = new List<SubProjectMemberInfo>
        {
            MakeMember(memberId1, userId1),
            MakeMember(memberId2, userId2),
        };

        _mockMgmt.Setup(x => x.GetSubProjectMembersByPaperIdAsync(paperId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(allMembers);

        var handler = new GetAvailableMemberSectionQueryHandler(Session, _mockMgmt.Object);
        var result = await handler.Handle(
            new GetAvailableMemberSectionQuery(markSectionId, paperId), CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].MemberId.Should().Be(memberId2);
    }

    #endregion

    #region GetMemberSection

    [Fact]
    public async Task GetMemberSection_NoContributors_ShouldReturnEmptyResult()
    {
        var sectionId = Guid.NewGuid();
        var paperId = Guid.NewGuid();

        _mockMgmt.Setup(x => x.GetSubProjectMembersByPaperIdAsync(paperId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SubProjectMemberInfo>());

        var handler = new GetMemberSectionQueryHandler(Session, _mockMgmt.Object);
        var result = await handler.Handle(
            new GetMemberSectionQuery(sectionId, paperId), CancellationToken.None);

        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetMemberSection_WithContributors_ShouldReturnMemberInfo()
    {
        var markSectionId = Guid.NewGuid();
        var paperId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var contributor = PaperContributorEntity.Create(
            Guid.NewGuid(), "SectionAuthor", paperId, null, memberId, markSectionId);
        Session.Store(contributor);
        await Session.SaveChangesAsync();

        _mockMgmt.Setup(x => x.GetSubProjectMembersByPaperIdAsync(paperId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SubProjectMemberInfo> { MakeMember(memberId, userId) });

        var handler = new GetMemberSectionQueryHandler(Session, _mockMgmt.Object);
        var result = await handler.Handle(
            new GetMemberSectionQuery(markSectionId, paperId), CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].MemberId.Should().Be(memberId);
    }

    #endregion

    #region GetPaperContributors

    [Fact]
    public async Task GetPaperContributors_NoContributors_ShouldReturnEmptyResult()
    {
        var paperId = Guid.NewGuid();

        _mockMgmt.Setup(x => x.GetSubProjectMembersByPaperIdAsync(paperId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SubProjectMemberInfo>());
        _mockUser.Setup(x => x.GetUsersByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, UserInfo>());

        var handler = new GetPaperContributorsQueryHandler(Session, _mockMgmt.Object, _mockUser.Object);
        var result = await handler.Handle(new GetPaperContributorsQuery(paperId), CancellationToken.None);

        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPaperContributors_WithContributors_ShouldReturnEnrichedList()
    {
        var paperId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var markSectionId = Guid.NewGuid();

        // Role must NOT be PaperAuthor to be returned by GetPaperContributors
        var contributor = PaperContributorEntity.Create(
            Guid.NewGuid(), "SectionAuthor", paperId, null, memberId, markSectionId);
        Session.Store(contributor);
        await Session.SaveChangesAsync();

        _mockMgmt.Setup(x => x.GetSubProjectMembersByPaperIdAsync(paperId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SubProjectMemberInfo> { MakeMember(memberId, userId) });
        _mockUser.Setup(x => x.GetUsersByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, UserInfo>
            {
                { userId, new UserInfo(userId, "user1", "user1@test.com", "First", "Last") }
            });

        var handler = new GetPaperContributorsQueryHandler(Session, _mockMgmt.Object, _mockUser.Object);
        var result = await handler.Handle(new GetPaperContributorsQuery(paperId), CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].MemberId.Should().Be(memberId);
        result.Items[0].ContributorName.Should().Contain("First");
    }

    [Fact]
    public async Task GetPaperContributors_WithPaperAuthorRole_ShouldBeExcluded()
    {
        var paperId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var markSectionId = Guid.NewGuid();

        // PaperAuthor role should be excluded
        var contributor = PaperContributorEntity.Create(
            Guid.NewGuid(), AuthorizeConstants.PaperAuthor, paperId, null, memberId, markSectionId);
        Session.Store(contributor);
        await Session.SaveChangesAsync();

        _mockMgmt.Setup(x => x.GetSubProjectMembersByPaperIdAsync(paperId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SubProjectMemberInfo>());
        _mockUser.Setup(x => x.GetUsersByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, UserInfo>());

        var handler = new GetPaperContributorsQueryHandler(Session, _mockMgmt.Object, _mockUser.Object);
        var result = await handler.Handle(new GetPaperContributorsQuery(paperId), CancellationToken.None);

        result.Items.Should().BeEmpty();
    }

    #endregion
}
