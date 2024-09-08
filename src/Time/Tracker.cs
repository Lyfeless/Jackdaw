using Foster.Framework;

namespace LittleLib;

internal class TimeTracker(TimeSpan startTime, bool paused = false) {
    public TimeSpan CurrentTime { get; private set; } = startTime;
    public bool Paused { get; private set; } = paused;

    public void Update() {
        if (Paused) { return; }

        CurrentTime += TimeSpan.FromSeconds(Time.Delta);
    }

    public void SetTime(TimeSpan time) {
        CurrentTime = time;
    }

    public void SetPaused(bool value) {
        Paused = value;
    }
}