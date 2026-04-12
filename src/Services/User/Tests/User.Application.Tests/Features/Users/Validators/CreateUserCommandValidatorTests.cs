using User.Application.Features.Users;
using User.Application.Tests.Common;
using User.Application.Tests.Common.TestData;

namespace User.Application.Tests.Features.Users.Validators;

public sealed class CreateUserCommandValidatorTests : BaseTest
{
    private readonly CreateUserCommandValidator _validator;

    public CreateUserCommandValidatorTests()
    {
        _validator = new CreateUserCommandValidator();
    }

    [Fact]
    public async Task Validate_ShouldPass_WhenCommandIsValid()
    {
        // Arrange
        var dto = UserTestData.CreateUserDtoRequest();
        var command = new CreateUserCommand(dto, UserTestData.SystemActor());

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
        var command = new CreateUserCommand(null!, UserTestData.SystemActor());

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.ErrorMessage == MessageCode.BadRequest);
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenUsernameIsEmpty()
    {
        // Arrange
        var dto = UserTestData.CreateUserDtoRequest(username: string.Empty);
        var command = new CreateUserCommand(dto, UserTestData.SystemActor());

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.UsernameIsRequired);
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenUsernameTooShort()
    {
        // Arrange
        var dto = UserTestData.CreateUserDtoRequest(username: "ab");
        var command = new CreateUserCommand(dto, UserTestData.SystemActor());

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.BadRequest);
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenEmailIsEmpty()
    {
        // Arrange
        var dto = UserTestData.CreateUserDtoRequest(email: string.Empty);
        var command = new CreateUserCommand(dto, UserTestData.SystemActor());

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.EmailIsRequired);
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenEmailIsInvalidFormat()
    {
        // Arrange
        var dto = UserTestData.CreateUserDtoRequest(email: "not-an-email");
        var command = new CreateUserCommand(dto, UserTestData.SystemActor());

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.InvalidEmail);
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenInitialPasswordIsEmpty()
    {
        // Arrange
        var dto = UserTestData.CreateUserDtoRequest(initialPassword: string.Empty);
        var command = new CreateUserCommand(dto, UserTestData.SystemActor());

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.InitialPasswordIsRequired);
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenInitialPasswordIsTooShort()
    {
        // Arrange
        var dto = UserTestData.CreateUserDtoRequest(initialPassword: "short");
        var command = new CreateUserCommand(dto, UserTestData.SystemActor());

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.PasswordMinimumLength);
    }
}
