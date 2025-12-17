namespace Jackdaw;

/// <summary>
/// A manager system for storing multiple time spaces, allowing different timers to be paused or scaled independently.
/// </summary>
/// <param name="game">The current game instance.</param>
public class TimeManager(Game game) {
    readonly Game Game = game;

    readonly Dictionary<string, TimeTracker> trackers = [];

    /// <summary>
    /// Add a controllable time tracker. Any modifications to this tracker will automatically affect any timers using it.
    /// </summary>
    /// <param name="name">The tracker identification name.</param>
    /// <param name="startTime">The time the tracker should consider as "now". Defaults to the current time if not provided.</param>
    /// <param name="paused">If the timer should start paused.</param>
    /// <param name="scale">The amount to scale the time tracker's speed by. Defaults to 1 if not provided.</param>
    public void AddTracker(string name, TimeSpan? startTime = null, bool paused = false, float scale = 1) {
        trackers.Add(name, new(startTime != null ? (TimeSpan)startTime : Game.Time.Elapsed, paused, scale));
    }

    /// <summary>
    /// Clear a time tracker.
    /// All timers currently using the tracker will switch back to normal time, which could cause unintended behavior.
    /// </summary>
    /// <param name="name">The tracker to remove.</param>
    public void RemoveTracker(string name) {
        trackers.Remove(name);
    }

    /// <summary>
    /// Clear all timer trackers.
    /// All timers will switch back to normal time, which could cause unintended behavior.
    /// </summary>
    public void ClearTrackers() {
        trackers.Clear();
    }

    /// <summary>
    /// Get the current time of a tracker.
    /// </summary>
    /// <param name="name">The tracker to query.</param>
    /// <returns>The current time assigned to the tracker, or the default time if no tracker is found.</returns>
    public TimeSpan GetTrackedTime(string name) {
        if (!trackers.TryGetValue(name, out TimeTracker? tracker)) { return Game.Time.Elapsed; }
        return tracker.CurrentTime;
    }

    /// <summary>
    /// Set the time for a tracker.
    /// </summary>
    /// <param name="name">The tracker to set.</param>
    /// <param name="time">The value to set the tracker's current time to.</param>
    public void SetTrackedTime(string name, TimeSpan time) {
        if (!trackers.TryGetValue(name, out TimeTracker? tracker)) { return; }
        tracker.SetTime(time);
    }

    /// <summary>
    /// Offset a atracker's current time by an amount.
    /// </summary>
    /// <param name="name">The tracker to change.</param>
    /// <param name="amount">The amount to offset the tracker's time by.</param>
    public void ChangeTrackedTime(string name, TimeSpan amount) {
        if (!trackers.TryGetValue(name, out TimeTracker? tracker)) { return; }
        tracker.SetTime(tracker.CurrentTime + amount);
    }

    /// <summary>
    /// Pause a tracker. The tracker's time won't increase until it's unpaused.
    /// </summary>
    /// <param name="name">The tracker to pause.</param>
    public void Pause(string name) => SetPaused(name, true);

    /// <summary>
    /// Unpause a tracker. If the tracker was paused, it's timer will begin increasing again.
    /// </summary>
    /// <param name="name">The tracker to unpause.</param>
    public void Unpause(string name) => SetPaused(name, false);

    /// <summary>
    /// Set a tracker's paused state. While paused, the tracker's time won't increase until it's unpaused.
    /// </summary>
    /// <param name="name">The tracker to set.</param>
    /// <param name="value">The pause state to set the tracker to.</param>
    public void SetPaused(string name, bool value) {
        if (!trackers.TryGetValue(name, out TimeTracker? tracker)) { return; }
        tracker.SetPaused(value);
    }

    /// <summary>
    /// Get a tracker's current pause state.
    /// </summary>
    /// <param name="name">The tracker to query.</param>
    /// <returns>If the tracker is currently pause, false if no tracker is found.</returns>
    public bool GetPaused(string name) {
        if (!trackers.TryGetValue(name, out TimeTracker? tracker)) { return false; }
        return tracker.Paused;
    }

    /// <summary>
    /// Reset any scaling on a tracker, causing it to run at normal speed.
    /// </summary>
    /// <param name="name">The tracker to set.</param>
    public void ResetScale(string name) => SetScale(name, 1);

    /// <summary>
    /// Set the speed scale of a tracker, causing it to run faster or slower.
    /// <param name="name">The tracker to set.</param>
    /// <param name="scale">The scale to set the tracker to.</param>
    /// </summary>
    public void SetScale(string name, float scale) {
        if (!trackers.TryGetValue(name, out TimeTracker? tracker)) { return; }
        tracker.SetScale(scale);
    }

    /// <summary>
    /// Get the current speed scale of a tracker.
    /// </summary>
    /// <param name="name">The tracker to query.</param>
    /// <returns>The current speed scale of the tracker, 1 if not tracker is found.</returns>
    public float GetScale(string name) {
        if (!trackers.TryGetValue(name, out TimeTracker? tracker)) { return 1; }
        return tracker.Scale;
    }

    public void Update() {
        foreach (TimeTracker tracker in trackers.Values) {
            tracker.Update(Game.Time.Delta);
        }
    }
}