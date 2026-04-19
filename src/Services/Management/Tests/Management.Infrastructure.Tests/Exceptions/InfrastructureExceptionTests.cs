using Management.Infrastructure.Exceptions;

namespace Management.Infrastructure.Tests.Exceptions;

public sealed class InfrastructureExceptionTests
{
    [Fact]
    public void Constructor_ShouldSetMessage_WhenMessageIsProvided()
    {
        // Arrange
        const string message = "Infrastructure error";

        // Act
        var exception = new InfrastructureException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void Constructor_ShouldBeException_WhenCreated()
    {
        // Arrange & Act
        var exception = new InfrastructureException("test");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }
}
