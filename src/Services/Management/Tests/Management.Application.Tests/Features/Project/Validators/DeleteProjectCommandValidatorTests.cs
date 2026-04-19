using Management.Application.Features.Project.Commands;
using Management.Application.Tests.Common;

namespace Management.Application.Tests.Features.Project.Validators;

public sealed class DeleteProjectCommandValidatorTests : BaseTest
{
    private readonly DeleteProjectCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldPass_WhenProjectIdIsValid()
    {
        // Arrange
        var command = new DeleteProjectCommand(Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenProjectIdIsEmpty()
    {
        // Arrange
        var command = new DeleteProjectCommand(Guid.Empty);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.ProjectIdIsRequired);
    }
}
