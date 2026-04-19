namespace Lab.Domain.Tests.Constants;

public sealed class SectionConstantsTests
{
    [Fact]
    public void ReferencesTitle_ShouldBeReferences()
    {
        SectionConstants.ReferencesTitle.Should().Be("References");
    }

    [Fact]
    public void ReferenceTitle_ShouldBeReference()
    {
        SectionConstants.ReferenceTitle.Should().Be("Reference");
    }

    [Theory]
    [InlineData("References", true)]
    [InlineData("references", true)]
    [InlineData("REFERENCES", true)]
    [InlineData("Reference", true)]
    [InlineData("reference", true)]
    [InlineData("REFERENCE", true)]
    [InlineData("Introduction", false)]
    [InlineData("Ref", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsReferenceSection_ShouldReturnExpectedResult(string? title, bool expected)
    {
        SectionConstants.IsReferenceSection(title).Should().Be(expected);
    }
}
