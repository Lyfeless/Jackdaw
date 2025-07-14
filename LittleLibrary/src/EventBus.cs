namespace LittleLib;

public class EventBus {
    Dictionary<Type, List<Delegate>> Subsciptions = [];

    public void Subscribe<T>(Action<T> callback) where T : struct {
        Type type = typeof(T);
        if (!Subsciptions.TryGetValue(type, out List<Delegate>? value)) {
            value = [];
            Subsciptions.Add(type, value);
        }

        value.Add(callback);
    }

    public void Unsubscribe<T>(Action<T> callback) where T : struct {
        Type type = typeof(T);
        if (!Subsciptions.TryGetValue(type, out List<Delegate>? value)) { return; }
        value.Remove(callback);
    }

    public void Dispatch<T>(T @event) where T : struct {
        Type type = typeof(T);
        if (!Subsciptions.TryGetValue(type, out List<Delegate>? value)) { return; }
        foreach (Delegate callback in value) {
            ((Action<T>)callback)(@event);
        }
    }
}