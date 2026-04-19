using Management.Application.Features.Member.Queries;
using Management.Application.Tests.Common;

namespace Management.Application.Tests.Features.Member.Validators;

public sealed class GetProjectMembersValidatorTests : BaseTest
{
    private readonly GetProjectMembersValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldPass_WhenProjectIdIsValid()
    {
        // Arrange
        var query = new GetProjectMembersQuery(Guid.NewGuid(), new GetProjectMembersFilter(), new PaginationRequest());

        // Act
        var result = await _validator.ValidateAsync(query, CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenProjectIdIsEmpty()
    {
        // Arrange
        var query = new GetProjectMembersQuery(Guid.Empty, new GetProjectMembersFilter(), new PaginationRequest());

        // Act
        var result = await _validator.ValidateAsync(query, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.MemberProjectIdIsRequired);
    }
}
