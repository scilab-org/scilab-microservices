using User.Application.Features.Roles.Queries;
using User.Application.Tests.Common;

namespace User.Application.Tests.Features.Roles.Validators;

public sealed class GetGroupRolesQueryValidatorTests : BaseTest
{
    private readonly GetGroupRolesQueryValidator _validator;

    public GetGroupRolesQueryValidatorTests()
    {
        _validator = new GetGroupRolesQueryValidator();
    }

    [Fact]
    public async Task Validate_ShouldPass_WhenQueryIsValid()
    {
        // Arrange
        var query = new GetGroupRolesQuery("group-id-001");

        // Act
        var result = await _validator.ValidateAsync(query, CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenGroupIdIsEmpty()
    {
        // Arrange
        var query = new GetGroupRolesQuery(string.Empty);

        // Act
        var result = await _validator.ValidateAsync(query, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.GroupIdIsRequired);
    }
}
