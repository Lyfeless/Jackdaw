namespace LittleLib;

public class Timer {
    LittleGame Game;

    readonly string TimeTracker;

    public readonly float Duration;
    public readonly float StartDelay;

    double StartTime;
    bool Looping;

    public Timer(LittleGame game, float duration, string? timeTracker = null, bool startFinished = false, float startDelay = 0, bool looping = false) {
        Game = game;
        TimeTracker = timeTracker ?? string.Empty;
        Duration = duration;
        StartDelay = startDelay;
        StartTime = Milliseconds;
        Looping = looping;
        if (startFinished) { Stop(); }
    }

    double Milliseconds {
        get {
            double millis = Game.Timers.GetTrackedTime(TimeTracker).TotalMilliseconds;
            //! FIXME (Alex): These seems to not be looping correctly in all calses
            //  Reproducing values: looping true, duration 400
            if (Looping) { millis %= Duration; }
            return millis;
        }
    }

    public double ElapsedTime => Milliseconds - StartTime - StartDelay;
    public double ElapsedTimeClamped => Math.Clamp(ElapsedTime, 0, Duration);
    public float Percent => (float)(ElapsedTime / Duration);
    public bool Done => ElapsedTime >= Duration;

    public void Restart() { StartTime = Milliseconds; }
    public void Stop() { StartTime = Milliseconds - Duration - StartDelay; }
}