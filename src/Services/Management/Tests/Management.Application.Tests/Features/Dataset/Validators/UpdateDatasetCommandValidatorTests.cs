using Management.Application.Features.Dataset.Commands;
using Management.Application.Tests.Common;

namespace Management.Application.Tests.Features.Dataset.Validators;

public sealed class UpdateDatasetCommandValidatorTests : BaseTest
{
    private readonly UpdateDatasetCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldPass_WhenCommandIsValid()
    {
        // Arrange
        var dto = new UpdateDatasetDto
        {
            Name = "Updated Dataset",
            Description = "Updated description",
            UploadFile = null!
        };
        var command = new UpdateDatasetCommand(Guid.NewGuid(), dto);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenIdIsEmpty()
    {
        // Arrange
        var dto = new UpdateDatasetDto { Name = "Test", UploadFile = null! };
        var command = new UpdateDatasetCommand(Guid.Empty, dto);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.DatasetIdIsRequired);
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenDtoIsNull()
    {
        // Arrange
        var command = new UpdateDatasetCommand(Guid.NewGuid(), null!);

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
        var dto = new UpdateDatasetDto { Name = string.Empty, UploadFile = null! };
        var command = new UpdateDatasetCommand(Guid.NewGuid(), dto);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.DatasetNameIsRequired);
    }
}
