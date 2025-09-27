namespace Jackdaw;

internal class TimeTracker(TimeSpan startTime, bool paused = false, float scale = 1) {
    public TimeSpan CurrentTime { get; private set; } = startTime;
    public bool Paused { get; private set; } = paused;
    public float Scale { get; private set; } = scale;

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