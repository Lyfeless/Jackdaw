namespace LittleLib;

/// <summary>
/// A simple event manager for subscribing and dispatching events.
/// </summary>
public class EventBus {
    record class Subscription(object? Subscriber, Delegate Callback);

    readonly Dictionary<Type, List<Subscription>> Subsciptions = [];

    record struct QueueAction(Type Type, Subscription Subscription);
    bool subscriptionsLocked = false;
    readonly List<QueueAction> addQueue = [];
    readonly List<QueueAction> removeQueue = [];

    /// <summary>
    /// Subscribe to an event with a callback.
    /// </summary>
    /// <typeparam name="T">The type of event to subscribe to.</typeparam>
    /// <param name="callback">The action that should be run when an event is recieved.</param>
    public void Subscribe<T>(Action<T> callback) where T : struct => Subscribe(null, callback);

    /// <summary>
    /// Subscribe to an event with a callback.
    /// </summary>
    /// <typeparam name="T">The type of event to subscribe to.</typeparam>
    /// <param name="subscriber">The object subscribing to the event, can be used to unsubscribe with.</param>
    /// <param name="callback">The action that should be run when an event is recieved.</param>
    public void Subscribe<T>(object? subscriber, Action<T> callback) where T : struct {
        Type type = typeof(T);
        Subscription subscription = new(subscriber, callback);
        if (subscriptionsLocked) { addQueue.Add(new(type, subscription)); return; }
        Subscribe(type, subscription);
    }

    void Subscribe(Type type, Subscription subscription) {
        if (!Subsciptions.TryGetValue(type, out List<Subscription>? value)) {
            value = [];
            Subsciptions.Add(type, value);
        }

        value.Add(subscription);
    }

    /// <summary>
    /// Unsubscribe from an event by subscriber object.
    /// </summary>
    /// <typeparam name="T">The type of event to unsubscribe from.</typeparam>
    /// <param name="subscriber">The subscribed object.</param>
    public void Unsubscribe<T>(object? subscriber) where T : struct {
        Type type = typeof(T);
        if (!Subsciptions.TryGetValue(type, out List<Subscription>? value)) { return; }
        Subscription? subscription = value.Find(e => e.Subscriber == subscriber);
        if (subscription == null) { return; }

        if (subscriptionsLocked) { removeQueue.Add(new(type, subscription)); return; }
        value.Remove(subscription);
    }

    /// <summary>
    /// Unsubscribe from an event by callback.
    /// </summary>
    /// <typeparam name="T">The type of event to unsubscribe from.</typeparam>
    /// <param name="callback">The subscribed callback.</param>
    public void Unsubscribe<T>(Action<T> callback) where T : struct {
        Type type = typeof(T);
        if (!Subsciptions.TryGetValue(type, out List<Subscription>? value)) { return; }
        Subscription? subscription = value.Find(e => e.Callback == (Delegate)callback);
        if (subscription == null) { return; }

        if (subscriptionsLocked) { removeQueue.Add(new(type, subscription)); return; }
        value.Remove(subscription);
    }

    /// <summary>
    /// Send an event to all subscriptions.
    /// </summary>
    /// <typeparam name="T">The type of event to send.</typeparam>
    /// <param name="event">The event package.</param>
    public void Dispatch<T>(T @event) where T : struct {
        Type type = typeof(T);
        if (!Subsciptions.TryGetValue(type, out List<Subscription>? value)) { return; }
        subscriptionsLocked = true;
        foreach (Subscription subscription in value) {
            ((Action<T>)subscription.Callback)(@event);
        }
        subscriptionsLocked = false;
        ProcessQueue();
    }

    void ProcessQueue() {
        foreach (QueueAction action in addQueue) { Subscribe(action.Type, action.Subscription); }
        foreach (QueueAction action in removeQueue) {
            if (Subsciptions.TryGetValue(action.Type, out List<Subscription>? value)) { value.Remove(action.Subscription); }
        }

        addQueue.Clear();
        removeQueue.Clear();
    }
}