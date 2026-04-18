using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Manager for loading and unloading collections of assets.
/// </summary>
public class AssetCollectionPipeline {
    readonly Assets Assets;

    readonly Dictionary<string, AssetCollection> Collections = [];

    internal AssetCollectionPipeline(Assets assets, AssetCollectionBuilderContainer builder) {
        Assets = assets;

        AssetProviderItem[] loadOptions = assets.Loaders.GetLoadOptions();

        Collections = builder.FilterAll(assets, loadOptions);
    }

    /// <summary>
    /// Load a collection of assets.
    /// </summary>
    /// <param name="name">The collection to load.</param>
    public void Load(string name) {
        if (!Collections.TryGetValue(name, out AssetCollection collection)) {
            Log.Warning($"Asset Loader: no matching collection {name} exists, skipping load.");
            return;
        }

        Assets.Loaders.RunLoad(collection);
    }

    /// <summary>
    /// Add a collection to the async load queue.
    /// </summary>
    /// <param name="name">The collection to load.</param>
    public void LoadAsync(string name) {
        if (!Collections.TryGetValue(name, out AssetCollection collection)) {
            Log.Warning($"Asset Loader: no matching collection {name} exists, skipping load.");
            return;
        }

        Assets.Loaders.RunLoadAsync(collection);
    }

    /// <summary>
    /// Unload a collection of assets.
    /// </summary>
    /// <param name="name">The collection to unload.</param>
    public void Unload(string name) {
        if (!Collections.TryGetValue(name, out AssetCollection collection)) {
            Log.Warning($"Asset Loader: no matching collection {name} exists, skipping unload.");
            return;
        }

        Assets.Loaders.RunUnload(collection);
    }

    /// <summary>
    /// Add a collection to the async unload queue.
    /// </summary>
    /// <param name="name">The collection to unload.</param>
    public void UnloadAsync(string name) {
        if (!Collections.TryGetValue(name, out AssetCollection collection)) {
            Log.Warning($"Asset Loader: no matching collection {name} exists, skipping unload.");
            return;
        }

        Assets.Loaders.RunUnloadAsync(collection);
    }

    /// <summary>
    /// If the given asset collection is fully loaded.
    /// </summary>
    /// <param name="name">The collection to check.</param>
    /// <returns>If the asset collection is loaded.</returns>
    public bool IsLoaded(string name) => Assets.Loaders.IsLoaded(name);

    /// <summary>
    /// If the given asset collection is in the async load queue.
    /// </summary>
    /// <param name="name">The collection to check.</param>
    /// <returns>If the asset collection is being loaded.</returns>
    public bool IsLoading(string name) => Assets.Loaders.IsLoading(name);

    /// <summary>
    /// If the given asset collection is already loaded or in the async load queue.
    /// </summary>
    /// <param name="name">The collection to check.</param>
    /// <returns>If the asset collection is being loaded or is done loading.</returns>
    public bool IsLoadedOrLoading(string name) => Assets.Loaders.IsLoadedOrLoading(name);

    /// <summary>
    /// If the asset collection is not loaded.
    /// </summary>
    /// <param name="name">The collection to check.</param>
    /// <returns>If the asset collection is unloaded</returns>
    public bool IsUnloaded(string name) => Assets.Loaders.IsUnloaded(name);

    /// <summary>
    /// If the asset collection is in the async unload queue.
    /// </summary>
    /// <param name="name">The collection to check.</param>
    /// <returns>If the asset collection is being unloaded.</returns>
    public bool IsUnloading(string name) => Assets.Loaders.IsUnloading(name);

    /// <summary>
    /// If the given asset collection is already unloaded or in the async unload queue.
    /// </summary>
    /// <param name="name">The collection to check.</param>
    /// <returns>If the asset collection is being unloaded or is already unloaded.</returns>
    public bool IsUnloadedOrUnloading(string name) => Assets.Loaders.IsUnloadedOrUnloading(name);

    /// <summary>
    /// Halt the current thread until a queued asset collection is loaded.
    /// </summary>
    /// <param name="name">The collection to wait for.</param>
    public void WaitForLoad(string name) => Assets.Loaders.WaitForLoad(name);

    /// <summary>
    /// Halt the current thread until a queued asset collection is unloaded.
    /// </summary>
    /// <param name="name">The collection to wait for.</param>
    public void WaitForUnload(string name) => Assets.Loaders.WaitForUnload(name);

    /// <summary>
    /// Halt the current thread until all asset collections queued for load are completed.
    /// </summary>
    public void WaitUntilAllLoaded() => Assets.Loaders.WaitForLoadAll();

    /// <summary>
    /// Halt the current thread until all asset collections queued for unload are completed.
    /// </summary>
    public void WaitUntilAllUnloaded() => Assets.Loaders.WaitForUnloadAll();

    /// <summary>
    /// Halt the current thread until all asset collections queued for load or unload are completed.
    /// </summary>
    public void WaitForLoadQueueFinish() => Assets.Loaders.WaitForQueueFinish();
}