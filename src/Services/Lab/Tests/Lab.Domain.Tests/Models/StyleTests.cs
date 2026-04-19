namespace Lab.Domain.Tests.Models;

public sealed class StyleTests
{
    [Fact]
    public void Style_ShouldSetAndGetProperties()
    {
        var style = new Style
        {
            Name = "IEEE",
            Description = "IEEE Format",
            Rule = "Follow IEEE guidelines"
        };

        style.Name.Should().Be("IEEE");
        style.Description.Should().Be("IEEE Format");
        style.Rule.Should().Be("Follow IEEE guidelines");
    }
}
