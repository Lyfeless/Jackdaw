namespace Jackdaw;

/// <summary>
/// A sequence element for delaying the sequence until a condition is met.
/// </summary>
/// <param name="condition">The condition to wait until.</param>
public class SequenceElementConditionWait(Func<bool> condition) : ISequenceElement {
    public ISequenceElementRunner GetRunner(Game game)
        => new SequenceElementConditionWaitRunner(condition);
}

/// <summary>
/// A runner for handling <see cref="SequenceElementConditionWait"/> elements.
/// </summary>
/// <param name="condition">The condition to wait until.</param>
public class SequenceElementConditionWaitRunner(Func<bool> condition) : ISequenceElementRunner {
    readonly Func<bool> Condition = condition;

    public SequenceRunnerState IsDone() => Condition()
        ? SequenceRunnerState.ELEMENT_COMPLETE
        : SequenceRunnerState.ELEMENT_INCOMPLETE;

    public void Cancel() { }
}