using Management.Application.Features.Project.Commands;
using Management.Application.Tests.Common;

namespace Management.Application.Tests.Features.Project.Validators;

public sealed class DeleteProjectConferenceJournalsValidatorTests : BaseTest
{
    private readonly DeleteProjectConferenceJournalsValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldPass_WhenCommandIsValid()
    {
        // Arrange
        var command = new DeleteProjectConferenceJournalsCommand(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenProjectIdIsEmpty()
    {
        // Arrange
        var command = new DeleteProjectConferenceJournalsCommand(Guid.Empty, Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.ProjectIdIsRequired);
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenJournalIdIsEmpty()
    {
        // Arrange
        var command = new DeleteProjectConferenceJournalsCommand(Guid.NewGuid(), Guid.Empty);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.ConferenceJournalIdIsRequired);
    }
}
