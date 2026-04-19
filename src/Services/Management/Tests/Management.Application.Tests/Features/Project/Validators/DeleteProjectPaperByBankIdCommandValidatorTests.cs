using Management.Application.Features.Project.Commands;
using Management.Application.Tests.Common;

namespace Management.Application.Tests.Features.Project.Validators;

public sealed class DeleteProjectPaperByBankIdCommandValidatorTests : BaseTest
{
    private readonly DeleteProjectPaperByBankIdCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldPass_WhenCommandIsValid()
    {
        // Arrange
        var command = new DeleteProjectPaperByBankIdCommand(Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenPaperBankIdIsEmpty()
    {
        // Arrange
        var command = new DeleteProjectPaperByBankIdCommand(Guid.Empty);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == MessageCode.PaperIdIsRequired);
    }
}
