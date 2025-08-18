namespace LittleLib;

/// <summary>
/// A wrapper for the timer object to automatically run actions when finished.
/// </summary>
/// <param name="game">The current game instance.</param>
/// <param name="duration">The timer's duration in milliseconds.</param>
/// <param name="callback">The action to run when the timer finishes.</param>
/// <param name="timeTracker">The tracker to use for the current time. Defaults to normal time.</param>
/// <param name="startFinished">If the timer should start finished.</param>
/// <param name="startTime">The time offset the timer should begin with.</param>
public class TimerComponent(LittleGame game, float duration, Action callback, string? timeTracker = null, bool startFinished = false, float startTime = 0) : Component(game) {
    //! FIXME (Alex): Should maybe clean up the timer constructor to take in less info by default?
    readonly Timer Timer = new(game, duration, timeTracker, startFinished, startTime);
    readonly Action Callback = callback;

    /// <summary>
    /// If the timer should reset when finished.
    /// </summary>
    public bool LoopOnFinish = false;

    /// <summary>
    /// If the timer should remove itsself from its owner when finished.
    /// </summary>
    public bool RemoveOnFinish = false;

    /// <summary>
    /// A wrapper for the timer object to automatically run actions when finished.
    /// </summary>
    /// <param name="game">The current game instance.</param>
    /// <param name="duration">The timer's duration in milliseconds.</param>
    /// <param name="timeTracker">The tracker to use for the current time. Defaults to normal time.</param>
    /// <param name="startFinished">If the timer should start finished.</param>
    /// <param name="startTime">The time offset the timer should begin with.</param>
    public TimerComponent(LittleGame game, float duration, string? timeTracker = null, bool startFinished = false, float startTime = 0) : this(game, duration, () => { }, timeTracker, startFinished, startTime) { }

    /// <summary>
    /// The time since the timer started. Resets when looping.
    /// </summary>
    public double ElapsedTime => Timer.ElapsedTime;

    /// <summary>
    /// The elapsed time, stopping once the time goes past the timer's duration.
    /// </summary>
    public double ElapsedTimeClamped => Timer.ElapsedTimeClamped;

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

        Callback();

        if (RemoveOnFinish) { Actor.Components.Remove(this); }
        else if (LoopOnFinish) { Timer.Restart(); }
        else { Ticking = false; }
    }
}