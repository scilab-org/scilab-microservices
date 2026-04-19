namespace Lab.Domain.Tests.Exceptions;

public sealed class DomainExceptionTests
{
    [Fact]
    public void Constructor_ShouldSetMessage()
    {
        var ex = new DomainException("test error");
        ex.Message.Should().Be("test error");
    }

    [Fact]
    public void ShouldBeException()
    {
        var ex = new DomainException("err");
        ex.Should().BeAssignableTo<Exception>();
    }
}
