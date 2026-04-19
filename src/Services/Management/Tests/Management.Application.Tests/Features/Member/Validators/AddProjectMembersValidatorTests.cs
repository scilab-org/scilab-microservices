using Management.Application.Features.Member.Commands;
using Management.Application.Tests.Common;

namespace Management.Application.Tests.Features.Member.Validators;

public sealed class AddProjectMembersValidatorTests : BaseTest
{
    private readonly AddProjectMembersValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldPass_WhenCommandIsValid()
    {
        // Arrange
        var dto = new AddProjectMembersDto
        {
            Members = new List<ProjectMemberEntry>
            {
                new() { UserId = Guid.NewGuid(), GroupName = AuthorizeConstants.ProjectMember }
            }
        };
        var command = new AddProjectMembersCommand(Guid.NewGuid(), dto, Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenProjectIdIsEmpty()
    {
        // Arrange
        var dto = new AddProjectMembersDto
        {
            Members = new List<ProjectMemberEntry>
            {
                new() { UserId = Guid.NewGuid() }
            }
        };
        var command = new AddProjectMembersCommand(Guid.Empty, dto, Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.MemberProjectIdIsRequired);
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenMembersListIsEmpty()
    {
        // Arrange
        var dto = new AddProjectMembersDto { Members = new List<ProjectMemberEntry>() };
        var command = new AddProjectMembersCommand(Guid.NewGuid(), dto, Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.UserIdsAreRequired);
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenMemberUserIdIsEmpty()
    {
        // Arrange
        var dto = new AddProjectMembersDto
        {
            Members = new List<ProjectMemberEntry>
            {
                new() { UserId = Guid.Empty }
            }
        };
        var command = new AddProjectMembersCommand(Guid.NewGuid(), dto, Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.UserIdsAreRequired);
    }
}
