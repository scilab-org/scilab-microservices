using Management.Application.Features.Project.Commands;
using Management.Application.Tests.Common;
using Management.Application.Tests.Common.TestData;

namespace Management.Application.Tests.Features.Project.Validators;

public sealed class UpdateProjectCommandValidatorTests : BaseTest
{
    private readonly UpdateProjectCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldPass_WhenCommandIsValid()
    {
        // Arrange
        var dto = ProjectTestData.UpdateProjectDto();
        var command = new UpdateProjectCommand(Guid.NewGuid(), dto);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenProjectIdIsEmpty()
    {
        // Arrange
        var dto = ProjectTestData.UpdateProjectDto();
        var command = new UpdateProjectCommand(Guid.Empty, dto);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.ProjectIdIsRequired);
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenDtoIsNull()
    {
        // Arrange
        var command = new UpdateProjectCommand(Guid.NewGuid(), null!);

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
        var dto = ProjectTestData.UpdateProjectDto(name: string.Empty);
        var command = new UpdateProjectCommand(Guid.NewGuid(), dto);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.ProjectNameIsRequired);
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenStartDateIsAfterEndDate()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var dto = ProjectTestData.UpdateProjectDto(startDate: now.AddDays(10), endDate: now);
        var command = new UpdateProjectCommand(Guid.NewGuid(), dto);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.StartDateMustBeBeforeEndDate);
    }

    [Fact]
    public async Task Validate_ShouldPass_WhenDatesAreNull()
    {
        // Arrange
        var dto = ProjectTestData.UpdateProjectDto(startDate: null, endDate: null);
        var command = new UpdateProjectCommand(Guid.NewGuid(), dto);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
