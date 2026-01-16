namespace Jackdaw;

/// <summary>
/// A wrapper for the timer object to automatically run actions when finished.
/// </summary>
/// <param name="game">The current game instance.</param>
/// <param name="duration">The timer's duration.</param>
/// <param name="timeTracker">The tracker to use for the current time. Defaults to normal time.</param>
/// <param name="startTime">The time offset the timer should begin with.</param>
public class TimerComponent(Game game, TimeSpan duration, string? timeTracker = null, TimeSpan? startTime = null) : Component(game) {
    readonly TicklessTimer Timer = new(game, duration, timeTracker, startTime);

    /// <summary>
    /// If the timer should reset when finished.
    /// </summary>
    public bool LoopOnFinish = false;

    /// <summary>
    /// If the timer should remove itsself from its owner when finished.
    /// </summary>
    public bool RemoveOnFinish = false;

    /// <summary>
    /// The action to run when the timer finishes or loops.
    /// </summary>
    public Action? Callback;

    /// <summary>
    /// A wrapper for the timer object to automatically run actions when finished.
    /// </summary>
    /// <param name="game">The current game instance.</param>
    /// <param name="duration">The timer's duration.</param>
    /// <param name="callback">The action to run when the timer finishes or loops.</param>
    /// <param name="timeTracker">The tracker to use for the current time. Defaults to normal time.</param>
    /// <param name="startTime">The time offset the timer should begin with.</param>
    public TimerComponent(Game game, TimeSpan duration, Action callback, string? timeTracker = null, TimeSpan? startTime = null) : this(game, duration, timeTracker, startTime) {
        Callback = callback;
    }

    /// <summary>
    /// The time since the timer started. Resets when looping.
    /// </summary>
    public TimeSpan ElapsedTime => Timer.ElapsedTime;

    /// <summary>
    /// The elapsed time, stopping once the time goes past the timer's duration.
    /// </summary>
    public TimeSpan ElapsedTimeClamped => Timer.ElapsedTimeClamped;

    /// <summary>
    /// The percent of the total duration the timer has reached.
    /// </summary>
    public float Percent => Timer.Percent;

    /// <summary>
    /// The percent of the total duration the timer has reached, stopping at 1.
    /// </summary>
    public float PercentClamped => Timer.PercentClamped;

    /// <summary>
    /// If the timer has reached its duration. Will always be false when looping.
    /// </summary>
    public bool Done => Timer.Done;

    /// <summary>
    /// Restart the timer back to its start.
    /// </summary>
    public void Restart() {
        Ticking = true;
        Timer.Restart();
    }

    /// <summary>
    /// Immediately end the timer.
    /// </summary>
    public void Stop() {
        Ticking = false;
        Timer.Stop();
    }

    protected override void Update() {
        if (!Timer.Done) { return; }

        Callback?.Invoke();

        if (RemoveOnFinish) { Actor.Components.Remove(this); }
        else if (LoopOnFinish) { Timer.Restart(); }
        else { Ticking = false; }
    }
}