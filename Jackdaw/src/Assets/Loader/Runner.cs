namespace Jackdaw;

internal class AssetLoaderRunner(AssetCollection collection, Action<Assets, AssetCollection, AssetLoaderStage> action) {
    public readonly AssetCollection Collection = collection;
    public readonly Action<Assets, AssetCollection, AssetLoaderStage> Action = action;

    public void Run(Assets assets, List<AssetLoaderStage> stages) {
        // Clone stage list so modifications don't apply to provided list
        stages = [.. stages];

        for (int i = stages.Count; i > 0; --i) {
            AssetLoaderStage loader = NextLoaderStage(stages) ?? throw new Exception("Assets: AssetLoader ordering error, can't finish loading assets due to dependancy loop.");
            Action(assets, Collection, loader);
            stages.Remove(loader);
        }
    }

    static AssetLoaderStage? NextLoaderStage(List<AssetLoaderStage> stages)
        => stages.FirstOrDefault(e => CanRunLoaderStage(e, stages));

    static bool CanRunLoaderStage(AssetLoaderStage loader, List<AssetLoaderStage> stages)
        => !stages.Any(e => e != loader && (e.IsBefore(loader) || loader.IsAfter(e)));

    public static void RunnerActionLoad(Assets assets, AssetCollection collection, AssetLoaderStage stage)
        => stage.RunLoad(assets, collection);

    public static void RunnerActionUnload(Assets assets, AssetCollection collection, AssetLoaderStage stage)
        => stage.RunUnload(assets, collection);
}