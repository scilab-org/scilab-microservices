using Management.Application.Features.Member.Commands;
using Management.Application.Tests.Common;

namespace Management.Application.Tests.Features.Member.Validators;

public sealed class AddSubProjectMembersValidatorTests : BaseTest
{
    private readonly AddSubProjectMembersValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldPass_WhenCommandIsValid()
    {
        // Arrange
        var dto = new AddProjectMembersDto
        {
            Members = new List<ProjectMemberEntry>
            {
                new() { UserId = Guid.NewGuid(), GroupName = AuthorizeConstants.PaperMember }
            }
        };
        var command = new AddSubProjectMembersCommand(Guid.NewGuid(), dto);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenSubProjectIdIsEmpty()
    {
        // Arrange
        var dto = new AddProjectMembersDto
        {
            Members = new List<ProjectMemberEntry>
            {
                new() { UserId = Guid.NewGuid() }
            }
        };
        var command = new AddSubProjectMembersCommand(Guid.Empty, dto);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.MemberProjectIdIsRequired);
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
        var command = new AddSubProjectMembersCommand(Guid.NewGuid(), dto);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.UserIdsAreRequired);
    }
}
