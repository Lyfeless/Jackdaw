namespace Jackdaw;

public class SequenceElementDelay(TimeSpan time) : ISequenceElement {
    public ISequenceElementRunner GetRunner(Game game)
        => new SequenceElementDelayRunner(game, time);
}

public class SequenceElementDelayRunner(Game game, TimeSpan time) : ISequenceElementRunner {
    readonly TicklessTimer Timer = new(game, time);

    public SequenceRunnerState IsDone() => Timer.Done
        ? SequenceRunnerState.ELEMENT_COMPLETE
        : SequenceRunnerState.ELEMENT_INCOMPLETE;

    public void Cancel() { }
}