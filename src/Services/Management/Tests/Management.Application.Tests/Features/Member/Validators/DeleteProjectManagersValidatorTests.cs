using Management.Application.Features.Member.Commands;
using Management.Application.Tests.Common;

namespace Management.Application.Tests.Features.Member.Validators;

public sealed class DeleteProjectManagersValidatorTests : BaseTest
{
    private readonly DeleteProjectManagersValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldPass_WhenCommandIsValid()
    {
        // Arrange
        var dto = new DeleteProjectManagersDto { MemberIds = new List<Guid> { Guid.NewGuid() } };
        var command = new DeleteProjectManagersCommand(Guid.NewGuid(), dto);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenProjectIdIsEmpty()
    {
        // Arrange
        var dto = new DeleteProjectManagersDto { MemberIds = new List<Guid> { Guid.NewGuid() } };
        var command = new DeleteProjectManagersCommand(Guid.Empty, dto);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.MemberProjectIdIsRequired);
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenMemberIdsIsEmpty()
    {
        // Arrange
        var dto = new DeleteProjectManagersDto { MemberIds = new List<Guid>() };
        var command = new DeleteProjectManagersCommand(Guid.NewGuid(), dto);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.MemberIdsAreRequired);
    }
}
