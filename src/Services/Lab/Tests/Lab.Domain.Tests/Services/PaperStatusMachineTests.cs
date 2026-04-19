namespace Lab.Domain.Tests.Services;

public sealed class PaperStatusMachineTests
{
    #region IsTransitionAllowed

    [Theory]
    [InlineData(SubmissionStatus.Draft, SubmissionStatus.Submitted, true)]
    [InlineData(SubmissionStatus.Draft, SubmissionStatus.OnHold, true)]
    [InlineData(SubmissionStatus.Draft, SubmissionStatus.Accepted, false)]
    [InlineData(SubmissionStatus.Draft, SubmissionStatus.Rejected, false)]
    [InlineData(SubmissionStatus.Submitted, SubmissionStatus.RevisionRequired, true)]
    [InlineData(SubmissionStatus.Submitted, SubmissionStatus.Accepted, true)]
    [InlineData(SubmissionStatus.Submitted, SubmissionStatus.Rejected, true)]
    [InlineData(SubmissionStatus.Submitted, SubmissionStatus.OnHold, true)]
    [InlineData(SubmissionStatus.Submitted, SubmissionStatus.Published, false)]
    [InlineData(SubmissionStatus.RevisionRequired, SubmissionStatus.Resubmitted, true)]
    [InlineData(SubmissionStatus.RevisionRequired, SubmissionStatus.OnHold, true)]
    [InlineData(SubmissionStatus.RevisionRequired, SubmissionStatus.Accepted, false)]
    [InlineData(SubmissionStatus.Resubmitted, SubmissionStatus.RevisionRequired, true)]
    [InlineData(SubmissionStatus.Resubmitted, SubmissionStatus.Accepted, true)]
    [InlineData(SubmissionStatus.Resubmitted, SubmissionStatus.Rejected, true)]
    [InlineData(SubmissionStatus.Resubmitted, SubmissionStatus.OnHold, true)]
    [InlineData(SubmissionStatus.Resubmitted, SubmissionStatus.Published, false)]
    [InlineData(SubmissionStatus.Accepted, SubmissionStatus.Published, true)]
    [InlineData(SubmissionStatus.Accepted, SubmissionStatus.OnHold, true)]
    [InlineData(SubmissionStatus.Accepted, SubmissionStatus.Rejected, false)]
    [InlineData(SubmissionStatus.Published, SubmissionStatus.Draft, false)]
    [InlineData(SubmissionStatus.Published, SubmissionStatus.Rejected, false)]
    [InlineData(SubmissionStatus.Rejected, SubmissionStatus.Draft, false)]
    [InlineData(SubmissionStatus.Rejected, SubmissionStatus.Submitted, false)]
    [InlineData(SubmissionStatus.OnHold, SubmissionStatus.Draft, true)]
    [InlineData(SubmissionStatus.OnHold, SubmissionStatus.Submitted, false)]
    public void IsTransitionAllowed_ShouldReturnExpectedResult(SubmissionStatus from, SubmissionStatus to, bool expected)
    {
        PaperStatusMachine.IsTransitionAllowed(from, to).Should().Be(expected);
    }

    [Fact]
    public void IsTransitionAllowed_ShouldReturnFalse_ForUnknownFromStatus()
    {
        PaperStatusMachine.IsTransitionAllowed((SubmissionStatus)999, SubmissionStatus.Draft).Should().BeFalse();
    }

    #endregion

    #region IsTerminal

    [Theory]
    [InlineData(SubmissionStatus.Published, true)]
    [InlineData(SubmissionStatus.Rejected, true)]
    [InlineData(SubmissionStatus.Draft, false)]
    [InlineData(SubmissionStatus.Submitted, false)]
    [InlineData(SubmissionStatus.Accepted, false)]
    [InlineData(SubmissionStatus.OnHold, false)]
    public void IsTerminal_ShouldReturnExpectedResult(SubmissionStatus status, bool expected)
    {
        PaperStatusMachine.IsTerminal(status).Should().Be(expected);
    }

    #endregion

    #region RequiresPdf

    [Theory]
    [InlineData(SubmissionStatus.Submitted, true)]
    [InlineData(SubmissionStatus.Resubmitted, true)]
    [InlineData(SubmissionStatus.Draft, false)]
    [InlineData(SubmissionStatus.Accepted, false)]
    [InlineData(SubmissionStatus.Published, false)]
    public void RequiresPdf_ShouldReturnExpectedResult(SubmissionStatus status, bool expected)
    {
        PaperStatusMachine.RequiresPdf(status).Should().Be(expected);
    }

    #endregion

    #region RequiresAuthorRole

    [Theory]
    [InlineData(SubmissionStatus.Submitted, true)]
    [InlineData(SubmissionStatus.Resubmitted, true)]
    [InlineData(SubmissionStatus.OnHold, true)]
    [InlineData(SubmissionStatus.Draft, true)]
    [InlineData(SubmissionStatus.Accepted, false)]
    [InlineData(SubmissionStatus.Published, false)]
    [InlineData(SubmissionStatus.Rejected, false)]
    [InlineData(SubmissionStatus.RevisionRequired, false)]
    public void RequiresAuthorRole_ShouldReturnExpectedResult(SubmissionStatus status, bool expected)
    {
        PaperStatusMachine.RequiresAuthorRole(status).Should().Be(expected);
    }

    #endregion

    #region RequiresEditorRole

    [Theory]
    [InlineData(SubmissionStatus.RevisionRequired, true)]
    [InlineData(SubmissionStatus.Accepted, true)]
    [InlineData(SubmissionStatus.Rejected, true)]
    [InlineData(SubmissionStatus.Published, true)]
    [InlineData(SubmissionStatus.Submitted, false)]
    [InlineData(SubmissionStatus.Resubmitted, false)]
    [InlineData(SubmissionStatus.Draft, false)]
    [InlineData(SubmissionStatus.OnHold, false)]
    public void RequiresEditorRole_ShouldReturnExpectedResult(SubmissionStatus status, bool expected)
    {
        PaperStatusMachine.RequiresEditorRole(status).Should().Be(expected);
    }

    #endregion

    #region GetAllowedTransitions

    [Fact]
    public void GetAllowedTransitions_Draft_ShouldReturnSubmittedAndOnHold()
    {
        var transitions = PaperStatusMachine.GetAllowedTransitions(SubmissionStatus.Draft);
        transitions.Should().Contain(SubmissionStatus.Submitted);
        transitions.Should().Contain(SubmissionStatus.OnHold);
        transitions.Should().HaveCount(2);
    }

    [Fact]
    public void GetAllowedTransitions_Published_ShouldReturnEmpty()
    {
        var transitions = PaperStatusMachine.GetAllowedTransitions(SubmissionStatus.Published);
        transitions.Should().BeEmpty();
    }

    [Fact]
    public void GetAllowedTransitions_Rejected_ShouldReturnEmpty()
    {
        var transitions = PaperStatusMachine.GetAllowedTransitions(SubmissionStatus.Rejected);
        transitions.Should().BeEmpty();
    }

    [Fact]
    public void GetAllowedTransitions_UnknownStatus_ShouldReturnEmpty()
    {
        var transitions = PaperStatusMachine.GetAllowedTransitions((SubmissionStatus)999);
        transitions.Should().BeEmpty();
    }

    [Fact]
    public void GetAllowedTransitions_OnHold_ShouldReturnDraft()
    {
        var transitions = PaperStatusMachine.GetAllowedTransitions(SubmissionStatus.OnHold);
        transitions.Should().ContainSingle().Which.Should().Be(SubmissionStatus.Draft);
    }

    #endregion
}
