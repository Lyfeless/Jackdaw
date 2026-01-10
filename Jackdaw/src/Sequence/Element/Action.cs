namespace Jackdaw;

public class SequenceElementAction(Action action) : ISequenceElement {
    public ISequenceElementRunner GetRunner(Game game)
        => new SequenceElementActionRunner(action);
}

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