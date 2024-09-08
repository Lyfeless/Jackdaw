using Foster.Framework;

namespace LittleLib;

public static class TimeManager {
    static readonly Dictionary<string, TimeTracker> trackers = [];

    public static void AddTracker(string name, TimeSpan? startTime = null, bool paused = false) {
        trackers.Add(name, new(startTime != null ? (TimeSpan)startTime : Time.Now, paused));
    }

    public static void RemoveTracker(string name) {
        trackers.Remove(name);
    }

    public static void ClearTrackers() {
        trackers.Clear();
    }

    public static TimeSpan GetTrackedTime(string name) {
        if (!trackers.TryGetValue(name, out TimeTracker? tracker)) { return Time.Now; }
        return tracker.CurrentTime;
    }

    public static void SetTrackedTime(string name, TimeSpan time) {
        if (!trackers.TryGetValue(name, out TimeTracker? tracker)) { return; }
        tracker.SetTime(time);
    }

    public static bool GetPaused(string name) {
        if (!trackers.TryGetValue(name, out TimeTracker? tracker)) { return false; }
        return tracker.Paused;
    }

    public static void Pause(string name) => SetPaused(name, true);
    public static void Unpause(string name) => SetPaused(name, false);
    public static void SetPaused(string name, bool value) {
        if (!trackers.TryGetValue(name, out TimeTracker? tracker)) { return; }
        tracker.SetPaused(value);
    }

    public static void Update() {
        foreach (TimeTracker tracker in trackers.Values) {
            tracker.Update();
        }
    }
}