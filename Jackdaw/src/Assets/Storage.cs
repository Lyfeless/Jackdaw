using Foster.Framework;

namespace Jackdaw;

internal class AssetStorage<T>() {
    protected readonly Dictionary<string, T> Assets = [];
    internal T Fallback;

    public void Add(string name, T asset) => Assets.Add(name, asset);
    public void AddFallback(T asset) {
        Fallback = asset;
        Assets["error"] = asset;
    }

    public T Get(string name, Type type) {
        if (Assets.TryGetValue(name, out T? output)) { return output; }
        Log.Warning($"ASSETS: Failed to find {type} \"{name}\", returning default");
        return Fallback;
    }
}