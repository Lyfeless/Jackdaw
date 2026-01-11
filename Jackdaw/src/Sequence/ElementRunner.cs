namespace Jackdaw;

/// <summary>
/// An interface for running sequence elements during execution.
/// Runner logic is seperated from elements to allow sequences to be rerun.
/// </summary>
public interface ISequenceElementRunner {
    /// <summary>
    /// Updates the runner and gets the state to determine if the sequence playback can move on.
    /// </summary>
    /// <returns>The state of the current runner.</returns>
    public SequenceRunnerState IsDone();

    /// <summary>
    /// Performs any cleanup in the case the runner is cancelled mid-execution.
    /// </summary>
    public void Cancel();
}