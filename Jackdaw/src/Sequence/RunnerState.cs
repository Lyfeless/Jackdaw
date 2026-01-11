namespace Jackdaw;

/// <summary>
/// The state a sequence element can be when checking for complete.
/// </summary>
public enum SequenceRunnerState {
    /// <summary>
    /// The element is complete and can move on to the next.
    /// </summary>
    ELEMENT_COMPLETE,

    /// <summary>
    /// The element isn't done yet.
    /// </summary>
    ELEMENT_INCOMPLETE,

    /// <summary>
    /// The sequence should cancel.
    /// </summary>
    CANCEL
}