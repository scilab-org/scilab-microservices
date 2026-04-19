using Management.Application.Features.Project.Commands;
using Management.Application.Tests.Common;

namespace Management.Application.Tests.Features.Project.Validators;

public sealed class CreateProjectPaperValidatorTests : BaseTest
{
    private readonly CreateProjectPaperValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldPass_WhenCommandIsValid()
    {
        // Arrange
        var dto = new CreateProjectPaperDto { PaperIds = new List<Guid> { Guid.NewGuid() } };
        var command = new CreateProjectPaperCommand(Guid.NewGuid(), dto);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenParentProjectIdIsEmpty()
    {
        // Arrange
        var dto = new CreateProjectPaperDto { PaperIds = new List<Guid> { Guid.NewGuid() } };
        var command = new CreateProjectPaperCommand(Guid.Empty, dto);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.ProjectIdIsRequired);
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenPaperIdsIsEmpty()
    {
        // Arrange
        var dto = new CreateProjectPaperDto { PaperIds = new List<Guid>() };
        var command = new CreateProjectPaperCommand(Guid.NewGuid(), dto);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "PAPER_IDS_REQUIRED");
    }
}
