using Marten;
using User.Application.Features.System;
using User.Application.Tests.Common;
using User.Application.Tests.Common.TestData;

namespace User.Application.Tests.Features.System;

public sealed class InitialDataCommandHandlerTests : BaseTest
{
    private readonly Mock<IDocumentSession> _documentSession;
    private readonly Mock<ISeedDataService> _seedDataService;
    private readonly InitialDataCommandHandler _handler;

    public InitialDataCommandHandlerTests()
    {
        _documentSession = new Mock<IDocumentSession>();
        _seedDataService = new Mock<ISeedDataService>();
        _handler = new InitialDataCommandHandler(
            _documentSession.Object,
            _seedDataService.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTrue_WhenSeedDataSucceeds()
    {
        // Arrange
        var command = new InitialDataCommand(UserTestData.SystemActor());

        _seedDataService
            .Setup(s => s.SeedDataAsync(_documentSession.Object, CancellationToken))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Should().BeTrue();
        _seedDataService.Verify(
            s => s.SeedDataAsync(_documentSession.Object, CancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenSeedDataReturnsFalse()
    {
        // Arrange
        var command = new InitialDataCommand(UserTestData.SystemActor());

        _seedDataService
            .Setup(s => s.SeedDataAsync(_documentSession.Object, CancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldPropagateException_WhenSeedDataThrows()
    {
        // Arrange
        var command = new InitialDataCommand(UserTestData.SystemActor());

        _seedDataService
            .Setup(s => s.SeedDataAsync(_documentSession.Object, CancellationToken))
            .ThrowsAsync(new InvalidOperationException("Seed data failed"));

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Seed data failed");
    }
}
