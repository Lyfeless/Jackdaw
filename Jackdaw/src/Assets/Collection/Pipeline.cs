using Foster.Framework;

namespace Jackdaw;

public class AssetCollectionPipeline {
    readonly Assets Assets;

    readonly Dictionary<string, AssetCollection> Collections = [];

    public AssetCollectionPipeline(Assets assets, AssetCollectionBuilderContainer builder) {
        Assets = assets;

        AssetProviderItem[] loadOptions = assets.Loaders.GetLoadOptions();

        Collections = builder.FilterAll(assets, loadOptions);
    }

    public void Load(string name) {
        if (!Collections.TryGetValue(name, out AssetCollection collection)) {
            Log.Warning($"Asset Loader: no matching collection {name} exists, skipping load.");
            return;
        }

        Assets.Loaders.RunLoad(collection);
    }

    public void LoadAsync(string name) {
        if (!Collections.TryGetValue(name, out AssetCollection collection)) {
            Log.Warning($"Asset Loader: no matching collection {name} exists, skipping load.");
            return;
        }

        Assets.Loaders.RunLoadAsync(collection);
    }

    public void Unload(string name) {
        if (!Collections.TryGetValue(name, out AssetCollection collection)) {
            Log.Warning($"Asset Loader: no matching collection {name} exists, skipping unload.");
            return;
        }

        Assets.Loaders.RunUnload(collection);
    }

    public void UnloadAsync(string name) {
        if (!Collections.TryGetValue(name, out AssetCollection collection)) {
            Log.Warning($"Asset Loader: no matching collection {name} exists, skipping unload.");
            return;
        }

        Assets.Loaders.RunUnloadAsync(collection);
    }

    public bool IsLoaded(string name) => Assets.Loaders.IsLoaded(name);

    public bool IsLoading(string name) => Assets.Loaders.IsLoading(name);

    public bool IsLoadedOrLoading(string name) => Assets.Loaders.IsLoadedOrLoading(name);

    public bool IsUnloaded(string name) => Assets.Loaders.IsUnloaded(name);

    public void WaitForLoad(string name) => Assets.Loaders.WaitForLoad(name);

    public void WaitForUnload(string name) => Assets.Loaders.WaitForUnload(name);

    public void WaitUntilAllLoaded() => Assets.Loaders.WaitForLoadAll();

    public void WaitUntilAllUnloaded() => Assets.Loaders.WaitForUnloadAll();

    public void WaitForLoadQueueFinish() => Assets.Loaders.WaitForQueueFinish();
}