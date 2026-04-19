using Common.Models;
using Lab.Application.Dtos.Comments;
using Lab.Application.Dtos.Journals;
using Lab.Application.Dtos.PaperBanks;
using Lab.Application.Dtos.PaperContributors;
using Lab.Application.Dtos.Papers;
using Lab.Application.Dtos.Projects;
using Lab.Application.Dtos.Sections;
using Lab.Application.Dtos.Template;
using Lab.Application.Features.Comment.Commands.CreateComment;
using Lab.Application.Features.Journal.Commands.CreateJournal;
using Lab.Application.Features.Journal.Commands.UpdateJournal;
using Lab.Application.Features.Paper.Commands.CreatePaper;
using Lab.Application.Features.Paper.Commands.TransitionPaperStatus;
using Lab.Application.Features.PaperBank.Commands.CreatePaperBank;
using Lab.Application.Features.PaperContributor.Commands.CreatePaperContributor;
using Lab.Application.Features.Section.Commands.UpsertSection;
using Lab.Application.Features.System;
using Lab.Application.Features.Tag.Commands.CreateTag;
using Lab.Application.Features.Tag.Commands.DeleteTag;
using Lab.Application.Features.Tag.Commands.UpdateTag;
using Lab.Application.Features.Template.Commands;

namespace Lab.Application.Tests.Validators;

#region Comment Validators

public sealed class CreateCommentCommandValidatorTests
{
    private readonly CreateCommentCommandValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenContentIsEmpty()
    {
        var command = new CreateCommentCommand("user", new CreateCommentDto { Content = "", SectionId = Guid.NewGuid() });
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto.Content);
    }

    [Fact]
    public void ShouldNotHaveError_WhenContentIsProvided()
    {
        var command = new CreateCommentCommand("user", new CreateCommentDto { Content = "Good work", SectionId = Guid.NewGuid() });
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Dto.Content);
    }
}

#endregion

#region Tag Validators

public sealed class CreateTagCommandValidatorTests
{
    private readonly CreateTagCommandValidator _validator = new();

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ShouldHaveError_WhenNameIsEmpty(string? name)
    {
        var command = new CreateTagCommand(name!);
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void ShouldNotHaveError_WhenNameIsProvided()
    {
        var command = new CreateTagCommand("ML");
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}

public sealed class UpdateTagCommandValidatorTests
{
    private readonly UpdateTagCommandValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenIdIsEmpty()
    {
        var command = new UpdateTagCommand(Guid.Empty, "name");
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldNotHaveError_WhenIdIsProvided()
    {
        var command = new UpdateTagCommand(Guid.NewGuid(), "name");
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}

public sealed class DeleteTagCommandValidatorTests
{
    private readonly DeleteTagCommandValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenIdIsEmpty()
    {
        var command = new DeleteTagCommand(Guid.Empty);
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldNotHaveError_WhenIdIsProvided()
    {
        var command = new DeleteTagCommand(Guid.NewGuid());
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}

#endregion

#region Template Validators

public sealed class CreateTemplateCommandValidatorTests
{
    private readonly CreateTemplateCommandValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenDtoIsNull()
    {
        var command = new CreateTemplateCommand(null!);
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto);
    }

    [Fact]
    public void ShouldHaveError_WhenCodeIsEmpty()
    {
        var dto = new CreateTemplateDto
        {
            Code = "",
            Sections = new List<Section> { new() { Title = "T", SectionRule = "R", DisplayOrder = 1 } }
        };
        var command = new CreateTemplateCommand(dto);
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto.Code);
    }

    [Fact]
    public void ShouldHaveError_WhenSectionsAreNull()
    {
        var dto = new CreateTemplateDto { Code = "CODE", Sections = null };
        var command = new CreateTemplateCommand(dto);
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto.Sections);
    }

    [Fact]
    public void ShouldHaveError_WhenSectionsAreEmpty()
    {
        var dto = new CreateTemplateDto { Code = "CODE", Sections = new List<Section>() };
        var command = new CreateTemplateCommand(dto);
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto.Sections);
    }

    [Fact]
    public void ShouldHaveError_WhenSectionTitleIsEmpty()
    {
        var dto = new CreateTemplateDto
        {
            Code = "CODE",
            Sections = new List<Section> { new() { Title = "", SectionRule = "R", DisplayOrder = 1 } }
        };
        var command = new CreateTemplateCommand(dto);
        var result = _validator.TestValidate(command);
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public void ShouldHaveError_WhenSectionRuleIsEmpty()
    {
        var dto = new CreateTemplateDto
        {
            Code = "CODE",
            Sections = new List<Section> { new() { Title = "T", SectionRule = "", DisplayOrder = 1 } }
        };
        var command = new CreateTemplateCommand(dto);
        var result = _validator.TestValidate(command);
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public void ShouldNotHaveError_WhenValid()
    {
        var dto = new CreateTemplateDto
        {
            Code = "IMRAD",
            Description = "Standard",
            Sections = new List<Section> { new() { Title = "Intro", SectionRule = "Rule1", DisplayOrder = 1 } }
        };
        var command = new CreateTemplateCommand(dto);
        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }
}

#endregion

#region Journal Validators

public sealed class CreateJournalCommandValidatorTests
{
    private readonly CreateJournalCommandValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenDtoIsNull()
    {
        var command = new CreateJournalCommand(null!, "user");
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto);
    }

    [Fact]
    public void ShouldHaveError_WhenNameIsEmpty()
    {
        var dto = new CreateJournalEntityDto
        {
            Name = "", Ranking = "A", Url = "https://test.com",
            TemplateId = Guid.NewGuid(), Style = "IEEE"
        };
        var command = new CreateJournalCommand(dto, "user");
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto.Name);
    }

    [Fact]
    public void ShouldHaveError_WhenTexFileHasWrongExtension()
    {
        var dto = new CreateJournalEntityDto
        {
            Name = "J", Ranking = "A", Url = "https://test.com",
            TemplateId = Guid.NewGuid(), Style = "IEEE",
            TexUploadFile = new UploadFileBytes { FileName = "file.pdf", ContentType = "app/pdf", Bytes = new byte[1] }
        };
        var command = new CreateJournalCommand(dto, "user");
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto.TexUploadFile);
    }

    [Fact]
    public void ShouldHaveError_WhenPdfFileHasWrongExtension()
    {
        var dto = new CreateJournalEntityDto
        {
            Name = "J", Ranking = "A", Url = "https://test.com",
            TemplateId = Guid.NewGuid(), Style = "IEEE",
            PdfUploadFile = new UploadFileBytes { FileName = "file.tex", ContentType = "app/tex", Bytes = new byte[1] }
        };
        var command = new CreateJournalCommand(dto, "user");
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto.PdfUploadFile);
    }

