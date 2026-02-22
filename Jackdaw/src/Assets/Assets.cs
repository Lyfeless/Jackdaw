using System.Reflection;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Manager class used for storing and loading game assets including textures, sounds, fonts, etc
/// </summary>
public class Assets {
    readonly Dictionary<Type, IAssetStorage> LoadedAssets = [];
    readonly List<AssetLoaderStage> LoaderStages = [];
    bool Loaded = false;

    readonly Dictionary<Type, string> TypeWarnings = [];

    /// <summary>
    /// The game instance the assets are being used for.
    /// </summary>
    public readonly Game Game;

    /// <summary>
    /// The game's graphics device, used for creating assets.
    /// </summary>
    public GraphicsDevice GraphicsDevice => Game.GraphicsDevice;

    /// <summary>
    /// Configuration data for how assets should be loaded.
    /// </summary>
    public readonly GameContentConfig Config;

    /// <summary>
    /// The source assets are loaded from.
    /// </summary>
    public readonly IAssetProvider Provider;

    /// <summary>
    /// The library's file assembly. <br/>
    /// Most default fallback data is stored inside the embedded assembly.
    /// </summary>
    public readonly Assembly Assembly;

    /// <summary>
    /// The name of the library's executing assembly.
    /// </summary>
    public readonly string AssemblyName;

    public Assets(Game game, GameContentConfig config) {
        Game = game;
        Config = config;

        Provider = config.AssetProvider;

        Assembly = Assembly.GetExecutingAssembly();
        AssemblyName = Assembly.GetName().Name ?? "";

        RegisterLoaderStage(new TextureLoader());
        RegisterLoaderStage(new TextureFallbackLoader());
        RegisterLoaderStage(new AsepriteLoader());
        RegisterLoaderStage(new AsepriteAnimationLoader());
        RegisterLoaderStage(new PackerLoader());
        RegisterLoaderStage(new AnimationLoader());
        RegisterLoaderStage(new FontLoader());
        RegisterLoaderStage(new ShaderLoader());

        AddTypeWarning<Image>("Use Subtexture to access auto-loaded image data.");
        AddTypeWarning<Texture>("Use Subtexture to access auto-loaded texture data.");
        AddTypeWarning<Font>("Use Spritefont to access auto-loaded font data.");

        foreach (AssetLoaderStage stage in config.CustomAssetLoaders) {
            RegisterLoaderStage(stage);
        }

        Load();
    }

    /// <summary>
    /// Add a custom asset loader for a specific asset type. <br/>
    /// Only needed for assets that require custom logic when accessed,
    /// any asset that doesn't need additional logic after loading is will automatically use a default storage system.
    /// </summary>
    /// <typeparam name="T">The type to register the storage for.</typeparam>
    /// <param name="storage">The asset storage object.</param>
    public void RegisterCustomAssetStorage<T>(IAssetStorage storage)
        => RegisterCustomAssetStorage(typeof(T), storage);

    /// <summary>
    /// Add a custom asset loader for a specific asset type. <br/>
    /// Only needed for assets that require custom logic when accessed,
    /// any asset that doesn't need additional logic after loading is will automatically use a default storage system.
    /// </summary>
    /// <param name="type">The type to register the storage for.</param>
    /// <param name="storage">The asset storage object.</param>
    public void RegisterCustomAssetStorage(Type type, IAssetStorage storage) {
        if (LoadedAssets.ContainsKey(type)) {
            Log.Warning($"Attempting to add a new storage object for type {type} when one already exists, skipping.");
            return;
        }
        LoadedAssets.Add(type, storage);
    }

    /// <summary>
    /// Find an asset loader stage in the queue by type. Currently only one loader of each type is supported.
    /// </summary>
    /// <typeparam name="T">The loader type to find.</typeparam>
    /// <returns>The asset loader, null if no loader matches the given type.</returns>
    public T? FindLoaderStage<T>() where T : AssetLoaderStage => (T?)LoaderStages.FirstOrDefault(e => e.GetType() == typeof(T));

    /// <summary>
    /// Info to append to exceptions when searching for an asset of a specified type. <br/>
    /// Used mostly for informing programs searching for the wrong asset types.
    /// </summary>
    /// <typeparam name="T">The asset type to show the warning for.</typeparam>
    /// <param name="warning">The warning to show in exceptions.</param>
    public void AddTypeWarning<T>(string warning) => AddTypeWarning(typeof(T), warning);

    /// <summary>
    /// Info to append to exceptions when searching for an asset of a specified type. <br/>
    /// Used mostly for informing programs searching for the wrong asset types.
    /// </summary>
    /// <param name="type">The asset type to show the warning for.</param>
    /// <param name="warning">The warning to show in exceptions.</param>
    public void AddTypeWarning(Type type, string warning) => TypeWarnings.Add(type, warning);

    void Load() {
        if (Loaded) {
            Log.Warning("Assets: Attempting to run asset load after load has already run, skipping.");
            return;
        }

        for (int i = LoaderStages.Count; i > 0; --i) {
            AssetLoaderStage? loader = NextLoaderStage() ?? throw new Exception("Assets: AssetLoader ordering error, can't finish loading assets due to dependancy loop.");
            loader.Run(this);
            LoaderStages.Remove(loader);
        }

        Loaded = true;
    }

