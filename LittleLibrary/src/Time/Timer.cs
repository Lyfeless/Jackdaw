namespace LittleLib;

public class Timer {
    //! FIXME (Alex): Needs logic for looping
    LittleGame Game;

    readonly string TimeTracker;

    public readonly float Duration;
    public readonly float StartDelay;

    double StartTime;

    public Timer(LittleGame game, float duration, string? timeTracker = null, bool startFinished = false, float startDelay = 0) {
        Game = game;
        TimeTracker = timeTracker ?? string.Empty;
        Duration = duration;
        StartDelay = startDelay;
        StartTime = Milliseconds;
        if (startFinished) { Stop(); }
    }

    double Milliseconds => Game.Timers.GetTrackedTime(TimeTracker).TotalMilliseconds;

    public double ElapsedTime => Milliseconds - StartTime - StartDelay;
    public double ElapsedTimeClamped => Math.Clamp(ElapsedTime, 0, Duration);
    public float Percent => (float)(ElapsedTimeClamped / Duration);
    public bool Done => ElapsedTime >= Duration;

    public void Restart() { StartTime = Milliseconds; }
    public void Stop() { StartTime = Milliseconds - Duration - StartDelay; }
}