namespace Management.Domain.Tests.Exceptions;

public sealed class DomainExceptionTests
{
    [Fact]
    public void Constructor_ShouldSetMessage()
    {
        // Arrange
        const string message = "Something went wrong in the domain";

        // Act
        var exception = new DomainException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void DomainException_ShouldInheritFromException()
    {
        // Arrange & Act
        var exception = new DomainException("test");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }
}
