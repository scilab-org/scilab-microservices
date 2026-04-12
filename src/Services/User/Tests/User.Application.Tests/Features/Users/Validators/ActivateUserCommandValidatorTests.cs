using User.Application.Features.Users;
using User.Application.Tests.Common;
using User.Application.Tests.Common.TestData;

namespace User.Application.Tests.Features.Users.Validators;

public sealed class ActivateUserCommandValidatorTests : BaseTest
{
    private readonly ActivateUserCommandValidator _validator;

    public ActivateUserCommandValidatorTests()
    {
        _validator = new ActivateUserCommandValidator();
    }

    [Fact]
    public async Task Validate_ShouldPass_WhenCommandIsValid()
    {
        // Arrange
        var command = new ActivateUserCommand("user-id-001", UserTestData.SystemActor());

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenUserIdIsEmpty()
    {
        // Arrange
        var command = new ActivateUserCommand(string.Empty, UserTestData.SystemActor());

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.UserIdIsRequired);
    }
}
