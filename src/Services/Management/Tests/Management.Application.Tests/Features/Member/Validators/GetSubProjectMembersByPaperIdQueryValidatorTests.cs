using Management.Application.Features.Member.Queries;
using Management.Application.Tests.Common;

namespace Management.Application.Tests.Features.Member.Validators;

public sealed class GetSubProjectMembersByPaperIdQueryValidatorTests : BaseTest
{
    private readonly GetSubProjectMembersByPaperIdQueryValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldPass_WhenPaperIdIsValid()
    {
        // Arrange
        var query = new GetSubProjectMembersByPaperIdQuery(Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(query, CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenPaperIdIsEmpty()
    {
        // Arrange
        var query = new GetSubProjectMembersByPaperIdQuery(Guid.Empty);

        // Act
        var result = await _validator.ValidateAsync(query, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.PaperIdIsRequired);
    }
}
