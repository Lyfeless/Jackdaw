namespace LittleLib;

internal class TimeTracker(TimeSpan startTime, bool paused = false) {
    public TimeSpan CurrentTime { get; private set; } = startTime;
    public bool Paused { get; private set; } = paused;
    public float Scale { get; private set; } = 1;

    public void Update(float delta) {
        if (Paused) { return; }

        CurrentTime += TimeSpan.FromSeconds(delta) * Scale;
    }

    public void SetTime(TimeSpan time) {
        CurrentTime = time;
    }

    public void SetPaused(bool value) {
        Paused = value;
    }

    public void SetScale(float scale) {
        Scale = scale;
    }
}