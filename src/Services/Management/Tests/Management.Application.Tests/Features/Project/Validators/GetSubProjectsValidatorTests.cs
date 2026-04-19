using Management.Application.Features.Project.Queries;
using Management.Application.Tests.Common;

namespace Management.Application.Tests.Features.Project.Validators;

public sealed class GetSubProjectsValidatorTests : BaseTest
{
    private readonly GetSubProjectsValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldPass_WhenProjectIdIsValid()
    {
        // Arrange
        var query = new GetSubProjectsQuery(Guid.NewGuid(), new PaginationRequest());

        // Act
        var result = await _validator.ValidateAsync(query, CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenProjectIdIsEmpty()
    {
        // Arrange
        var query = new GetSubProjectsQuery(Guid.Empty, new PaginationRequest());

        // Act
        var result = await _validator.ValidateAsync(query, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.ProjectIdIsRequired);
    }
}
