using User.Application.Tests.Common;
using AppException = User.Application.Exceptions.ApplicationException;

namespace User.Application.Tests.Exceptions;

public sealed class ApplicationExceptionTests : BaseTest
{
    [Fact]
    public void Constructor_ShouldSetMessage_WhenCreated()
    {
        // Arrange
        const string message = "Application error occurred";

        // Act
        var exception = new AppException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void Constructor_ShouldBeAssignableFromException()
    {
        // Arrange & Act
        var exception = new AppException("error");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }
}
