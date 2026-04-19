using Common.Models;
using Lab.Application.Dtos.Journals;
using Lab.Application.Dtos.Papers;
using Lab.Application.Features.Journal.Commands.DeleteJournal;
using Lab.Application.Features.Journal.Queries.GetJournalById;
using Lab.Application.Features.Paper.Commands.CombineSectionsToPaper;
using Lab.Application.Features.Paper.Commands.CreatePaperVersionFile;
using Lab.Application.Features.Paper.Commands.DeletePaper;
using Lab.Application.Features.Paper.Commands.UpdatePaper;

namespace Lab.Application.Tests.Validators;

#region Journal Validators

public sealed class DeleteJournalCommandValidatorTests
{
    private readonly DeleteJournalCommandValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenIdIsEmpty()
    {
        var command = new DeleteJournalCommand(Guid.Empty, "user");

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldNotHaveError_WhenIdIsProvided()
    {
        var command = new DeleteJournalCommand(Guid.NewGuid(), "user");

        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}

public sealed class GetJournalByIdQueryValidatorTests
{
    private readonly GetJournalByIdQueryValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenIdIsEmpty()
    {
        var query = new GetJournalByIdQuery(Guid.Empty);

        _validator.TestValidate(query).ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldNotHaveError_WhenIdIsProvided()
    {
        var query = new GetJournalByIdQuery(Guid.NewGuid());

        _validator.TestValidate(query).ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}

public sealed class GetJournalInProjectByIdQueryValidatorTests
{
    private readonly GetJournalInProjectByIdQueryValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenIdIsEmpty()
    {
        var query = new GetJournalInProjectByIdQuery(Guid.Empty, Guid.NewGuid());

        _validator.TestValidate(query).ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldHaveError_WhenProjectIdIsEmpty()
    {
        var query = new GetJournalInProjectByIdQuery(Guid.NewGuid(), Guid.Empty);

        _validator.TestValidate(query).ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public void ShouldNotHaveError_WhenIdsAreProvided()
    {
        var query = new GetJournalInProjectByIdQuery(Guid.NewGuid(), Guid.NewGuid());

        _validator.TestValidate(query).ShouldNotHaveAnyValidationErrors();
    }
}

#endregion

#region Paper Validators

public sealed class CombineSectionsToPaperCommandValidatorTests
{
    private readonly CombineSectionsToPaperCommandValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenPaperIdIsEmpty()
    {
        var command = new CombineSectionsToPaperCommand(Guid.Empty, new CreatePaperCombineDto { ProjectId = Guid.NewGuid() }, "user");

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.PaperId);
    }

    [Fact]
    public void ShouldHaveError_WhenProjectIdIsEmpty()
    {
        var command = new CombineSectionsToPaperCommand(Guid.NewGuid(), new CreatePaperCombineDto { ProjectId = Guid.Empty }, "user");

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto.ProjectId);
    }

    [Fact]
    public void ShouldNotHaveError_WhenValid()
    {
        var command = new CombineSectionsToPaperCommand(Guid.NewGuid(), new CreatePaperCombineDto { ProjectId = Guid.NewGuid() }, "user");

        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }
}

public sealed class CreatePaperVersionFileCommandValidatorTests
{
    private readonly CreatePaperVersionFileCommandValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenPaperIdIsEmpty()
    {
        var command = CreateCommand(paperId: Guid.Empty);

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.PaperId);
    }

    [Fact]
    public void ShouldHaveError_WhenVersionIdIsEmpty()
    {
        var command = CreateCommand(versionId: Guid.Empty);

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.VersionId);
    }

    [Fact]
    public void ShouldHaveError_WhenDtoIsNull()
    {
        var command = new CreatePaperVersionFileCommand(Guid.NewGuid(), Guid.NewGuid(), null!, "user");

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto);
    }

    [Fact]
    public void ShouldHaveError_WhenUploadFileIsNull()
    {
        var command = CreateCommand(dto: new CreatePaperVersionFileDto { UploadFile = null!, Note = "note" });

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto.UploadFile);
    }

    [Fact]
    public void ShouldNotHaveError_WhenValid()
    {
        var command = CreateCommand();

        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }

    private static CreatePaperVersionFileCommand CreateCommand(
        Guid? paperId = null,
        Guid? versionId = null,
        CreatePaperVersionFileDto? dto = null)
    {
        return new CreatePaperVersionFileCommand(
            paperId ?? Guid.NewGuid(),
            versionId ?? Guid.NewGuid(),
            dto ?? new CreatePaperVersionFileDto
            {
                UploadFile = new UploadFileBytes
                {
                    FileName = "version.pdf",
                    ContentType = "application/pdf",
                    Bytes = new byte[] { 1 }
                },
                Note = "note"
            },
            "user");
    }
}

public sealed class DeletePaperCommandValidatorTests
{
    private readonly DeletePaperCommandValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenIdIsEmpty()
    {
        var command = new DeletePaperCommand(Guid.Empty);

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldNotHaveError_WhenIdIsProvided()
    {
        var command = new DeletePaperCommand(Guid.NewGuid());

        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}

public sealed class UpdatePaperCommandValidatorTests
{
    private readonly UpdatePaperCommandValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenDtoIsNull()
    {
        var command = new UpdatePaperCommand(null!, Guid.NewGuid(), "user");

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto);
    }

    [Fact]
    public void ShouldHaveError_WhenContextIsEmpty()
    {
        var dto = CreateDto(context: string.Empty);
        var command = new UpdatePaperCommand(dto, Guid.NewGuid(), "user");

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto.Context);
    }

    [Fact]
    public void ShouldHaveError_WhenJournalNameIsEmpty()
    {
        var dto = CreateDto(conferenceJournalName: string.Empty);
        var command = new UpdatePaperCommand(dto, Guid.NewGuid(), "user");

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto.ConferenceJournalName);
    }

    [Fact]
    public void ShouldNotHaveError_WhenValid()
    {
        var command = new UpdatePaperCommand(CreateValidDto(), Guid.NewGuid(), "user");

        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }

    private static UpdatePaperDto CreateValidDto() => CreateDto();

    private static UpdatePaperDto CreateDto(
        string context = "context",
        string conferenceJournalName = "ICSE") => new()
    {
        Context = context,
        Abstract = "abstract",
        ResearchGap = "gap",
        GapType = "type",
        MainContribution = "contribution",
        ResearchAim = "aim",
        ConferenceJournalName = conferenceJournalName,
        ConferenceJournalId = Guid.NewGuid(),
        ConferenceJournalStartAt = DateTimeOffset.UtcNow,
        ConferenceJournalEndAt = DateTimeOffset.UtcNow.AddDays(1)
    };
}

#endregion
