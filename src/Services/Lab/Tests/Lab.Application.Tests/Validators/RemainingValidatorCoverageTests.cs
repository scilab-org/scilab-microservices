using Common.Models;
using Lab.Application.Dtos.PaperBanks;
using Lab.Application.Dtos.PaperContributors;
using Lab.Application.Dtos.Papers;
using Lab.Application.Dtos.Sections;
using Lab.Application.Dtos.Tasks;
using Lab.Application.Features.Paper.Commands.UpdateCombinePaper;
using Lab.Application.Features.Paper.Queries.GetCombinePaperById;
using Lab.Application.Features.Paper.Queries.GetPaperById;
using Lab.Application.Features.Paper.Queries.GetPaperStatusHistory;
using Lab.Application.Features.Paper.Queries.GetPaperVersionFileById;
using Lab.Application.Features.Paper.Queries.GetPaperVersionFiles;
using Lab.Application.Features.Paper.Queries.GetSectionsByPaperId;
using Lab.Application.Features.Paper.Queries.GetVersionsByPaperId;
using Lab.Application.Features.PaperBank.Commands.DeletePaperBank;
using Lab.Application.Features.PaperBank.Commands.UpdatePaperBank;
using Lab.Application.Features.PaperBank.Commands.UpdatePaperBankIngestionStatus;
using Lab.Application.Features.PaperBank.Queries.GetPaperBankById;
using Lab.Application.Features.PaperContributor.Commands.UpdatePaperContributor;
using Lab.Application.Features.PaperContributor.Queries.GetAssignedPaperSections;
using Lab.Application.Features.PaperContributor.Queries.GetAssignedPaperSectionsHistory;
using Lab.Application.Features.PaperContributor.Queries.GetAvailableMemberSection;
using Lab.Application.Features.PaperContributor.Queries.GetMemberSection;
using Lab.Application.Features.PaperContributor.Queries.GetPaperContributors;
using Lab.Application.Features.PaperTag.Commands.AddTagToPaper;
using Lab.Application.Features.Section.Commands.MarkMainSection;
using Lab.Application.Features.Section.Commands.MarkSectionToCompleted;
using Lab.Application.Features.Section.Commands.MarkSectionToReview;
using Lab.Application.Features.Section.Commands.UpdateReference;
using Lab.Application.Features.Section.Commands.UploadSectionFile;
using Lab.Application.Features.Section.Queries.GetInUseReferenceBySectionId;
using Lab.Application.Features.Section.Queries.GetPreviewReference;
using Lab.Application.Features.Section.Queries.GetReferenceBySectionId;
using Lab.Application.Features.Section.Queries.GetSectionById;
using Lab.Application.Features.Section.Queries.GetSectionnFileById;
using Lab.Application.Features.Tag.Queries.GetTagById;
using Lab.Application.Features.TaskDefinition.Commands.CreateTask;
using Lab.Application.Features.Template.Queries.GetTemplateById;
using Lab.Application.Models.Filters;
using Lab.Domain.Enums;

namespace Lab.Application.Tests.Validators;

public sealed class UpdateCombinePaperCommandValidatorTests
{
    private readonly UpdateCombinePaperCommandValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenPaperIdIsEmpty()
    {
        var command = new UpdateCombinePaperCommand(Guid.Empty, Guid.NewGuid(), "user", new UpdateCombinePaperDto { ProjectId = Guid.NewGuid(), Content = "content" });

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.PaperId);
    }

    [Fact]
    public void ShouldHaveError_WhenVersionIdIsEmpty()
    {
        var command = new UpdateCombinePaperCommand(Guid.NewGuid(), Guid.Empty, "user", new UpdateCombinePaperDto { ProjectId = Guid.NewGuid(), Content = "content" });

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.VersionId);
    }

    [Fact]
    public void ShouldHaveError_WhenProjectIdIsEmpty()
    {
        var command = new UpdateCombinePaperCommand(Guid.NewGuid(), Guid.NewGuid(), "user", new UpdateCombinePaperDto { ProjectId = Guid.Empty, Content = "content" });

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto.ProjectId);
    }

    [Fact]
    public void ShouldHaveError_WhenContentIsEmpty()
    {
        var command = new UpdateCombinePaperCommand(Guid.NewGuid(), Guid.NewGuid(), "user", new UpdateCombinePaperDto { ProjectId = Guid.NewGuid(), Content = string.Empty });

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto.Content);
    }

    [Fact]
    public void ShouldNotHaveError_WhenValid()
    {
        var command = new UpdateCombinePaperCommand(Guid.NewGuid(), Guid.NewGuid(), "user", new UpdateCombinePaperDto { ProjectId = Guid.NewGuid(), Content = "content" });

        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }
}

