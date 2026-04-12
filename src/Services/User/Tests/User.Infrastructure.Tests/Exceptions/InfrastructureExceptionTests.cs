namespace User.Infrastructure.Tests.Exceptions;

public sealed class InfrastructureExceptionTests
{
    [Fact]
    public void Constructor_ShouldSetMessage()
    {
        // Arrange
        const string message = "Something went wrong in infrastructure";

        // Act
        var exception = new InfrastructureException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void InfrastructureException_ShouldInheritFromException()
    {
        // Arrange & Act
        var exception = new InfrastructureException("test");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }
}
