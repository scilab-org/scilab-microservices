using User.Application.Features.Users.Queries;
using User.Application.Tests.Common;

namespace User.Application.Tests.Features.Users.Validators;

public sealed class GetUserByIdQueryValidatorTests : BaseTest
{
    private readonly GetUserByIdQueryValidator _validator;

    public GetUserByIdQueryValidatorTests()
    {
        _validator = new GetUserByIdQueryValidator();
    }

    [Fact]
    public async Task Validate_ShouldPass_WhenQueryIsValid()
    {
        // Arrange
        var query = new GetUserByIdQuery("user-id-001");

        // Act
        var result = await _validator.ValidateAsync(query, CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenUserIdIsEmpty()
    {
        // Arrange
        var query = new GetUserByIdQuery(string.Empty);

        // Act
        var result = await _validator.ValidateAsync(query, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.UserIdIsRequired);
    }
}