public sealed class GetCombinePaperByIdQueryValidatorTests
{
    private readonly GetCombinePaperByIdQueryValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenPaperIdIsEmpty()
    {
        var query = new GetCombinePaperByIdQuery(Guid.Empty, Guid.NewGuid());

        _validator.TestValidate(query).ShouldHaveValidationErrorFor(x => x.PaperId);
    }

    [Fact]
    public void ShouldHaveError_WhenVersionIdIsEmpty()
    {
        var query = new GetCombinePaperByIdQuery(Guid.NewGuid(), Guid.Empty);

        _validator.TestValidate(query).ShouldHaveValidationErrorFor(x => x.VersionId);
    }

    [Fact]
    public void ShouldNotHaveError_WhenValid()
    {
        var query = new GetCombinePaperByIdQuery(Guid.NewGuid(), Guid.NewGuid());

        _validator.TestValidate(query).ShouldNotHaveAnyValidationErrors();
    }
}

public sealed class GetPaperByIdQueryValidatorTests
{
    private readonly GetPaperByIdQueryValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenIdIsEmpty()
    {
        var query = new GetPaperByIdQuery(Guid.Empty);

        _validator.TestValidate(query).ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldNotHaveError_WhenIdIsProvided()
    {
        var query = new GetPaperByIdQuery(Guid.NewGuid());

        _validator.TestValidate(query).ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}

public sealed class GetPaperStatusHistoryQueryValidatorTests
{
    private readonly GetPaperStatusHistoryQueryValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenPaperIdIsEmpty()
    {
        var query = new GetPaperStatusHistoryQuery(Guid.Empty);

        _validator.TestValidate(query).ShouldHaveValidationErrorFor(x => x.PaperId);
    }

    [Fact]
    public void ShouldNotHaveError_WhenPaperIdIsProvided()
    {
        var query = new GetPaperStatusHistoryQuery(Guid.NewGuid());

        _validator.TestValidate(query).ShouldNotHaveValidationErrorFor(x => x.PaperId);
    }
}

public sealed class GetPaperVersionFileByIdQueryValidatorTests
{
    private readonly GetPaperVersionFileByIdQueryValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenIdIsEmpty()
    {
        var query = new GetPaperVersionFileByIdQuery(Guid.Empty);

        _validator.TestValidate(query).ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldNotHaveError_WhenIdIsProvided()
    {
        var query = new GetPaperVersionFileByIdQuery(Guid.NewGuid());

        _validator.TestValidate(query).ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}

public sealed class GetPaperVersionFilesQueryValidatorTests
{
    private readonly GetPaperVersionFilesQueryValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenPaperIdIsEmpty()
    {
        var query = new GetPaperVersionFilesQuery(Guid.Empty, Guid.NewGuid());

        _validator.TestValidate(query).ShouldHaveValidationErrorFor(x => x.PaperId);
    }

    [Fact]
    public void ShouldHaveError_WhenVersionIdIsEmpty()
    {
        var query = new GetPaperVersionFilesQuery(Guid.NewGuid(), Guid.Empty);

        _validator.TestValidate(query).ShouldHaveValidationErrorFor(x => x.VersionId);
    }

    [Fact]
    public void ShouldNotHaveError_WhenValid()
    {
        var query = new GetPaperVersionFilesQuery(Guid.NewGuid(), Guid.NewGuid());

        _validator.TestValidate(query).ShouldNotHaveAnyValidationErrors();
    }
}

public sealed class GetSectionsByPaperIdQueryValidatorTests
{
    private readonly GetSectionsByPaperIdQueryValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenPaperIdIsEmpty()
    {
        var query = new GetSectionsByPaperIdQuery(Guid.Empty);

        _validator.TestValidate(query).ShouldHaveValidationErrorFor(x => x.PaperId);
    }

    [Fact]
    public void ShouldNotHaveError_WhenPaperIdIsProvided()
    {
        var query = new GetSectionsByPaperIdQuery(Guid.NewGuid());

        _validator.TestValidate(query).ShouldNotHaveValidationErrorFor(x => x.PaperId);
    }
}

public sealed class GetVersionsByPaperIdQueryValidatorTests
{
    private readonly GetVersionsByPaperIdQueryValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenPaperIdIsEmpty()
    {
        var query = new GetVersionsByPaperIdQuery(Guid.Empty);

        _validator.TestValidate(query).ShouldHaveValidationErrorFor(x => x.PaperId);
    }

    [Fact]
    public void ShouldNotHaveError_WhenPaperIdIsProvided()
    {
        var query = new GetVersionsByPaperIdQuery(Guid.NewGuid());

        _validator.TestValidate(query).ShouldNotHaveValidationErrorFor(x => x.PaperId);
    }
}

public sealed class DeletePaperBankCommandValidatorTests
{
    private readonly DeletePaperBankCommandValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenIdIsEmpty()
    {
        var command = new DeletePaperBankCommand(Guid.Empty);

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldNotHaveError_WhenIdIsProvided()
    {
        var command = new DeletePaperBankCommand(Guid.NewGuid());

        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}

public sealed class UpdatePaperBankCommandValidatorTests
{
    private readonly UpdatePaperCommandVaBanklidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenIdIsEmpty()
    {
        var command = new UpdatePaperBankCommand(Guid.Empty, CreateValidUpdatePaperBankDto());

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldHaveError_WhenBankDtoIsNull()
    {
        var command = new UpdatePaperBankCommand(Guid.NewGuid(), null!);

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.BankDto);
    }

    [Fact]
    public void ShouldHaveError_WhenTitleIsEmpty()
    {
        var command = new UpdatePaperBankCommand(Guid.NewGuid(), new UpdatePaperBankDto { Title = string.Empty, PublicationDate = DateTimeOffset.UtcNow.AddDays(-1) });

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.BankDto.Title);
    }

    [Fact]
    public void ShouldHaveError_WhenPublicationDateIsInFuture()
    {
        var command = new UpdatePaperBankCommand(Guid.NewGuid(), new UpdatePaperBankDto { Title = "Paper", PublicationDate = DateTimeOffset.UtcNow.AddDays(1) });

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.BankDto.PublicationDate);
    }

    [Fact]
    public void ShouldNotHaveError_WhenValid()
    {
        var command = new UpdatePaperBankCommand(Guid.NewGuid(), CreateValidUpdatePaperBankDto());

        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }

    private static UpdatePaperBankDto CreateValidUpdatePaperBankDto() => new()
    {
        Title = "Paper",
        PublicationDate = DateTimeOffset.UtcNow.AddDays(-1),
        ConferenceJournalId = Guid.NewGuid()
    };
}

public sealed class UpdatePaperBankIngestionStatusValidatorTests
{
    private readonly UpdatePaperBankIngestionStatusValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenPaperIdIsEmpty()
    {
        var command = new UpdatePaperBankIngestionStatusCommand(Guid.Empty, true, null);

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.PaperId);
    }

    [Fact]
    public void ShouldRetainRecordValues_WhenConstructed()
    {
        var command = new UpdatePaperBankIngestionStatusCommand(Guid.NewGuid(), false, "error");

        command.IsSuccess.Should().BeFalse();
        command.ErrorMessage.Should().Be("error");
    }

    [Fact]
    public void ShouldNotHaveError_WhenPaperIdIsProvided()
    {
        var command = new UpdatePaperBankIngestionStatusCommand(Guid.NewGuid(), true, null);

        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.PaperId);
    }
}

public sealed class GetPaperBankByIdQueryValidatorTests
{
    private readonly GetPaperBankByIdQueryValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenIdIsEmpty()
    {
        var query = new GetPaperBankByIdQuery(Guid.Empty);

        _validator.TestValidate(query).ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldNotHaveError_WhenIdIsProvided()
    {
        var query = new GetPaperBankByIdQuery(Guid.NewGuid());

        _validator.TestValidate(query).ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}

public sealed class UpdatePaperContributorCommandValidatorCoverageTests
{
    private readonly UpdatePaperContributorCommandValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenDtoIsNull()
    {
        var command = new UpdatePaperContributorCommand(Guid.NewGuid(), null!);

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto);
    }

    [Fact]
    public void ShouldHaveError_WhenMemberIdIsNull()
    {
        var command = new UpdatePaperContributorCommand(Guid.NewGuid(), new UpdatePaperContributorDto { MemberId = null });

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto.MemberId);
    }

    [Fact]
    public void ShouldNotHaveError_WhenValid()
    {
        var command = new UpdatePaperContributorCommand(Guid.NewGuid(), new UpdatePaperContributorDto { MemberId = Guid.NewGuid() });

        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }
}

public sealed class GetMySectionsQueryValidatorTests
{
    private readonly GetMySectionsQueryValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenPaperIdIsEmpty()
    {
        var query = new GetAssignedPaperSectionsQuery(Guid.Empty, Guid.NewGuid(), new PaginationRequest());

        _validator.TestValidate(query).ShouldHaveValidationErrorFor(x => x.PaperId);
    }

    [Fact]
    public void ShouldHaveError_WhenUserIdIsEmpty()
    {
        var query = new GetAssignedPaperSectionsQuery(Guid.NewGuid(), Guid.Empty, new PaginationRequest());

        _validator.TestValidate(query).ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void ShouldNotHaveError_WhenValid()
    {
        var query = new GetAssignedPaperSectionsQuery(Guid.NewGuid(), Guid.NewGuid(), new PaginationRequest());

        _validator.TestValidate(query).ShouldNotHaveAnyValidationErrors();
    }
}

public sealed class GetAssignedPaperSectionsHistoryQueryValidatorTests
{
    private readonly GetAssignedPaperSectionsHistoryQueryValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenPaperIdIsEmpty()
    {
        var query = new GetAssignedPaperSectionsHistoryQuery(Guid.Empty, Guid.NewGuid(), new GetAssignedPaperSectionsHistoryFilter(), new PaginationRequest());

        _validator.TestValidate(query).ShouldHaveValidationErrorFor(x => x.PaperId);
    }

    [Fact]
    public void ShouldHaveError_WhenUserIdIsEmpty()
    {
        var query = new GetAssignedPaperSectionsHistoryQuery(Guid.NewGuid(), Guid.Empty, new GetAssignedPaperSectionsHistoryFilter(), new PaginationRequest());

        _validator.TestValidate(query).ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void ShouldHaveError_WhenFromDateIsAfterToDate()
    {
        var query = new GetAssignedPaperSectionsHistoryQuery(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new GetAssignedPaperSectionsHistoryFilter
            {
                FromDate = DateTimeOffset.UtcNow,
                ToDate = DateTimeOffset.UtcNow.AddDays(-1)
            },
            new PaginationRequest());

        _validator.TestValidate(query).ShouldHaveValidationErrorFor(x => x.Filter);
    }

    [Fact]
    public void ShouldRetainPagingValues_WhenConstructed()
    {
        var paging = new PaginationRequest(2, 25);
        var query = new GetAssignedPaperSectionsHistoryQuery(Guid.NewGuid(), Guid.NewGuid(), new GetAssignedPaperSectionsHistoryFilter(), paging);

        query.Paging.Should().Be(paging);
    }

    [Fact]
    public void ShouldNotHaveError_WhenValid()
    {
        var query = new GetAssignedPaperSectionsHistoryQuery(Guid.NewGuid(), Guid.NewGuid(), new GetAssignedPaperSectionsHistoryFilter(), new PaginationRequest());

        _validator.TestValidate(query).ShouldNotHaveAnyValidationErrors();
    }
}

public sealed class GetAvailableMemberSectionQueryValidatorTests
{
    private readonly GetAvailableMemberSectionQueryValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenSectionIdIsEmpty()
    {
        var query = new GetAvailableMemberSectionQuery(Guid.Empty, Guid.NewGuid());

        _validator.TestValidate(query).ShouldHaveValidationErrorFor(x => x.SectionId);
    }

    [Fact]
    public void ShouldHaveError_WhenPaperIdIsEmpty()
    {
        var query = new GetAvailableMemberSectionQuery(Guid.NewGuid(), Guid.Empty);

        _validator.TestValidate(query).ShouldHaveValidationErrorFor(x => x.PaperId);
    }

    [Fact]
    public void ShouldNotHaveError_WhenValid()
    {
        var query = new GetAvailableMemberSectionQuery(Guid.NewGuid(), Guid.NewGuid());

        _validator.TestValidate(query).ShouldNotHaveAnyValidationErrors();
    }
}

public sealed class GetMemberSectionQueryValidatorTests
{
    private readonly GetMemberSectionQueryValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenSectionIdIsEmpty()
    {
        var query = new GetMemberSectionQuery(Guid.Empty, Guid.NewGuid());

        _validator.TestValidate(query).ShouldHaveValidationErrorFor(x => x.SectionId);
    }

    [Fact]
    public void ShouldHaveError_WhenPaperIdIsEmpty()
    {
        var query = new GetMemberSectionQuery(Guid.NewGuid(), Guid.Empty);

        _validator.TestValidate(query).ShouldHaveValidationErrorFor(x => x.PaperId);
    }

    [Fact]
    public void ShouldNotHaveError_WhenValid()
    {
        var query = new GetMemberSectionQuery(Guid.NewGuid(), Guid.NewGuid());

        _validator.TestValidate(query).ShouldNotHaveAnyValidationErrors();
    }
}

public sealed class GetPaperContributorsQueryValidatorTests
{
    private readonly GetPaperContributorsQueryValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenPaperIdIsEmpty()
    {
        var query = new GetPaperContributorsQuery(Guid.Empty);

        _validator.TestValidate(query).ShouldHaveValidationErrorFor(x => x.PaperId);
    }

    [Fact]
    public void ShouldNotHaveError_WhenPaperIdIsProvided()
    {
        var query = new GetPaperContributorsQuery(Guid.NewGuid());

        _validator.TestValidate(query).ShouldNotHaveValidationErrorFor(x => x.PaperId);
    }
}

public sealed class AddTagToPaperCommandValidatorTests
{
    private readonly AddTagToPaperCommandValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenPaperIdIsEmpty()
    {
        var command = new AddTagToPaperCommand(Guid.Empty, new List<Guid> { Guid.NewGuid() });

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.PaperId);
    }

    [Fact]
    public void ShouldHaveError_WhenTagsAreEmpty()
    {
        var command = new AddTagToPaperCommand(Guid.NewGuid(), new List<Guid>());

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Tags);
    }

    [Fact]
    public void ShouldNotHaveError_WhenValid()
    {
        var command = new AddTagToPaperCommand(Guid.NewGuid(), new List<Guid> { Guid.NewGuid() });

        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }
}

public sealed class MarkMainSectionCommandValidatorTests
{
    private readonly MarkMainSectionCommandValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenIdIsEmpty()
    {
        var command = new MarkMainSectionCommand(new MarkMainSectionDto { ProjectId = Guid.NewGuid() }, Guid.Empty);

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldHaveError_WhenProjectIdIsEmpty()
    {
        var command = new MarkMainSectionCommand(new MarkMainSectionDto { ProjectId = Guid.Empty }, Guid.NewGuid());

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto.ProjectId);
    }

    [Fact]
    public void ShouldNotHaveError_WhenValid()
    {
        var command = new MarkMainSectionCommand(new MarkMainSectionDto { ProjectId = Guid.NewGuid() }, Guid.NewGuid());

        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }
}

public sealed class MarkSectionToCompletedCommandValidatorTests
{
    private readonly MarkSectionToCompletedCommandValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenIdIsEmpty()
    {
        var command = new MarkSectionToCompletedCommand(Guid.Empty, new MarkSectionToCompletedDto { MemberId = Guid.NewGuid(), ProjectId = Guid.NewGuid() }, "user");

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldHaveError_WhenMemberIdIsEmpty()
    {
        var command = new MarkSectionToCompletedCommand(Guid.NewGuid(), new MarkSectionToCompletedDto { MemberId = Guid.Empty, ProjectId = Guid.NewGuid() }, "user");

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto.MemberId);
    }

    [Fact]
    public void ShouldHaveError_WhenProjectIdIsEmpty()
    {
        var command = new MarkSectionToCompletedCommand(Guid.NewGuid(), new MarkSectionToCompletedDto { MemberId = Guid.NewGuid(), ProjectId = Guid.Empty }, "user");

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto.ProjectId);
    }

    [Fact]
    public void ShouldNotHaveError_WhenValid()
    {
        var command = new MarkSectionToCompletedCommand(Guid.NewGuid(), new MarkSectionToCompletedDto { MemberId = Guid.NewGuid(), ProjectId = Guid.NewGuid() }, "user");

        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }
}

public sealed class MarkSectionToReviewCommandValidatorTests
{
    private readonly MarkSectionToReviewCommandValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenIdIsEmpty()
    {
        var command = new MarkSectionToReviewCommand(Guid.Empty, new MarkSectionToReviewDto { MemberId = Guid.NewGuid(), ProjectId = Guid.NewGuid() }, "user");

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldHaveError_WhenMemberIdIsEmpty()
    {
        var command = new MarkSectionToReviewCommand(Guid.NewGuid(), new MarkSectionToReviewDto { MemberId = Guid.Empty, ProjectId = Guid.NewGuid() }, "user");

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto.MemberId);
    }

