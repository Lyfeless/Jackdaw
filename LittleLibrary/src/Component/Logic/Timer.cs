namespace LittleLib;

public class TimerComponent(LittleGame game, float duration, Action callback, string? timeTracker = null, bool startFinished = false, float startDelay = 0) : Component(game) {
    //! FIXME (Alex): Not the prettiest implementation but ig it works
    //! FIXME (Alex): Should maybe clean up the timer constructor to take in less info by default?
    readonly Timer Timer = new(game, duration, timeTracker, startFinished, startDelay);
    readonly Action Callback = callback;

    public bool LoopOnFinish = false;
    public bool RemoveOnFinish = false;

    public TimerComponent(LittleGame game, float duration, string? timeTracker = null, bool startFinished = false, float startDelay = 0) : this(game, duration, () => { }, timeTracker, startFinished, startDelay) { }

    public double ElapsedTime => Timer.ElapsedTime;
    public double ElapsedTimeClamped => Timer.ElapsedTimeClamped;
    public float Percent => Timer.Percent;
    public bool Done => Timer.Done;

    public void Restart() => Timer.Restart();
    public void Stop() => Timer.Stop();

    public override void Update() {
        if (!Timer.Done) { return; }

        Ticking = false;
        Callback();

        if (RemoveOnFinish) { Actor.Components.Remove(this); }
        else if (LoopOnFinish) { Timer.Restart(); }
    }
}