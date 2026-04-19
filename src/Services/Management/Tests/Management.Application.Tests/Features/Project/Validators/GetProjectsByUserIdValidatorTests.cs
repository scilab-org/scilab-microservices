using Management.Application.Features.Project.Queries;
using Management.Application.Tests.Common;

namespace Management.Application.Tests.Features.Project.Validators;

public sealed class GetProjectsByUserIdValidatorTests : BaseTest
{
    private readonly GetProjectsByUserIdValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldPass_WhenUserIdIsValid()
    {
        // Arrange
        var query = new GetProjectsByUserIdQuery(Guid.NewGuid(), new PaginationRequest());

        // Act
        var result = await _validator.ValidateAsync(query, CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenUserIdIsEmpty()
    {
        // Arrange
        var query = new GetProjectsByUserIdQuery(Guid.Empty, new PaginationRequest());

        // Act
        var result = await _validator.ValidateAsync(query, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.UserIdIsRequired);
    }
}
