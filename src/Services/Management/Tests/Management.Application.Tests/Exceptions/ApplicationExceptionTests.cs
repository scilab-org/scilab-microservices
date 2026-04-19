using AppException = Management.Application.Exceptions.ApplicationException;

namespace Management.Application.Tests.Exceptions;

public sealed class ApplicationExceptionTests
{
    [Fact]
    public void Constructor_ShouldSetMessage()
    {
        // Arrange
        const string message = "Something went wrong in the application";

        // Act
        var exception = new AppException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void ApplicationException_ShouldInheritFromException()
    {
        // Arrange & Act
        var exception = new AppException("test");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }
}