    [Fact]
    public void ShouldHaveError_WhenProjectIdIsEmpty()
    {
        var command = new MarkSectionToReviewCommand(Guid.NewGuid(), new MarkSectionToReviewDto { MemberId = Guid.NewGuid(), ProjectId = Guid.Empty }, "user");

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto.ProjectId);
    }

    [Fact]
    public void ShouldNotHaveError_WhenValid()
    {
        var command = new MarkSectionToReviewCommand(Guid.NewGuid(), new MarkSectionToReviewDto { MemberId = Guid.NewGuid(), ProjectId = Guid.NewGuid() }, "user");

        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }
}

public sealed class UpdateReferenceCommandValidatorCoverageTests
{
    private readonly UpdateReferenceCommandValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenDtoIsNull()
    {
        var command = new UpdateReferenceCommand(null!, Guid.NewGuid(), "user", Guid.NewGuid());

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto);
    }

    [Fact]
    public void ShouldHaveError_WhenPaperIdIsEmpty()
    {
        var command = new UpdateReferenceCommand(new UpdateReferenceDto { PaperId = Guid.Empty, PaperBankIds = new List<Guid> { Guid.NewGuid() } }, Guid.NewGuid(), "user", Guid.NewGuid());

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto.PaperId);
    }

    [Fact]
    public void ShouldHaveError_WhenPaperBankIdsContainEmptyGuid()
    {
        var command = new UpdateReferenceCommand(new UpdateReferenceDto { PaperId = Guid.NewGuid(), PaperBankIds = new List<Guid> { Guid.Empty } }, Guid.NewGuid(), "user", Guid.NewGuid());

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto.PaperBankIds);
    }

    [Fact]
    public void ShouldHaveError_WhenIdIsEmpty()
    {
        var command = new UpdateReferenceCommand(new UpdateReferenceDto { PaperId = Guid.NewGuid(), PaperBankIds = new List<Guid> { Guid.NewGuid() } }, Guid.NewGuid(), "user", Guid.Empty);

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldNotHaveError_WhenValid()
    {
        var command = new UpdateReferenceCommand(new UpdateReferenceDto { PaperId = Guid.NewGuid(), PaperBankIds = new List<Guid> { Guid.NewGuid() } }, Guid.NewGuid(), "user", Guid.NewGuid());

        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }
}

