namespace LittleLib;

/// <summary>
/// An updateless timer with a duration.
/// </summary>
public class Timer {
    readonly LittleGame Game;

    readonly string TimeTracker;

    /// <summary>
    /// The length of the timer in milliseconds.
    /// </summary>
    public readonly float Duration;

    double StartTime;

    /// <summary>
    /// If the timer should loop when it finishes running.
    /// </summary>
    public bool Looping;

    /// <summary>
    /// An updateless timer with a duration.
    /// </summary>
    /// <param name="game">The current game instance.</param>
    /// <param name="duration">The length of the timer in milliseconds.</param>
    /// <param name="timeTracker">The tracker name to time should use. Leave empty to use real time.</param>
    /// <param name="startFinished">If the timer should be already completed when created.</param>
    /// <param name="startTime">The time offset from the timer's start to begin at in milliseconds.</param>
    /// <param name="looping">If the timer should loop when done.</param>
    public Timer(LittleGame game, float duration, string? timeTracker = null, bool startFinished = false, float startTime = 0, bool looping = false) {
        Game = game;
        TimeTracker = timeTracker ?? string.Empty;
        Duration = duration;
        Looping = looping;
        Set(startTime);
        if (startFinished) { Stop(); }
    }

    double Milliseconds => Game.Timers.GetTrackedTime(TimeTracker).TotalMilliseconds;

    /// <summary>
    /// The time since the timer started. Resets when looping.
    /// </summary>
    public double ElapsedTime {
        get {
            //! FIXME (Alex): This potentially fixes the timer looping problems, retest
            double time = Milliseconds - StartTime;
            if (Looping) { time %= Duration; }
            return time;
        }
    }

    /// <summary>
    /// The elapsed time, stopping once the time goes past the timer's duration.
    /// </summary>
    public double ElapsedTimeClamped => Math.Clamp(ElapsedTime, 0, Duration);

    /// <summary>
    /// The percent of the total duration the timer has reached.
    /// </summary>
    public float Percent => (float)(ElapsedTime / Duration);

    /// <summary>
    /// The percent of the total duration the timer has reached, stopping at 1.
    /// </summary>
    public float PercentClamped => Math.Clamp(Percent, 0, 1);

    /// <summary>
    /// If the timer has reached its duration. Will always be false when looping.
    /// </summary>
    public bool Done => ElapsedTime >= Duration;

    /// <summary>
    /// Restart the timer back to its start.
    /// </summary>
    public void Restart() => Set(0);

    /// <summary>
    /// Immediately end the timer.
    /// </summary>
    public void Stop() => Set(Duration);

    /// <summary>
    /// Offset the timer's value.
    /// </summary>
    /// <param name="amount">The amount to offset the timer's value by.</param>
    public void Change(double amount) => Set(ElapsedTime + amount);

    /// <summary>
    /// Set the timer's current time.
    /// </summary>
    /// <param name="value">The time to set the timer to.</param>
    public void Set(double value) { StartTime = Game.Timers.GetTrackedTime(TimeTracker).TotalMilliseconds - value; }
}