namespace Jackdaw;

/// <summary>
/// A sequence element for delaying the sequence until a condition is met on an interval.
/// </summary>
/// <param name="condition">The condition to wait until.</param>
/// <param name="interval">The amount of time to wait between checks.</param>
/// <param name="startPercent">he percent through the interval to start at when the element starts</param>
public class SequenceElementConditionIntervalWait(Func<bool> condition, TimeSpan interval, float startPercent = 0) : ISequenceElement {
    public ISequenceElementRunner GetRunner(Game game)
        => new SequenceElementConditionIntervalWaitRunner(game, condition, interval, startPercent);
}

/// <summary>
/// A runner for handling <see cref="SequenceElementConditionIntervalWait"/> elements.
/// </summary>
public class SequenceElementConditionIntervalWaitRunner : ISequenceElementRunner {
    readonly Func<bool> Condition;
    readonly TicklessTimer Timer;

    /// <summary>
    /// A runner for handling <see cref="SequenceElementConditionIntervalWait"/> elements.
    /// </summary>
    /// <param name="game">The current game instance.</param>
    /// <param name="condition">The condition to wait until.</param>
    /// <param name="interval">The amount of time to wait between checks.</param>
    /// <param name="startPercent">he percent through the interval to start at when the element starts</param>
    public SequenceElementConditionIntervalWaitRunner(Game game, Func<bool> condition, TimeSpan interval, float startPercent = 0) {
        Condition = condition;

        Timer = new(game, interval);
        Timer.Set(Timer.Duration * startPercent);
    }

    public SequenceRunnerState IsDone() {
        if (!Timer.Done) { return SequenceRunnerState.ELEMENT_INCOMPLETE; }
        if (Condition()) { return SequenceRunnerState.ELEMENT_COMPLETE; }
        Timer.Restart();
        return SequenceRunnerState.ELEMENT_INCOMPLETE;
    }

    public void Cancel() { }
}