public sealed class UploadSectionFileCommandValidatorTests
{
    private readonly UploadSectionFileCommandValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenIdIsEmpty()
    {
        var command = new UploadSectionFileCommand(new UploadSectionFileDto { UploadFile = CreateUploadFile() }, Guid.Empty);

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldHaveError_WhenUploadFileIsNull()
    {
        var command = new UploadSectionFileCommand(new UploadSectionFileDto { UploadFile = null! }, Guid.NewGuid());

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto.UploadFile);
    }

    [Fact]
    public void ShouldNotHaveError_WhenValid()
    {
        var command = new UploadSectionFileCommand(new UploadSectionFileDto { UploadFile = CreateUploadFile() }, Guid.NewGuid());

        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }

    private static UploadFileBytes CreateUploadFile() => new()
    {
        FileName = "section.tex",
        ContentType = "text/plain",
        Bytes = new byte[] { 1 }
    };
}

public sealed class GetInUseReferenceBySectionIdQueryValidatorTests
{
    private readonly GetInUseReferenceBySectionIdQueryValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenIdIsEmpty()
    {
        var query = new GetInUseReferenceBySectionIdQuery(Guid.Empty);

        _validator.TestValidate(query).ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldNotHaveError_WhenIdIsProvided()
    {
        var query = new GetInUseReferenceBySectionIdQuery(Guid.NewGuid());

        _validator.TestValidate(query).ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}

public sealed class GetPreviewReferenceQueryValidatorTests
{
    private readonly GetPreviewReferenceQueryValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenDtoIsNull()
    {
        var query = new GetPreviewReferenceQuery(null!);

        _validator.TestValidate(query).ShouldHaveValidationErrorFor(x => x.Dto);
    }

    [Fact]
    public void ShouldHaveError_WhenPaperBankIdsContainEmptyGuid()
    {
        var query = new GetPreviewReferenceQuery(new PreviewReferenceDto { PaperBankIds = new List<Guid> { Guid.Empty } });

        _validator.TestValidate(query).ShouldHaveValidationErrorFor(x => x.Dto.PaperBankIds);
    }

    [Fact]
    public void ShouldNotHaveError_WhenValid()
    {
        var query = new GetPreviewReferenceQuery(new PreviewReferenceDto { PaperBankIds = new List<Guid> { Guid.NewGuid() } });

        _validator.TestValidate(query).ShouldNotHaveAnyValidationErrors();
    }
}

public sealed class GetReferenceBySectionIdQueryValidatorTests
{
    private readonly GetReferenceBySectionIdQueryValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenIdIsEmpty()
    {
        var query = new GetReferenceBySectionIdQuery(Guid.Empty);

        _validator.TestValidate(query).ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldNotHaveError_WhenIdIsProvided()
    {
        var query = new GetReferenceBySectionIdQuery(Guid.NewGuid());

        _validator.TestValidate(query).ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}

public sealed class GetSectionByIdQueryValidatorTests
{
    private readonly GetSectionByIdQueryValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenIdIsEmpty()
    {
        var query = new GetSectionByIdQuery(Guid.Empty);

        _validator.TestValidate(query).ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldNotHaveError_WhenIdIsProvided()
    {
        var query = new GetSectionByIdQuery(Guid.NewGuid());

        _validator.TestValidate(query).ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}

public sealed class GetSectionnFileByIdQueryValidatorTests
{
    private readonly GetSectionnFileByIdQueryValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenIdIsEmpty()
    {
        var query = new GetSectionnFileByIdQuery(Guid.Empty);

        _validator.TestValidate(query).ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldNotHaveError_WhenIdIsProvided()
    {
        var query = new GetSectionnFileByIdQuery(Guid.NewGuid());

        _validator.TestValidate(query).ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}

public sealed class GetTagByIdQueryValidatorTests
{
    private readonly GetTagByIdQueryValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenIdIsEmpty()
    {
        var query = new GetTagByIdQuery(Guid.Empty);

        _validator.TestValidate(query).ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldNotHaveError_WhenIdIsProvided()
    {
        var query = new GetTagByIdQuery(Guid.NewGuid());

        _validator.TestValidate(query).ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}

public sealed class CreateTaskValidatorTests
{
    private readonly CreateTaskValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenDtoIsNull()
    {
        var command = new CreateTaskCommand(null!, Guid.NewGuid().ToString(), "user");

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto);
    }

    [Fact]
    public void ShouldHaveError_WhenNameIsEmpty()
    {
        var command = new CreateTaskCommand(new CreateTaskDto { Name = string.Empty }, Guid.NewGuid().ToString(), "user");

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Dto.Name);
    }

    [Fact]
    public void ShouldNotHaveError_WhenValid()
    {
        var command = new CreateTaskCommand(new CreateTaskDto
        {
            Name = "Review",
            PaperId = Guid.NewGuid(),
            MemberId = Guid.NewGuid(),
            Status = TaskDefineStatus.ToDo,
            Type = TaskType.Review
        }, Guid.NewGuid().ToString(), "user");

        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }
}

public sealed class GetTemplateByIdQueryValidatorTests
{
    private readonly GetTemplateByIdQueryValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenIdIsEmpty()
    {
        var query = new GetTemplateByIdQuery(Guid.Empty);

        _validator.TestValidate(query).ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldNotHaveError_WhenIdIsProvided()
    {
        var query = new GetTemplateByIdQuery(Guid.NewGuid());

        _validator.TestValidate(query).ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}
