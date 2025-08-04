namespace LittleLib;

public class Timer {
    LittleGame Game;

    readonly string TimeTracker;

    public readonly float Duration;

    double StartTime;
    bool Looping;

    public Timer(LittleGame game, float duration, string? timeTracker = null, bool startFinished = false, float startTime = 0, bool looping = false) {
        Game = game;
        TimeTracker = timeTracker ?? string.Empty;
        Duration = duration;
        Looping = looping;
        Set(startTime);
        if (startFinished) { Stop(); }
    }

    double Milliseconds {
        get {
            double millis = Game.Timers.GetTrackedTime(TimeTracker).TotalMilliseconds;
            //! FIXME (Alex): These seems to not be looping correctly in all cases
            //  Reproducing values: looping true, duration 400
            if (Looping) { millis %= Duration; }
            return millis;
        }
    }

    public double ElapsedTime => Milliseconds - StartTime;
    public double ElapsedTimeClamped => Math.Clamp(ElapsedTime, 0, Duration);
    public float Percent => (float)(ElapsedTime / Duration);
    public bool Done => ElapsedTime >= Duration;

    public void Restart() => Set(0);
    public void Stop() => Set(Duration);
    public void Change(double amount) => Set(ElapsedTime + amount);
    public void Set(double value) { StartTime = Game.Timers.GetTrackedTime(TimeTracker).TotalMilliseconds - value; }
}