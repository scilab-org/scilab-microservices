using Management.Application.Features.Project.Queries;
using Management.Application.Tests.Common;

namespace Management.Application.Tests.Features.Project.Validators;

public sealed class GetMyProjectRoleQueryValidatorTests : BaseTest
{
    private readonly GetMyProjectRoleQueryValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldPass_WhenQueryIsValid()
    {
        // Arrange
        var query = new GetMyProjectRoleQuery(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(query, CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenUserIdIsEmpty()
    {
        // Arrange
        var query = new GetMyProjectRoleQuery(Guid.Empty, Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(query, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.UserIdIsRequired);
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenProjectIdIsEmpty()
    {
        // Arrange
        var query = new GetMyProjectRoleQuery(Guid.NewGuid(), Guid.Empty);

        // Act
        var result = await _validator.ValidateAsync(query, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.ProjectIdIsRequired);
    }
}
