namespace Jackdaw;

public class SequenceElementConditionWait(Func<bool> condition) : ISequenceElement {
    public ISequenceElementRunner GetRunner(Game game)
        => new SequenceElementConditionWaitRunner(condition);
}

public class SequenceElementConditionWaitRunner(Func<bool> condition) : ISequenceElementRunner {
    readonly Func<bool> Condition = condition;

    public SequenceRunnerState IsDone() => Condition()
        ? SequenceRunnerState.ELEMENT_COMPLETE
        : SequenceRunnerState.ELEMENT_INCOMPLETE;

    public void Cancel() { }
}