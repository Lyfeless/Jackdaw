namespace Jackdaw;

internal class AssetLoaderRunner(AssetCollection collection, AssetLoaderRunner.RunAction action) {
    public enum RunAction {
        LOAD_COLLECTION,
        UNLOAD_COLLECTION
    }

    readonly AssetCollection Collection = collection;
    readonly RunAction Action = action;

    public void Run(Assets assets, List<AssetLoaderStage> stages) {
        // Clone stage list so modifications don't apply to provided list
        stages = [.. stages];

        for (int i = stages.Count; i > 0; --i) {
            AssetLoaderStage loader = NextLoaderStage(stages) ?? throw new Exception("Assets: AssetLoader ordering error, can't finish loading assets due to dependancy loop.");

            switch (Action) {
                case RunAction.LOAD_COLLECTION: loader.RunLoad(assets, Collection); break;
                case RunAction.UNLOAD_COLLECTION: loader.RunUnload(assets, Collection); break;
            }

            stages.Remove(loader);
        }
    }

    static AssetLoaderStage? NextLoaderStage(List<AssetLoaderStage> stages)
        => stages.FirstOrDefault(e => CanRunLoaderStage(e, stages));

    static bool CanRunLoaderStage(AssetLoaderStage loader, List<AssetLoaderStage> stages)
        => !stages.Any(e => e != loader && (e.IsBefore(loader) || loader.IsAfter(e)));
}