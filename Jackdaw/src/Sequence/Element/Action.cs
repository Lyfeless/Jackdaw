namespace Jackdaw;

/// <summary>
/// A sequence element for running an action with no delay.
/// </summary>
/// <param name="action">The action to run when the action runs in a sequence.</param>
public class SequenceElementAction(Action action) : ISequenceElement {
    public ISequenceElementRunner GetRunner(Game game)
        => new SequenceElementActionRunner(action);
}

/// <summary>
/// A runner for handling <see cref="SequenceElementAction"/> elements.
/// </summary>
/// <param name="action">The action to run when the action runs in a sequence.</param>
public class SequenceElementActionRunner(Action action) : ISequenceElementRunner {
    readonly Action Action = action;
    bool IsActionRun = false;

    public SequenceRunnerState IsDone() {
        if (!IsActionRun) {
            Action();
            IsActionRun = true;
        }
        return SequenceRunnerState.ELEMENT_COMPLETE;
    }
    public void Cancel() { }
}