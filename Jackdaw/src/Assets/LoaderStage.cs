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
    /// The code to be run when the loader's stage is executed.
    /// </summary>
    /// <param name="assets"></param>
    public abstract void Run(Assets assets);


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

    internal bool IsBefore(AssetLoaderStage loader) => IsBefore(loader.GetType());
    internal bool IsBefore(Type type) => Before.Contains(type);
    internal bool IsAfter(AssetLoaderStage loader) => IsAfter(loader.GetType());
    internal bool IsAfter(Type type) => After.Contains(type);
}