namespace LittleLib;

public class TimerComponent : Component {
    public readonly float Duration;
    public readonly float StartDelay;

    readonly string Tracker;
    double StartTime;

    public TimerComponent(
        LittleGame game,
        float duration,
        string? tracker = null,
        bool startFinished = false,
        float startDelay = 0
    ) : base(game) {
        Tracker = tracker ?? string.Empty;
        Duration = duration;
        StartDelay = startDelay;
        StartTime = Milliseconds;
        if (startFinished) { Stop(); }
    }

    double Milliseconds => Game.Timers.GetTrackedTime(Tracker).TotalMilliseconds;

    public double ElapsedTime => Milliseconds - StartTime - StartDelay;
    public double ElapsedTimeClamped => Math.Clamp(ElapsedTime, 0, Duration);
    public float Percent => (float)(ElapsedTimeClamped / Duration);
    public bool Done => ElapsedTime >= Duration;
    public void Restart() { StartTime = Milliseconds; }
    public void Stop() { StartTime = Milliseconds - Duration - StartDelay; }
}