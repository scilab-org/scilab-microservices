using Lab.Domain.Enums;

namespace Lab.Domain.Services;

/// <summary>
/// Defines the allowed state transitions for the paper submission lifecycle.
/// Lifecycle: draft → submitted → revision_required ↔ resubmitted → accepted → published
/// Terminal states: published, rejected
/// OnHold: any non-terminal status → on_hold → draft
/// </summary>
public static class PaperStatusMachine
{
    #region Fields, Properties and Indexers

    private static readonly IReadOnlyDictionary<SubmissionStatus, IReadOnlySet<SubmissionStatus>> AllowedTransitions =
        new Dictionary<SubmissionStatus, IReadOnlySet<SubmissionStatus>>
        {
            [SubmissionStatus.Draft] = new HashSet<SubmissionStatus>
            {
                SubmissionStatus.Submitted,
                SubmissionStatus.OnHold
            },
            [SubmissionStatus.Submitted] = new HashSet<SubmissionStatus>
            {
                SubmissionStatus.RevisionRequired,
                SubmissionStatus.Accepted,
                SubmissionStatus.Rejected,
                SubmissionStatus.OnHold
            },
            [SubmissionStatus.RevisionRequired] = new HashSet<SubmissionStatus>
            {
                SubmissionStatus.Resubmitted,
                SubmissionStatus.OnHold
            },
            [SubmissionStatus.Resubmitted] = new HashSet<SubmissionStatus>
            {
                SubmissionStatus.RevisionRequired,
                SubmissionStatus.Accepted,
                SubmissionStatus.Rejected,
                SubmissionStatus.OnHold
            },
            [SubmissionStatus.Accepted] = new HashSet<SubmissionStatus>
            {
                SubmissionStatus.Published,
                SubmissionStatus.OnHold
            },
            [SubmissionStatus.Published] = new HashSet<SubmissionStatus>(),
            [SubmissionStatus.Rejected] = new HashSet<SubmissionStatus>(),
            [SubmissionStatus.OnHold] = new HashSet<SubmissionStatus>
            {
                SubmissionStatus.Draft
            }
        };

    /// <summary>
    /// Transitions that only a paper author (submitter) may perform.
    /// </summary>
    private static readonly IReadOnlySet<SubmissionStatus> AuthorTransitions =
        new HashSet<SubmissionStatus>
        {
            SubmissionStatus.Submitted,
            SubmissionStatus.Resubmitted,
            SubmissionStatus.OnHold,
            SubmissionStatus.Draft
        };

    /// <summary>
    /// Transitions that require editor or reviewer-level access.
    /// </summary>
    private static readonly IReadOnlySet<SubmissionStatus> EditorTransitions =
        new HashSet<SubmissionStatus>
        {
            SubmissionStatus.RevisionRequired,
            SubmissionStatus.Accepted,
            SubmissionStatus.Rejected,
            SubmissionStatus.Published
        };

    private static readonly IReadOnlySet<SubmissionStatus> TerminalStatuses =
        new HashSet<SubmissionStatus>
        {
            SubmissionStatus.Published,
            SubmissionStatus.Rejected
        };

    /// <summary>
    /// Statuses that require a PDF file to be attached.
    /// </summary>
    private static readonly IReadOnlySet<SubmissionStatus> RequiresPdfStatuses =
        new HashSet<SubmissionStatus>
        {
            SubmissionStatus.Submitted,
            SubmissionStatus.Resubmitted
        };

    #endregion

    #region Methods

    public static bool IsTransitionAllowed(SubmissionStatus from, SubmissionStatus to)
    {
        return AllowedTransitions.TryGetValue(from, out var allowed) && allowed.Contains(to);
    }

    public static bool IsTerminal(SubmissionStatus status) =>
        TerminalStatuses.Contains(status);

    public static bool RequiresPdf(SubmissionStatus targetStatus) =>
        RequiresPdfStatuses.Contains(targetStatus);

    /// <summary>
    /// Returns true when moving to <paramref name="targetStatus"/> requires the actor
    /// to hold a paper-author role (submit / resubmit actions).
    /// </summary>
    public static bool RequiresAuthorRole(SubmissionStatus targetStatus) =>
        AuthorTransitions.Contains(targetStatus);

    /// <summary>
    /// Returns true when moving to <paramref name="targetStatus"/> requires the actor
    /// to hold a reviewer or editor-level role.
    /// </summary>
    public static bool RequiresEditorRole(SubmissionStatus targetStatus) =>
        EditorTransitions.Contains(targetStatus);

    public static IReadOnlySet<SubmissionStatus> GetAllowedTransitions(SubmissionStatus from) =>
        AllowedTransitions.TryGetValue(from, out var allowed)
            ? allowed
            : new HashSet<SubmissionStatus>();

    #endregion
}
