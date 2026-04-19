using Management.Application.Features.Member.Commands;
using Management.Application.Tests.Common;

namespace Management.Application.Tests.Features.Member.Validators;

public sealed class AddProjectManagersValidatorTests : BaseTest
{
    private readonly AddProjectManagersValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldPass_WhenCommandIsValid()
    {
        // Arrange
        var dto = new AddProjectManagersDto { UserId = Guid.NewGuid() };
        var command = new AddProjectManagersCommand(Guid.NewGuid(), dto);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenProjectIdIsEmpty()
    {
        // Arrange
        var dto = new AddProjectManagersDto { UserId = Guid.NewGuid() };
        var command = new AddProjectManagersCommand(Guid.Empty, dto);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.MemberProjectIdIsRequired);
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenUserIdIsEmpty()
    {
        // Arrange
        var dto = new AddProjectManagersDto { UserId = Guid.Empty };
        var command = new AddProjectManagersCommand(Guid.NewGuid(), dto);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.UserIdsAreRequired);
    }
}
