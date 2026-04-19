using Management.Application.Features.Member.Queries;
using Management.Application.Tests.Common;

namespace Management.Application.Tests.Features.Member.Validators;

public sealed class GetSubProjectMembersValidatorTests : BaseTest
{
    private readonly GetSubProjectMembersValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldPass_WhenSubProjectIdIsValid()
    {
        // Arrange
        var query = new GetSubProjectMembersQuery(Guid.NewGuid(), new GetProjectMembersFilter(), new PaginationRequest());

        // Act
        var result = await _validator.ValidateAsync(query, CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenSubProjectIdIsEmpty()
    {
        // Arrange
        var query = new GetSubProjectMembersQuery(Guid.Empty, new GetProjectMembersFilter(), new PaginationRequest());

        // Act
        var result = await _validator.ValidateAsync(query, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.MemberProjectIdIsRequired);
    }
}
