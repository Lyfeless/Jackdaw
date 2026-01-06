namespace Jackdaw;

/// <summary>
/// An updateless timer with a duration.
/// </summary>
public class TicklessTimer {
    readonly Game Game;

    readonly string TimeTracker;

    /// <summary>
    /// The length of the timer.
    /// </summary>
    public readonly TimeSpan Duration;

    TimeSpan StartTime;

    /// <summary>
    /// If the timer should loop when it finishes running.
    /// </summary>
    public bool Looping;

    /// <summary>
    /// An updateless timer with a duration.
    /// </summary>
    /// <param name="game">The current game instance.</param>
    /// <param name="duration">The length of the timer.</param>
    /// <param name="timeTracker">The tracker name to time should use. Leave empty to use real time.</param>
    /// <param name="startTime">The time offset from the timer's start to begin at.</param>
    public TicklessTimer(Game game, TimeSpan duration, string? timeTracker = null, TimeSpan? startTime = null) {
        Game = game;
        TimeTracker = timeTracker ?? string.Empty;
        Duration = duration;
        Set(startTime ?? TimeSpan.Zero);
    }

    TimeSpan TrackedTime => Game.Timers.GetTrackedTime(TimeTracker);

    /// <summary>
    /// The time since the timer started. Resets when looping.
    /// </summary>
    public TimeSpan ElapsedTime {
        get {
            TimeSpan time = TrackedTime - StartTime;
            if (Looping) { time = new(time.Ticks % Duration.Ticks); }
            return time;
        }
    }

    /// <summary>
    /// The elapsed time, stopping once the time goes past the timer's duration.
    /// </summary>
    public TimeSpan ElapsedTimeClamped => new(Math.Clamp(ElapsedTime.Ticks, 0, Duration.Ticks));

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
    public void Restart() => Set(TimeSpan.Zero);

    /// <summary>
    /// Immediately end the timer.
    /// </summary>
    public void Stop() => Set(Duration);

    /// <summary>
    /// Offset the timer's value.
    /// </summary>
    /// <param name="amount">The amount to offset the timer's value by.</param>
    public void Change(TimeSpan amount) => Set(ElapsedTime + amount);

    /// <summary>
    /// Set the timer's current time.
    /// </summary>
    /// <param name="value">The time to set the timer to.</param>
    public void Set(TimeSpan value) { StartTime = Game.Timers.GetTrackedTime(TimeTracker) - value; }
}