    [Fact]
    public void ShouldNotHaveError_WhenTexFileIsNull()
    {
        var dto = new CreateJournalEntityDto
        {
            Name = "J", Ranking = "A", Url = "https://test.com",
            TemplateId = Guid.NewGuid(), Style = "IEEE"
        };
        var command = new CreateJournalCommand(dto, "user");
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Dto.TexUploadFile);
    }

    [Fact]
    public void ShouldNotHaveError_WhenTexFileHasCorrectExtension()
    {
        var dto = new CreateJournalEntityDto
        {
            Name = "J", Ranking = "A", Url = "https://test.com",
            TemplateId = Guid.NewGuid(), Style = "IEEE",
            TexUploadFile = new UploadFileBytes { FileName = "template.tex", ContentType = "text/plain", Bytes = new byte[1] }
        };
        var command = new CreateJournalCommand(dto, "user");
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Dto.TexUploadFile);
    }

    [Fact]
    public void ShouldNotHaveError_WhenValid()
    {
        var dto = new CreateJournalEntityDto
        {
            Name = "ICSE", Ranking = "A*", Url = "https://icse.org",
            TemplateId = Guid.NewGuid(), Style = "IEEE"
        };
        var command = new CreateJournalCommand(dto, "user");
        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }
}

public sealed class UpdateJournalCommandValidatorTests
{
    private readonly UpdateJournalCommandValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenDtoIsNull()
    {
        var command = new UpdateJournalCommand(null!, Guid.NewGuid(), "user");
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto);
    }

    [Fact]
    public void ShouldHaveError_WhenIdIsEmpty()
    {
        var dto = new UpdateJournalEntityDto { Name = "J", Ranking = "A", Url = "https://test.com" };
        var command = new UpdateJournalCommand(dto, Guid.Empty, "user");
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldNotHaveError_WhenValid()
    {
        var dto = new UpdateJournalEntityDto { Name = "ICSE", Ranking = "A*", Url = "https://icse.org" };
        var command = new UpdateJournalCommand(dto, Guid.NewGuid(), "user");
        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }
}