    void RegisterLoaderStage(AssetLoaderStage loader) {
        if (LoaderStages.Any(e => e.GetType() == loader.GetType())) {
            Log.Warning($"Trying to add a second loader of type {loader.GetType()}. Behavior is currently unsupported, skipping.");
            return;
        }
        LoaderStages.Add(loader);
    }

    AssetLoaderStage? NextLoaderStage() {
        foreach (AssetLoaderStage loader in LoaderStages) {
            if (CanRunLoaderStage(loader)) { return loader; }
        }

        return null;
    }

    bool CanRunLoaderStage(AssetLoaderStage loader) {
        foreach (AssetLoaderStage other in LoaderStages) {
            if (loader == other) { continue; }
            if (other.IsBefore(loader) || loader.IsAfter(other)) { return false; }
        }

        return true;
    }

    /// <summary>
    /// Add a loaded asset to storage.
    /// </summary>
    /// <typeparam name="T">The type of asset to add.</typeparam>
    /// <param name="name">The asset name id. Must be unique within the asset type.</param>
    /// <param name="asset">The asset to add.</param>
    public void Add<T>(string name, T asset) => GetStorage<T>(true).Add(name, asset!);

    /// <summary>
    /// Set the fallback asset for a specific type. <br/>
    /// The fallback is given when an asset lookup fails.
    /// </summary>
    /// <typeparam name="T">The type of asset to set the fallback for.</typeparam>
    /// <param name="asset">The asset to set as fallback.</param>
    public void SetFallback<T>(T asset) => GetStorage<T>(true).SetFallback(asset!);

    /// <summary>
    /// Get the name ids of all loaded assets of a type.
    /// </summary>
    /// <typeparam name="T">The asset type to get the name ids of.</typeparam>
    /// <returns>A list of name ids.</returns>
    public string[] GetLoadedAssetNames<T>() => GetLoadedAssetNames(typeof(T));

    /// <summary>
    /// Get the name ids of all loaded assets of a type.
    /// </summary>
    /// <param name="type">The asset type to get the name ids of.</param>
    /// <returns>A list of name ids.</returns>
    public string[] GetLoadedAssetNames(Type type) {
        if (!LoadedAssets.TryGetValue(type, out IAssetStorage? storage)) { return []; }
        return storage.GetAssetNames();
    }

    /// <summary>
    /// Get a loaded asset from storage.
    /// </summary>
    /// <typeparam name="T">The type of asset to find.</typeparam>
    /// <param name="name">The name id of the asset to find.</param>
    /// <returns>The requested asset, or the asset type's fallback if the name id isn't present.</returns>
    public T Get<T>(string name) => (T)GetStorage<T>().Get(name);

    /// <summary>
    /// Get the fallback asset for the given type.
    /// The fallback is normally given when an asset lookup fails.
    /// </summary>
    /// <typeparam name="T">The asset type to get the fallback for.</typeparam>
    /// <returns>The asset type's fallback value.</returns>
    public T GetFallback<T>() => (T)GetStorage<T>().GetFallback();

    /// <summary>
    /// Get a loaded texture asset from storage.
    /// </summary>
    /// <param name="name">The name id of the texture asset to find.</param>
    /// <returns>The requested texture asset, or the fallback texture if the name id isn't present.</returns>
    public Subtexture GetSubtexture(string name) => Get<Subtexture>(name);

    /// <summary>
    /// Get a loaded font asset from storage.
    /// </summary>
    /// <param name="name">The name id of the font asset to find.</param>
    /// <returns>The requested font asset, or the fallback font if the name id isn't present.</returns>
    public SpriteFont GetSpriteFont(string name) => Get<SpriteFont>(name);

    /// <summary>
    /// Get a loaded shader asset from storage.
    /// </summary>
    /// <param name="name">The name id of the shader asset to find.</param>
    /// <returns>The requested shader asset, or the fallback shader if the name id isn't present.</returns>
    public Shader GetShader(string name) => Get<Shader>(name);

    /// <summary>
    /// Get a loaded animation asset from storage.
    /// </summary>
    /// <param name="name">The name id of the animation asset to find.</param>
    /// <returns>The requested animation asset, or the fallback animation if the name id isn't present.</returns>
    public AnimationData GetAnimationData(string name) => Get<AnimationData>(name);

    IAssetStorage GetStorage<T>(bool create = false)
        => GetStorage(typeof(T), create);

    IAssetStorage GetStorage(Type type, bool create = false) {
        if (!LoadedAssets.TryGetValue(type, out IAssetStorage? value)) {
            if (create) {
                value = new AssetStorage(type);
                LoadedAssets.Add(type, value);
            }
            else {
                TypeWarnings.TryGetValue(type, out string? warning);
                throw new Exception($"Assets: No assets initialized for type {type}, could not send fallback. {warning}");
            }
        }
        return value;
    }
}