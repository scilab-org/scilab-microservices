using Management.Application.Features.Dataset.Commands;
using Management.Application.Tests.Common;

namespace Management.Application.Tests.Features.Dataset.Validators;

public sealed class DeleteDatasetValidatorTests : BaseTest
{
    private readonly DeleteDatasetValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldPass_WhenIdIsValid()
    {
        // Arrange
        var command = new DeleteDatasetCommand(Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenIdIsEmpty()
    {
        // Arrange
        var command = new DeleteDatasetCommand(Guid.Empty);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.DatasetIdIsRequired);
    }
}