#endregion

#region Paper Validators

public sealed class CreatePaperCommandValidatorTests
{
    private readonly CreatePaperCommandValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenDtoIsNull()
    {
        var command = new CreatePaperCommand(null!, Guid.NewGuid(), "user");
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto);
    }

    [Fact]
    public void ShouldHaveError_WhenTitleIsEmpty()
    {
        var dto = CreateValidPaperDto();
        var emptyTitleDto = new CreatePaperDto
        {
            ProjectId = dto.ProjectId, Title = "", Template = dto.Template,
            Context = dto.Context, Abstract = dto.Abstract,
            ResearchGap = dto.ResearchGap, GapType = dto.GapType,
            MainContribution = dto.MainContribution, ResearchAim = dto.ResearchAim,
            ConferenceJournalId = dto.ConferenceJournalId,
            ConferenceJournalName = dto.ConferenceJournalName,
            ConferenceJournalStartAt = dto.ConferenceJournalStartAt,
            ConferenceJournalEndAt = dto.ConferenceJournalEndAt
        };
        var command = new CreatePaperCommand(emptyTitleDto, Guid.NewGuid(), "user");
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto.Title);
    }

    [Fact]
    public void ShouldNotHaveError_WhenValid()
    {
        var dto = CreateValidPaperDto();
        var command = new CreatePaperCommand(dto, Guid.NewGuid(), "user");
        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }

    private static CreatePaperDto CreateValidPaperDto() => new()
    {
        ProjectId = Guid.NewGuid(),
        Title = "Title",
        Template = "IMRAD",
        Context = "Context",
        Abstract = "Abstract",
        ResearchGap = "Gap",
        GapType = "Type",
        MainContribution = "Contribution",
        ResearchAim = "Aim",
        ConferenceJournalId = Guid.NewGuid(),
        ConferenceJournalName = "Journal",
        ConferenceJournalStartAt = DateTimeOffset.UtcNow,
        ConferenceJournalEndAt = DateTimeOffset.UtcNow.AddDays(3)
    };
}

public sealed class TransitionPaperStatusCommandValidatorTests
{
    private readonly TransitionPaperStatusCommandValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenPaperIdIsEmpty()
    {
        var dto = new TransitionPaperStatusDto { ProjectId = Guid.NewGuid() };
        var command = new TransitionPaperStatusCommand(Guid.Empty, dto, Guid.NewGuid(), "user");
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.PaperId);
    }

    [Fact]
    public void ShouldHaveError_WhenDtoIsNull()
    {
        var command = new TransitionPaperStatusCommand(Guid.NewGuid(), null!, Guid.NewGuid(), "user");
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto);
    }

    [Fact]
    public void ShouldHaveError_WhenProjectIdIsEmpty()
    {
        var dto = new TransitionPaperStatusDto { ProjectId = Guid.Empty };
        var command = new TransitionPaperStatusCommand(Guid.NewGuid(), dto, Guid.NewGuid(), "user");
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto.ProjectId);
    }

    [Fact]
    public void ShouldNotHaveError_WhenValid()
    {
        var dto = new TransitionPaperStatusDto { ProjectId = Guid.NewGuid() };
        var command = new TransitionPaperStatusCommand(Guid.NewGuid(), dto, Guid.NewGuid(), "user");
        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }
}

#endregion

#region PaperBank Validators

