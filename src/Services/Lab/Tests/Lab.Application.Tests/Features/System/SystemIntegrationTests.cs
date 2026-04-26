using Common.ValueObjects;
using Lab.Application.Features.System;
using Lab.Application.Services;
using Lab.Application.Tests.Common;
using Marten;
using Moq;

namespace Lab.Application.Tests.Features.System;

public class SystemIntegrationTests : MartenTestBase
{
    protected override string SchemaName => "system_tests";

    [Fact]
    public async Task InitialData_WithSuccessfulSeed_ShouldReturnTrue()
    {
        var mockSeedService = new Mock<ISeedDataService>();
        mockSeedService
            .Setup(x => x.SeedDataAsync(It.IsAny<IDocumentSession>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new InitialDataCommandHandler(Session, mockSeedService.Object);
        var result = await handler.Handle(
            new InitialDataCommand(Actor.System("system")), CancellationToken.None);

        result.Should().BeTrue();
        mockSeedService.Verify(x => x.SeedDataAsync(Session, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InitialData_WithFailedSeed_ShouldReturnFalse()
    {
        var mockSeedService = new Mock<ISeedDataService>();
        mockSeedService
            .Setup(x => x.SeedDataAsync(It.IsAny<IDocumentSession>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new InitialDataCommandHandler(Session, mockSeedService.Object);
        var result = await handler.Handle(
            new InitialDataCommand(Actor.System("system")), CancellationToken.None);

        result.Should().BeFalse();
    }
}
