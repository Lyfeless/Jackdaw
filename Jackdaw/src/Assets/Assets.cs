using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Manager class used for storing and loading game assets including textures, sounds, fonts, etc
/// </summary>
public class Assets {
    readonly AssetStoragePipeline Storage = new();
    public readonly AssetLoaderPipeline Loaders;
    readonly Dictionary<string, AssetCollection> Collections = [];

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
    /// The provider for accessing fallback files
    /// </summary>
    public readonly EmbeddedResourceAssetProvider FallbackProvider = new("Fallback");

    public Assets(Game game, GameContentConfig config) {
        Game = game;
        Config = config;

        Provider = config.AssetProvider;

        Loaders = new(this);

        RegisterLoaders(config);
        SetTypeWarnings();
        CreateCollections(config);
        SetDefaultFallbacks();

        Load(config);
    }

    void Load(GameContentConfig config) {
        if (config.UseAssetCollections) {
            foreach (string name in config.StartupCollections) {
                LoadCollection(name);
            }
        }
        else {
            LoadCollection(string.Empty);
        }
    }

    void RegisterLoaders(GameContentConfig config) {
        Loaders.Register(new TextureLoader());
        Loaders.Register(new AsepriteLoader());
        Loaders.Register(new AsepriteAnimationLoader());
        Loaders.Register(new PackerLoader());
        Loaders.Register(new AnimationLoader());
        Loaders.Register(new FontLoader());
        Loaders.Register(new ShaderLoader());

        foreach (AssetLoaderStage stage in config.CustomAssetLoaders) {
            Loaders.Register(stage);
        }
    }

    void CreateCollections(GameContentConfig config) {
        AssetProviderItem[] loadOptions = Loaders.GetLoadOptions();

        if (!config.UseAssetCollections) {
            Collections.Add(string.Empty, AssetCollectionBuilder.SingleFromAll().Filter(loadOptions));
            return;
        }

        foreach (AssetCollectionBuilder collection in config.Collections) {
            Collections.Add(collection.Name, collection.Filter(loadOptions));
        }
    }

    void SetTypeWarnings() {
        Storage.SetTypeWarning<Image>("Use Subtexture to access auto-loaded image data.");
        Storage.SetTypeWarning<Texture>("Use Subtexture to access auto-loaded texture data.");
        Storage.SetTypeWarning<Font>("Use Spritefont to access auto-loaded font data.");
    }

    void SetDefaultFallbacks() {
        // Texture Fallback
        using Stream textureStream = FallbackProvider.GetItemStream(new("", "texture", ".png"));
        SetFallback(new Subtexture(new Texture(GraphicsDevice, new Image(textureStream))));

        //Font Fallback
        using Stream fontStream = FallbackProvider.GetItemStream(new("", "font", ".ttf"));
        SetFallback(new SpriteFont(GraphicsDevice, fontStream, 16));

        // Animation Fallback
        SetFallback(new AnimationData(GetFallback<Subtexture>(), [new(0, TimeSpan.Zero)], TimeSpan.Zero));
    }

    /// <summary>
    /// Set the fallback asset for a specific type. <br/>
    /// The fallback is given when an asset lookup fails.
    /// </summary>
    /// <typeparam name="T">The type of asset to set the fallback for.</typeparam>
    /// <param name="asset">The asset to set as fallback.</param>
    public void SetFallback<T>(T asset)
        => Storage.Get<T>(AssetStoragePipeline.RequestFailResponse.CREATE).SetFallback(asset!);

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
    public string[] GetLoadedAssetNames(Type type)
        => Storage.Get(type, AssetStoragePipeline.RequestFailResponse.NOTHING).GetAssetNames();

    /// <summary>
    /// Get a loaded asset from storage.
    /// </summary>
    /// <typeparam name="T">The type of asset to find.</typeparam>
    /// <param name="name">The name id of the asset to find.</param>
    /// <returns>The requested asset, or the asset type's fallback if the name id isn't present.</returns>
    public T Get<T>(string name) => (T)Storage.Get<T>().Get(name);

    /// <summary>
    /// Get the fallback asset for the given type.
    /// The fallback is normally given when an asset lookup fails.
    /// </summary>
    /// <typeparam name="T">The asset type to get the fallback for.</typeparam>
    /// <returns>The asset type's fallback value.</returns>
    public T GetFallback<T>() => (T)Storage.Get<T>().GetFallback();

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

    public void LoadCollection(string name) {
        if (!Collections.TryGetValue(name, out AssetCollection collection)) { return; }
        Loaders.RunLoad(collection);
    }

    public Task LoadCollectionAsync(string name) {
        throw new NotImplementedException();
    }

    public void UnloadCollection(string name) {
        if (!Collections.TryGetValue(name, out AssetCollection collection)) { return; }
        Loaders.RunUnload(collection);
    }

    public Task UnloadCollectionAsync(string name) {
        throw new NotImplementedException();
    }

    public bool IsCollectionLoaded(string name) {
        throw new NotImplementedException();
    }

    public bool IsCollectionLoading(string name) {
        throw new NotImplementedException();
    }

    public bool IsCollectionLoadedOrLoading(string name)
        => IsCollectionLoaded(name) || IsCollectionLoading(name);

    public bool IsCollectionUnloaded(string name)
        => !IsCollectionLoadedOrLoading(name);


    public bool WaitForCollectionLoad(string name) {
        throw new NotImplementedException();
    }

    public bool WaitForCollectionUnload(string name) {
        throw new NotImplementedException();
    }

    public bool IsAssetLoaded<T>(string name) {
        throw new NotImplementedException();
    }

    public bool IsAssetLoading<T>(string name) {
        throw new NotImplementedException();
    }

    public bool IsAssetLoadedOrLoading<T>(string name)
        => IsAssetLoaded<T>(name) || IsAssetLoading<T>(name);

    public bool IsAssetUnloaded<T>(string name)
        => !IsAssetLoadedOrLoading<T>(name);

    public void WaitForAssetLoad<T>(string name) {
        throw new NotImplementedException();
    }

    public void WaitForAssetUnload<T>(string name) {
        throw new NotImplementedException();
    }

    internal void Add<T>(string name, T asset) => Storage.Get<T>(AssetStoragePipeline.RequestFailResponse.CREATE).Add(name, asset!);

    internal void Remove<T>(string name) => Storage.Get<T>(AssetStoragePipeline.RequestFailResponse.NOTHING).Remove(name);
}