using Lab.Application.Dtos.Template;
using Lab.Application.Features.Template.Commands;
using Lab.Application.Features.Template.Queries.GetTemplateById;
using Lab.Application.Features.Template.Queries.GetTemplates;
using Lab.Application.Features.Template.Queries.GetTemplatesByCode;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Lab.Application.Tests.Common;
using Lab.Domain.Entities;
using Lab.Domain.Models;

namespace Lab.Application.Tests.Features.Template;

public class TemplateIntegrationTests : MartenTestBase
{
    protected override string SchemaName => "template_tests";

    private static CreateTemplateDto MakeDto(string code, string? desc = null) => new()
    {
        Code = code,
        Description = desc ?? $"Description for {code}",
        Sections = new List<Lab.Domain.Models.Section>
        {
            new() { Title = "Introduction", SectionRule = "Write intro", DisplayOrder = 1 },
            new() { Title = "Methodology", SectionRule = "Describe method", DisplayOrder = 2 }
        }
    };

    [Fact]
    public async Task CreateTemplate_WithValidData_ShouldStoreAndReturnId()
    {
        var handler = new CreatePaperTemplateCommandHandler(Session);
        var result = await handler.Handle(new CreateTemplateCommand(MakeDto("TPL-001")), CancellationToken.None);

        result.Should().NotBeEmpty();
        var stored = await Session.LoadAsync<TemplateEntity>(result);
        stored.Should().NotBeNull();
        stored!.Code.Should().Be("TPL-001");
        stored.Sections.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateTemplate_WithDuplicateCode_ShouldThrowClientValidationException()
    {
        var handler = new CreatePaperTemplateCommandHandler(Session);
        await handler.Handle(new CreateTemplateCommand(MakeDto("TPL-DUP")), CancellationToken.None);

        var act = () => handler.Handle(new CreateTemplateCommand(MakeDto("TPL-DUP")), CancellationToken.None);
        await act.Should().ThrowAsync<ClientValidationException>();
    }

    [Fact]
    public async Task UpdateTemplate_WithValidData_ShouldUpdateFields()
    {
        var createHandler = new CreatePaperTemplateCommandHandler(Session);
        var id = await createHandler.Handle(new CreateTemplateCommand(MakeDto("TPL-UPD")), CancellationToken.None);

        var newSections = new List<Lab.Domain.Models.Section> { new() { Title = "Updated Section", SectionRule = "New rule", DisplayOrder = 1 } };
        var updateHandler = new UpdateTemplateCommandCommandHandler(Session);
        var result = await updateHandler.Handle(
            new UpdateTemplateCommand(id, new CreateTemplateVersionDto { Description = "Updated desc", Sections = newSections }),
            CancellationToken.None);

        result.Should().Be(id);
        var updated = await Session.LoadAsync<TemplateEntity>(id);
        updated!.Description.Should().Be("Updated desc");
        updated.Sections.Should().HaveCount(1);
        updated.Code.Should().Be("TPL-UPD"); // Code should not change
    }

    [Fact]
    public async Task UpdateTemplate_WithNullFields_ShouldKeepExisting()
    {
        var createHandler = new CreatePaperTemplateCommandHandler(Session);
        var id = await createHandler.Handle(new CreateTemplateCommand(MakeDto("TPL-KEEP", "Original")), CancellationToken.None);

        var updateHandler = new UpdateTemplateCommandCommandHandler(Session);
        await updateHandler.Handle(
            new UpdateTemplateCommand(id, new CreateTemplateVersionDto { Description = null, Sections = null }),
            CancellationToken.None);

        var updated = await Session.LoadAsync<TemplateEntity>(id);
        updated!.Description.Should().Be("Original");
        updated.Sections.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateTemplate_WithNonExistentId_ShouldThrowNotFoundException()
    {
        var handler = new UpdateTemplateCommandCommandHandler(Session);
        var act = () => handler.Handle(
            new UpdateTemplateCommand(Guid.NewGuid(), new CreateTemplateVersionDto { Description = "any" }),
            CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteTemplate_WithExistingEntity_ShouldRemoveFromStore()
    {
        var createHandler = new CreatePaperTemplateCommandHandler(Session);
        var id = await createHandler.Handle(new CreateTemplateCommand(MakeDto("TPL-DEL")), CancellationToken.None);

        var deleteHandler = new DeleteTemplateCommandHandler(Session);
        await deleteHandler.Handle(new DeleteTemplateCommand(id), CancellationToken.None);

        var deleted = await Session.LoadAsync<TemplateEntity>(id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeleteTemplate_WithNonExistentId_ShouldThrowNotFoundException()
    {
        var handler = new DeleteTemplateCommandHandler(Session);
        var act = () => handler.Handle(new DeleteTemplateCommand(Guid.NewGuid()), CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetTemplateById_WithExistingEntity_ShouldReturnMappedResult()
    {
        var createHandler = new CreatePaperTemplateCommandHandler(Session);
        var id = await createHandler.Handle(new CreateTemplateCommand(MakeDto("TPL-QRY")), CancellationToken.None);

        var queryHandler = new GetTemplateByIdQueryHandler(Session, Mapper);
        var result = await queryHandler.Handle(new GetTemplateByIdQuery(id), CancellationToken.None);

        result.Template.Should().NotBeNull();
        result.Template.Code.Should().Be("TPL-QRY");
    }

    [Fact]
    public async Task GetTemplateById_WithNonExistentId_ShouldThrowNotFoundException()
    {
        var handler = new GetTemplateByIdQueryHandler(Session, Mapper);
        var act = () => handler.Handle(new GetTemplateByIdQuery(Guid.NewGuid()), CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetTemplates_WithNoFilter_ShouldReturnAll()
    {
        var createHandler = new CreatePaperTemplateCommandHandler(Session);
        await createHandler.Handle(new CreateTemplateCommand(MakeDto("TPL-A")), CancellationToken.None);
        await createHandler.Handle(new CreateTemplateCommand(MakeDto("TPL-B")), CancellationToken.None);

        var queryHandler = new GetTemplatesQueryHandler(Session, Mapper);
        var result = await queryHandler.Handle(
            new GetTemplatesQuery(new GetTemplatesFilter(), new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetTemplates_WithCodeFilter_ShouldReturnMatchingTemplates()
    {
        var createHandler = new CreatePaperTemplateCommandHandler(Session);
        await createHandler.Handle(new CreateTemplateCommand(MakeDto("RESEARCH-001")), CancellationToken.None);
        await createHandler.Handle(new CreateTemplateCommand(MakeDto("RESEARCH-002")), CancellationToken.None);
        await createHandler.Handle(new CreateTemplateCommand(MakeDto("SURVEY-001")), CancellationToken.None);

        var queryHandler = new GetTemplatesQueryHandler(Session, Mapper);
        var result = await queryHandler.Handle(
            new GetTemplatesQuery(new GetTemplatesFilter { Code = "RESEARCH" }, new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetTemplates_WithDescriptionFilter_ShouldReturnMatchingTemplates()
    {
        var createHandler = new CreatePaperTemplateCommandHandler(Session);
        await createHandler.Handle(new CreateTemplateCommand(MakeDto("DF-A", "Machine Learning template")), CancellationToken.None);
        await createHandler.Handle(new CreateTemplateCommand(MakeDto("DF-B", "Deep Learning template")), CancellationToken.None);
        await createHandler.Handle(new CreateTemplateCommand(MakeDto("DF-C", "Data Mining template")), CancellationToken.None);

        var queryHandler = new GetTemplatesQueryHandler(Session, Mapper);
        var result = await queryHandler.Handle(
            new GetTemplatesQuery(new GetTemplatesFilter { Description = "Learning" }, new PaginationRequest(1, 10)),
            CancellationToken.None);

        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetTemplatesByCode_WithExistingCode_ShouldReturnTemplate()
    {
        var createHandler = new CreatePaperTemplateCommandHandler(Session);
        await createHandler.Handle(new CreateTemplateCommand(MakeDto("EXACT-CODE")), CancellationToken.None);

        var queryHandler = new GetTemplatesByCodeQueryHandler(Session, Mapper);
        var result = await queryHandler.Handle(new GetTemplatesByCodeQuery("EXACT-CODE"), CancellationToken.None);

        result.Should().NotBeNull();
        result.Code.Should().Be("EXACT-CODE");
    }

    [Fact]
    public async Task GetTemplatesByCode_WithNonExistentCode_ShouldThrowNotFoundException()
    {
        var handler = new GetTemplatesByCodeQueryHandler(Session, Mapper);
        var act = () => handler.Handle(new GetTemplatesByCodeQuery("NON-EXISTENT"), CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
