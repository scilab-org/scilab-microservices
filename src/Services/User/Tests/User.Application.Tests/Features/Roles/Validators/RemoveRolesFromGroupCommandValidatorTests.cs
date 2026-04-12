using User.Application.Features.Roles.Commands;
using User.Application.Tests.Common;

namespace User.Application.Tests.Features.Roles.Validators;

public sealed class RemoveRolesFromGroupCommandValidatorTests : BaseTest
{
    private readonly RemoveRolesFromGroupCommandValidator _validator;

    public RemoveRolesFromGroupCommandValidatorTests()
    {
        _validator = new RemoveRolesFromGroupCommandValidator();
    }

    [Fact]
    public async Task Validate_ShouldPass_WhenCommandIsValid()
    {
        // Arrange
        var command = new RemoveRolesFromGroupCommand("group-id-001", ["view-data"]);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenGroupIdIsEmpty()
    {
        // Arrange
        var command = new RemoveRolesFromGroupCommand(string.Empty, ["view-data"]);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.GroupIdIsRequired);
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenRoleNamesIsEmpty()
    {
        // Arrange
        var command = new RemoveRolesFromGroupCommand("group-id-001", []);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.RoleNamesAreRequired);
    }
}
