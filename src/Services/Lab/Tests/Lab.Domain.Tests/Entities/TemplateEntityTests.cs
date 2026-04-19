namespace Lab.Domain.Tests.Entities;

public sealed class TemplateEntityTests
{
    [Fact]
    public void Create_ShouldInitializeCorrectly()
    {
        var sections = new List<Section>
        {
            new() { Title = "Introduction", DisplayOrder = 1 }
        };

        var entity = TemplateEntity.Create("IMRAD", "Standard template", sections, "admin");

        entity.Id.Should().NotBe(Guid.Empty);
        entity.Code.Should().Be("IMRAD");
        entity.Description.Should().Be("Standard template");
        entity.Sections.Should().HaveCount(1);
        entity.CreatedBy.Should().Be("admin");
        entity.CreatedOnUtc.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Create_ShouldUseDefaults()
    {
        var entity = TemplateEntity.Create(null, null, null);
        entity.Code.Should().BeNull();
        entity.Description.Should().BeNull();
        entity.Sections.Should().BeNull();
        entity.CreatedBy.Should().BeNull();
    }

    [Fact]
    public void Update_ShouldUpdateProvidedValues()
    {
        var entity = TemplateEntity.Create("OLD", "old desc", null);
        var newSections = new List<Section>
        {
            new() { Title = "Methods", DisplayOrder = 2 }
        };

        entity.Update(code: "NEW", description: "new desc",
            sections: newSections, lastModifiedBy: "editor");

        entity.Code.Should().Be("NEW");
        entity.Description.Should().Be("new desc");
        entity.Sections.Should().HaveCount(1);
        entity.LastModifiedBy.Should().Be("editor");
    }

    [Fact]
    public void Update_ShouldKeepExisting_WhenNullsPassed()
    {
        var entity = TemplateEntity.Create("CODE", "desc", null);
        entity.Update();
        entity.Code.Should().Be("CODE");
        entity.Description.Should().Be("desc");
    }
}
