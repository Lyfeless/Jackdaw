using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Interface for a container that holds loaded assets.
/// Can be used to make custtom accessors for assets when needed. <br/>
/// Any asset that doesn't need additional logic after loading is will automatically use a default storage system.
/// </summary>
public interface IAssetStorage {
    /// <summary>
    /// Add an asset to the storage.
    /// </summary>
    /// <param name="name">The asset's name id.</param>
    /// <param name="asset">The asset to store.</param>
    public void Add(string name, object asset);

    /// <summary>
    /// Get an asset from storage.
    /// </summary>
    /// <param name="name">The asset's name id.</param>
    /// <returns>The asset, or a fallback if no asset with the given name id exists.</returns>
    public object Get(string name);

    /// <summary>
    /// Set the fallback value that should be returned if an asset lookup fails.
    /// </summary>
    /// <param name="asset">The asset to return as a fallback.</param>
    public void SetFallback(object asset);

    /// <summary>
    /// Get the fallback asset.
    /// </summary>
    /// <returns>The fallback asset.</returns>
    public object GetFallback();
}

internal class AssetStorage(Type type) : IAssetStorage {
    readonly Type Type = type;
    readonly Dictionary<string, object> Assets = [];
    object? Fallback = null;

    public void Add(string name, object asset) => Assets.Add(name, asset);

    public object Get(string name) {
        if (Assets.TryGetValue(name, out object? output)) { return output; }
        Log.Warning($"ASSETS: Failed to find {Type} \"{name}\", returning default");
        return GetFallback();
    }

    public void SetFallback(object asset) {
        Fallback = asset;
        Assets["error"] = asset;
    }

    public object GetFallback() {
        if (Fallback == null) { throw new NullReferenceException($"Attempting to access fallback where none was assigned for type {Type}."); }
        return Fallback;
    }
}