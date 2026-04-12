using User.Application.Features.Users;
using User.Application.Tests.Common;
using User.Application.Tests.Common.TestData;

namespace User.Application.Tests.Features.Users.Validators;

public sealed class DeactivateUserCommandValidatorTests : BaseTest
{
    private readonly DeactivateUserCommandValidator _validator;

    public DeactivateUserCommandValidatorTests()
    {
        _validator = new DeactivateUserCommandValidator();
    }

    [Fact]
    public async Task Validate_ShouldPass_WhenCommandIsValid()
    {
        // Arrange
        var command = new DeactivateUserCommand("user-id-001", UserTestData.SystemActor());

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenUserIdIsEmpty()
    {
        // Arrange
        var command = new DeactivateUserCommand(string.Empty, UserTestData.SystemActor());

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.UserIdIsRequired);
    }
}
