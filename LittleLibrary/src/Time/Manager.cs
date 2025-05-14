namespace LittleLib;

public class TimeManager(LittleGame game) {
    readonly LittleGame Game = game;

    readonly Dictionary<string, TimeTracker> trackers = [];

    public void AddTracker(string name, TimeSpan? startTime = null, bool paused = false) {
        trackers.Add(name, new(startTime != null ? (TimeSpan)startTime : Game.Time.Elapsed, paused));
    }

    public void RemoveTracker(string name) {
        trackers.Remove(name);
    }

    public void ClearTrackers() {
        trackers.Clear();
    }

    public TimeSpan GetTrackedTime(string name) {
        if (!trackers.TryGetValue(name, out TimeTracker? tracker)) { return Game.Time.Elapsed; }
        return tracker.CurrentTime;
    }

    public void SetTrackedTime(string name, TimeSpan time) {
        if (!trackers.TryGetValue(name, out TimeTracker? tracker)) { return; }
        tracker.SetTime(time);
    }

    public bool GetPaused(string name) {
        if (!trackers.TryGetValue(name, out TimeTracker? tracker)) { return false; }
        return tracker.Paused;
    }

    public void Pause(string name) => SetPaused(name, true);
    public void Unpause(string name) => SetPaused(name, false);
    public void SetPaused(string name, bool value) {
        if (!trackers.TryGetValue(name, out TimeTracker? tracker)) { return; }
        tracker.SetPaused(value);
    }

    public void Update() {
        foreach (TimeTracker tracker in trackers.Values) {
            tracker.Update(Game.Time.Delta);
        }
    }
}