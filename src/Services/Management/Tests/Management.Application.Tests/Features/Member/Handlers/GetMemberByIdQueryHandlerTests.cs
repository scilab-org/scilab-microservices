using AutoMapper;
using Management.Application.Dtos.Members;
using Management.Application.Features.Member.Queries;
using Management.Application.Tests.Common;
using Management.Application.Tests.Common.TestData;

namespace Management.Application.Tests.Features.Member.Handlers;

public class GetMemberByIdQueryHandlerTests : BaseTest
{
    private readonly Mock<IDocumentSession> _sessionMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetMemberByIdQueryHandler _handler;

    public GetMemberByIdQueryHandlerTests()
    {
        _sessionMock = new Mock<IDocumentSession>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetMemberByIdQueryHandler(_sessionMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_MemberNotFound_ReturnsEmptyMemberDto()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var query = new GetMemberByIdQuery(memberId);

        _sessionMock.Setup(s => s.LoadAsync<MemberEntity>(memberId, CancellationToken))
            .ReturnsAsync((MemberEntity?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(Guid.Empty);
        _mapperMock.Verify(m => m.Map<MemberDto>(It.IsAny<MemberEntity>()), Times.Never);
    }

    [Fact]
    public async Task Handle_MemberFound_ReturnsMappedDto()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var query = new GetMemberByIdQuery(memberId);

        var member = MemberTestData.CreateMemberEntity(
            id: memberId, userId: userId, projectId: projectId, projectRole: "project:manager");
        _sessionMock.Setup(s => s.LoadAsync<MemberEntity>(memberId, CancellationToken))
            .ReturnsAsync(member);

        var expectedDto = new MemberDto
        {
            UserId = userId,
            ProjectId = projectId,
            ProjectRole = "project:manager"
        };
        _mapperMock.Setup(m => m.Map<MemberDto>(member)).Returns(expectedDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken);

        // Assert
        result.Should().Be(expectedDto);
        result.UserId.Should().Be(userId);
        result.ProjectId.Should().Be(projectId);
        result.ProjectRole.Should().Be("project:manager");
        _mapperMock.Verify(m => m.Map<MemberDto>(member), Times.Once);
    }
}
