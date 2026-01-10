namespace Jackdaw;

public class SequenceElementConditionIntervalWait(Func<bool> condition, TimeSpan interval, bool startFinished = false) : ISequenceElement {
    public ISequenceElementRunner GetRunner(Game game)
        => new SequenceElementConditionIntervalWaitRunner(game, condition, interval, startFinished);
}

public class SequenceElementConditionIntervalWaitRunner : ISequenceElementRunner {
    readonly Func<bool> Condition;
    readonly TicklessTimer Timer;

    public SequenceElementConditionIntervalWaitRunner(Game game, Func<bool> condition, TimeSpan interval, bool startFinished = false) {
        Condition = condition;

        Timer = new(game, interval);
        if (startFinished) { Timer.Stop(); }
    }

    public SequenceRunnerState IsDone() {
        if (!Timer.Done) { return SequenceRunnerState.ELEMENT_INCOMPLETE; }
        if (Condition()) { return SequenceRunnerState.ELEMENT_COMPLETE; }
        Timer.Restart();
        return SequenceRunnerState.ELEMENT_INCOMPLETE;
    }

    public void Cancel() { }
}