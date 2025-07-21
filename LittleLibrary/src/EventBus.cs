namespace LittleLib;

//! FIXME (Alex): TEST THIS!!! TEST IT!!!

public class EventBus {
    record class Subscription(object? Subscriber, Delegate Callback);

    readonly Dictionary<Type, List<Subscription>> Subsciptions = [];

    record struct QueueAction(Type Type, Subscription Subscription);
    bool subscriptionsLocked = false;
    readonly List<QueueAction> addQueue = [];
    readonly List<QueueAction> removeQueue = [];

    public void Subscribe<T>(Action<T> callback) where T : struct => Subscribe(null, callback);
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

    public void Unsubscribe<T>(object? subscriber) where T : struct {
        Type type = typeof(T);
        if (!Subsciptions.TryGetValue(type, out List<Subscription>? value)) { return; }
        Subscription? subscription = value.Find(e => e.Subscriber == subscriber);
        if (subscription == null) { return; }

        if (subscriptionsLocked) { removeQueue.Add(new(type, subscription)); return; }
        value.Remove(subscription);
    }

    public void Unsubscribe<T>(Action<T> callback) where T : struct {
        Type type = typeof(T);
        if (!Subsciptions.TryGetValue(type, out List<Subscription>? value)) { return; }
        Subscription? subscription = value.Find(e => e.Callback == (Delegate)callback);
        if (subscription == null) { return; }

        if (subscriptionsLocked) { removeQueue.Add(new(type, subscription)); return; }
        value.Remove(subscription);
    }

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