namespace Jackdaw;

/// <summary>
/// A single stage in the loading pipeline for all game assets.
/// Can be registered with the game's <see cref="Assets" /> to load in assets on game startup. <br/>
/// Contains features for controlling what components run before or after one another.
/// </summary>
public abstract class AssetLoaderStage() {
    readonly List<Type> Before = [];
    readonly List<Type> After = [];

    /// <summary>
    /// Get a list of all assets the stage is able to load in as assets.
    /// </summary>
    /// <param name="assets">The game instance's asset controller.</param>
    public abstract AssetProviderItem[] GetLoadOptions(Assets assets);

    /// <summary>
    /// Run the loader stage on a given asset collection.
    /// </summary>
    /// <param name="assets">The game instance's asset controller.</param>
    /// <param name="collection">The asset collection to load from.</param>
    public abstract void RunLoad(Assets assets, AssetCollection collection);

    /// <summary>
    /// Unload all assets from a given asset collection.
    /// </summary>
    /// <param name="assets">The game instance's asset controller.</param>
    /// <param name="collection">The asset collection to unload.</param>
    public abstract void RunUnload(Assets assets, AssetCollection collection);

    /// <summary>
    /// Ensure this loader stage is run before a different stage of the given type executes.
    /// </summary>
    /// <typeparam name="T">The loader type to run before.</typeparam>
    /// <returns>The current loader stage.</returns>
    public AssetLoaderStage SetBefore<T>() where T : AssetLoaderStage => SetBefore(typeof(T));

    /// <summary>
    /// Ensure this loader stage is run before a different stage of the given type executes.
    /// </summary>
    /// <param name="type">The loader type to run before.</param>
    /// <returns>The current loader stage.</returns>
    public AssetLoaderStage SetBefore(Type type) {
        Before.Add(type);
        return this;
    }

    /// <summary>
    /// Ensure this loader stage is run after a different stage of the given type executes.
    /// </summary>
    /// <typeparam name="T">The loader type to run after.</typeparam>
    /// <returns>The current loader stage.</returns>
    public AssetLoaderStage SetAfter<T>() where T : AssetLoaderStage => SetAfter(typeof(T));

    /// <summary>
    /// Ensure this loader stage is run after a different stage of the given type executes.
    /// </summary>
    /// <param name="type">The loader type to run after.</param>
    /// <returns>The current loader stage.</returns>
    public AssetLoaderStage SetAfter(Type type) {
        After.Add(type);
        return this;
    }

    /// <summary>
    /// Add an asset to the loaded asset storage.
    /// </summary>
    /// <typeparam name="T">The type of asset to add.</typeparam>
    /// <param name="assets">The game instance's asset manager.</param>
    /// <param name="name">The asset's name.</param>
    /// <param name="asset">The asset.</param>
    public static void AddAsset<T>(Assets assets, string name, T asset)
        => assets.Add(name, asset);

    /// <summary>
    /// Remove an asset from the loaded asset storage.
    /// </summary>
    /// <typeparam name="T">The type of asset to remove.</typeparam>
    /// <param name="assets">The game instance's asset manager.</param>
    /// <param name="name">The asset's name.</param>
    public static void RemoveAsset<T>(Assets assets, string name)
        => assets.Remove<T>(name);

    /// <summary>
    /// Find a registered asset loaded stage.
    /// </summary>
    /// <typeparam name="T">The type of stage to find.</typeparam>
    /// <param name="assets">The game instance's asset manager.</param>
    /// <returns>The asset loader of the given type, if one exists.</returns>
    public static T? Find<T>(Assets assets) where T : AssetLoaderStage
        => assets.Loaders.Find<T>();

    internal bool IsBefore(AssetLoaderStage loader) => IsBefore(loader.GetType());
    internal bool IsBefore(Type type) => Before.Contains(type);
    internal bool IsAfter(AssetLoaderStage loader) => IsAfter(loader.GetType());
    internal bool IsAfter(Type type) => After.Contains(type);
}