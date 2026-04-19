using Management.Application.Features.Dataset.Commands;
using Management.Application.Tests.Common;

namespace Management.Application.Tests.Features.Dataset.Validators;

public sealed class CreateDatasetCommandValidatorTests : BaseTest
{
    private readonly CreateDatasetCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldPass_WhenCommandIsValid()
    {
        // Arrange
        var dto = new CreateDatasetDto
        {
            ProjectId = Guid.NewGuid(),
            Name = "Test Dataset",
            Description = "A test dataset",
            UploadFile = null
        };
        var command = new CreateDatasetCommand(dto);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenDtoIsNull()
    {
        // Arrange
        var command = new CreateDatasetCommand(null!);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.BadRequest);
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenNameIsEmpty()
    {
        // Arrange
        var dto = new CreateDatasetDto
        {
            ProjectId = Guid.NewGuid(),
            Name = string.Empty,
            UploadFile = null
        };
        var command = new CreateDatasetCommand(dto);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.DatasetNameIsRequired);
    }

}