public sealed class CreatePaperBankCommandValidatorTests
{
    private readonly CreatePaperBankCommandValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenDtoIsNull()
    {
        var command = new CreatePaperBankCommand(null!);
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto);
    }

    [Fact]
    public void ShouldHaveError_WhenTitleIsEmpty()
    {
        var dto = new CreatePaperBankDto
        {
            Title = "",
            UploadFile = new UploadFileBytes { FileName = "f.pdf", ContentType = "app/pdf", Bytes = new byte[1] }
        };
        var command = new CreatePaperBankCommand(dto);
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto.Title);
    }

    [Fact]
    public void ShouldHaveError_WhenUploadFileIsNull()
    {
        var dto = new CreatePaperBankDto { Title = "Title", UploadFile = null! };
        var command = new CreatePaperBankCommand(dto);
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto.UploadFile);
    }

    [Fact]
    public void ShouldHaveError_WhenPublicationDateIsInFuture()
    {
        var dto = new CreatePaperBankDto
        {
            Title = "Title",
            UploadFile = new UploadFileBytes { FileName = "f.pdf", ContentType = "app/pdf", Bytes = new byte[1] },
            PublicationDate = DateTimeOffset.UtcNow.AddDays(10)
        };
        var command = new CreatePaperBankCommand(dto);
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto.PublicationDate);
    }

    [Fact]
    public void ShouldNotHaveError_WhenPublicationDateIsNull()
    {
        var dto = new CreatePaperBankDto
        {
            Title = "Title",
            UploadFile = new UploadFileBytes { FileName = "f.pdf", ContentType = "app/pdf", Bytes = new byte[1] },
            PublicationDate = null
        };
        var command = new CreatePaperBankCommand(dto);
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Dto.PublicationDate);
    }

    [Fact]
    public void ShouldNotHaveError_WhenValid()
    {
        var dto = new CreatePaperBankDto
        {
            Title = "Paper Title",
            UploadFile = new UploadFileBytes { FileName = "f.pdf", ContentType = "app/pdf", Bytes = new byte[1] },
            PublicationDate = DateTimeOffset.UtcNow.AddDays(-1)
        };
        var command = new CreatePaperBankCommand(dto);
        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }
}

#endregion

#region PaperContributor Validators

public sealed class CreatePaperContributorCommandValidatorTests
{
    private readonly CreatePaperContributorCommandValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenSectionRoleIsNull()
    {
        var dto = new CreatePaperContributorDto
        {
            SectionRole = null!, PaperId = Guid.NewGuid(),
            MemberIds = new List<Guid> { Guid.NewGuid() }, MarkSectionId = Guid.NewGuid()
        };
        var command = new CreatePaperContributorCommand(dto);
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto.SectionRole);
    }

    [Fact]
    public void ShouldHaveError_WhenPaperIdIsEmpty()
    {
        var dto = new CreatePaperContributorDto
        {
            SectionRole = "Author", PaperId = Guid.Empty,
            MemberIds = new List<Guid> { Guid.NewGuid() }, MarkSectionId = Guid.NewGuid()
        };
        var command = new CreatePaperContributorCommand(dto);
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto.PaperId);
    }

    [Fact]
    public void ShouldHaveError_WhenMemberIdsIsEmpty()
    {
        var dto = new CreatePaperContributorDto
        {
            SectionRole = "Author", PaperId = Guid.NewGuid(),
            MemberIds = new List<Guid>(), MarkSectionId = Guid.NewGuid()
        };
        var command = new CreatePaperContributorCommand(dto);
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto.MemberIds);
    }

    [Fact]
    public void ShouldNotHaveError_WhenValid()
    {
        var dto = new CreatePaperContributorDto
        {
            SectionRole = "Author", PaperId = Guid.NewGuid(),
            MemberIds = new List<Guid> { Guid.NewGuid() }, MarkSectionId = Guid.NewGuid()
        };
        var command = new CreatePaperContributorCommand(dto);
        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }
}

#endregion

#region Section Validators

public sealed class UpsertSectionCommandValidatorTests
{
    private readonly UpsertSectionCommandValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenDtoIsNull()
    {
        var command = new UpsertSectionCommand(null!, Guid.NewGuid(), "user");
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto);
    }

    [Fact]
    public void ShouldHaveError_WhenMemberIdIsEmpty()
    {
        var dto = new UpsertSectionDto { MemberId = Guid.Empty };
        var command = new UpsertSectionCommand(dto, Guid.NewGuid(), "user");
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto.MemberId);
    }

    [Fact]
    public void ShouldHaveError_WhenIdIsEmpty()
    {
        var dto = new UpsertSectionDto { MemberId = Guid.NewGuid() };
        var command = new UpsertSectionCommand(dto, Guid.Empty, "user");
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldNotHaveError_WhenValid()
    {
        var dto = new UpsertSectionDto { MemberId = Guid.NewGuid() };
        var command = new UpsertSectionCommand(dto, Guid.NewGuid(), "user");
        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }
}

#endregion

#region System Validators

public sealed class UpdateProjectRulesCommandValidatorTests
{
    private readonly UpdateProjectRulesCommandValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenDtoIsNull()
    {
        var command = new UpdateProjectRulesCommand(null!);
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto);
    }

    [Fact]
    public void ShouldNotHaveError_WhenValid()
    {
        var dto = new UpdateProjectRulesDto { PaperIds = new List<Guid> { Guid.NewGuid() } };
        var command = new UpdateProjectRulesCommand(dto);
        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }
}

#endregion
