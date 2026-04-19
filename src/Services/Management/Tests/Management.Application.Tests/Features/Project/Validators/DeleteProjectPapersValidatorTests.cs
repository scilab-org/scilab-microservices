using Management.Application.Features.Project.Commands;
using Management.Application.Tests.Common;

namespace Management.Application.Tests.Features.Project.Validators;

public sealed class DeleteProjectPapersValidatorTests : BaseTest
{
    private readonly DeleteProjectPapersValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldPass_WhenCommandIsValid()
    {
        // Arrange
        var dto = new DeleteProjectPaperDto { PaperIds = [Guid.NewGuid()] };
        var command = new DeleteProjectPapersCommand(Guid.NewGuid(), dto);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenProjectIdIsEmpty()
    {
        // Arrange
        var dto = new DeleteProjectPaperDto { PaperIds = [Guid.NewGuid()] };
        var command = new DeleteProjectPapersCommand(Guid.Empty, dto);

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
        var dto = new DeleteProjectPaperDto { PaperIds = [] };
        var command = new DeleteProjectPapersCommand(Guid.NewGuid(), dto);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.PaperIdIsRequired);
    }
}
