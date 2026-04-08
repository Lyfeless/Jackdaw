using Foster.Framework;

namespace Jackdaw;

public class AssetLoaderPipeline(Assets assets) {
    readonly Assets Assets = assets;
    readonly List<AssetLoaderStage> Stages = [];

    internal AssetProviderItem[] GetLoadOptions() {
        IEnumerable<AssetProviderItem> options = [];
        foreach (AssetLoaderStage stage in Stages) {
            options = options.Concat(stage.GetLoadOptions(Assets));
        }
        return [.. options];
    }

    internal void RunLoad(AssetCollection collection) {
        AssetLoaderRunner runner = new(collection, AssetLoaderRunner.RunAction.LOAD_COLLECTION);
        runner.Run(Assets, Stages);
    }

    internal void RunUnload(AssetCollection collection) {
        AssetLoaderRunner runner = new(collection, AssetLoaderRunner.RunAction.UNLOAD_COLLECTION);
        runner.Run(Assets, Stages);
    }

    /// <summary>
    /// Find an asset loader stage in the queue by type. Currently only one loader of each type is supported.
    /// </summary>
    /// <typeparam name="T">The loader type to find.</typeparam>
    /// <returns>The asset loader, null if no loader matches the given type.</returns>
    public T? Find<T>() where T : AssetLoaderStage => (T?)Stages.FirstOrDefault(e => e.GetType() == typeof(T));

    internal void Register(AssetLoaderStage loader) {
        if (Stages.Any(e => e.GetType() == loader.GetType())) {
            Log.Warning($"Asset Loader: Trying to add a second loader of type {loader.GetType()}. Behavior is currently unsupported, skipping.");
            return;
        }
        Stages.Add(loader);
    }
}