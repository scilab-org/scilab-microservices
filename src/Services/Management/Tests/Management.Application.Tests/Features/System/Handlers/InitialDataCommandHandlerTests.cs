using Management.Application.Features.System;
using Management.Application.Tests.Common;

namespace Management.Application.Tests.Features.System.Handlers;

public class InitialDataCommandHandlerTests : BaseTest
{
    private readonly Mock<IDocumentSession> _sessionMock;
    private readonly Mock<ISeedDataService> _seedDataServiceMock;
    private readonly InitialDataCommandHandler _handler;

    public InitialDataCommandHandlerTests()
    {
        _sessionMock = new Mock<IDocumentSession>();
        _seedDataServiceMock = new Mock<ISeedDataService>();
        _handler = new InitialDataCommandHandler(_sessionMock.Object, _seedDataServiceMock.Object);
    }

    [Fact]
    public async Task Handle_SeedSucceeds_ReturnsTrue()
    {
        // Arrange
        var command = new InitialDataCommand(Actor.System("test"));
        _seedDataServiceMock.Setup(s => s.SeedDataAsync(_sessionMock.Object, CancellationToken))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Should().BeTrue();
        _seedDataServiceMock.Verify(s => s.SeedDataAsync(_sessionMock.Object, CancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_SeedFails_ReturnsFalse()
    {
        // Arrange
        var command = new InitialDataCommand(Actor.System("test"));
        _seedDataServiceMock.Setup(s => s.SeedDataAsync(_sessionMock.Object, CancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Should().BeFalse();
    }
}
