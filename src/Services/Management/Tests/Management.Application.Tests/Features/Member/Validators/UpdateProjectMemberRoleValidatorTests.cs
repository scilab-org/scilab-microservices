using Management.Application.Features.Member.Commands;
using Management.Application.Tests.Common;

namespace Management.Application.Tests.Features.Member.Validators;

public sealed class UpdateProjectMemberRoleValidatorTests : BaseTest
{
    private readonly UpdateProjectMemberRoleValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldPass_WhenCommandIsValid()
    {
        // Arrange
        var dto = new UpdateProjectMemberRoleDto { MemberId = Guid.NewGuid(), ProjectRole = "project:member" };
        var command = new UpdateProjectMemberRoleCommand(Guid.NewGuid(), dto, Guid.NewGuid().ToString());

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenProjectIdIsEmpty()
    {
        // Arrange
        var dto = new UpdateProjectMemberRoleDto { MemberId = Guid.NewGuid(), ProjectRole = "project:member" };
        var command = new UpdateProjectMemberRoleCommand(Guid.Empty, dto, Guid.NewGuid().ToString());

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.MemberProjectIdIsRequired);
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenMemberIdIsEmpty()
    {
        // Arrange
        var dto = new UpdateProjectMemberRoleDto { MemberId = Guid.Empty, ProjectRole = "project:member" };
        var command = new UpdateProjectMemberRoleCommand(Guid.NewGuid(), dto, Guid.NewGuid().ToString());

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.MemberIdsAreRequired);
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenProjectRoleIsEmpty()
    {
        // Arrange
        var dto = new UpdateProjectMemberRoleDto { MemberId = Guid.NewGuid(), ProjectRole = string.Empty };
        var command = new UpdateProjectMemberRoleCommand(Guid.NewGuid(), dto, Guid.NewGuid().ToString());

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.GroupNameIsRequired);
    }
}
