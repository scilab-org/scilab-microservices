using Management.Application.Features.Project.Commands;
using Management.Application.Tests.Common;
using Management.Application.Tests.Common.TestData;

namespace Management.Application.Tests.Features.Project.Validators;

public sealed class CreateProjectCommandValidatorTests : BaseTest
{
    private readonly CreateProjectCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldPass_WhenCommandIsValid()
    {
        // Arrange
        var dto = ProjectTestData.CreateProjectDto();
        var command = new CreateProjectCommand(dto);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenDtoIsNull()
    {
        // Arrange
        var command = new CreateProjectCommand(null!);

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
        var dto = ProjectTestData.CreateProjectDto(name: string.Empty);
        var command = new CreateProjectCommand(dto);

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
        var dto = ProjectTestData.CreateProjectDto(
            startDate: now.AddDays(10),
            endDate: now);
        var command = new CreateProjectCommand(dto);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.StartDateMustBeBeforeEndDate);
    }

    [Fact]
    public async Task Validate_ShouldPass_WhenStartDateAndEndDateAreNull()
    {
        // Arrange
        var dto = ProjectTestData.CreateProjectDto(startDate: null, endDate: null);
        var command = new CreateProjectCommand(dto);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldPass_WhenOnlyStartDateProvided()
    {
        // Arrange
        var dto = ProjectTestData.CreateProjectDto(startDate: DateTimeOffset.UtcNow, endDate: null);
        var command = new CreateProjectCommand(dto);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
