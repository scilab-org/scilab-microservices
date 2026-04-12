using User.Application.Features.Users;
using User.Application.Tests.Common;
using User.Application.Tests.Common.TestData;

namespace User.Application.Tests.Features.Users.Validators;

public sealed class UpdateUserGroupsCommandValidatorTests : BaseTest
{
    private readonly UpdateUserGroupsCommandValidator _validator;

    public UpdateUserGroupsCommandValidatorTests()
    {
        _validator = new UpdateUserGroupsCommandValidator();
    }

    [Fact]
    public async Task Validate_ShouldPass_WhenCommandIsValid()
    {
        // Arrange
        var command = new UpdateUserGroupsCommand(
            "user-id-001",
            ["Researchers", "Admins"],
            UserTestData.UserActor());

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldPass_WhenGroupNamesIsEmptyList()
    {
        // Arrange
        var command = new UpdateUserGroupsCommand(
            "user-id-001",
            [],
            UserTestData.UserActor());

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenUserIdIsEmpty()
    {
        // Arrange
        var command = new UpdateUserGroupsCommand(
            string.Empty,
            ["Researchers"],
            UserTestData.UserActor());

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.UserIdIsRequired);
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenGroupNamesIsNull()
    {
        // Arrange
        var command = new UpdateUserGroupsCommand(
            "user-id-001",
            null!,
            UserTestData.UserActor());

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.BadRequest);
    }
}
