namespace Jackdaw;

/// <summary>
/// A sequence element for delaying the sequence for a fixed length of time.
/// </summary>
/// <param name="time">The time to wait.</param>
public class SequenceElementDelay(TimeSpan time) : ISequenceElement {
    public ISequenceElementRunner GetRunner(Game game)
        => new SequenceElementDelayRunner(game, time);
}

/// <summary>
/// A runner for handling <see cref="SequenceElementDelay"/> elements.
/// </summary>
/// <param name="game">The current game instance.</param>
/// <param name="time">The time to wait.</param>
public class SequenceElementDelayRunner(Game game, TimeSpan time) : ISequenceElementRunner {
    readonly TicklessTimer Timer = new(game, time);

    public SequenceRunnerState IsDone() => Timer.Done
        ? SequenceRunnerState.ELEMENT_COMPLETE
        : SequenceRunnerState.ELEMENT_INCOMPLETE;

    public void Cancel() { }
}