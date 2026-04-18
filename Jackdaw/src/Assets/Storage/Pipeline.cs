using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Manager for adding and finding storage objects for different types of assets.
/// </summary>
public class AssetStoragePipeline {
    internal enum RequestFailResponse {
        THROW,
        CREATE,
        NOTHING
    }

    readonly Dictionary<Type, IAssetStorage> Containers = [];
    readonly Dictionary<Type, string> TypeWarnings = [];

    /// <summary>
    /// Add a custom asset storage for a specific asset type. <br/>
    /// Only needed for assets that require custom logic when accessed,
    /// any asset that doesn't need additional logic after loading is will automatically use a default storage system.
    /// </summary>
    /// <typeparam name="T">The type to register the storage for.</typeparam>
    /// <param name="storage">The asset storage object.</param>
    public void Register<T>(IAssetStorage storage)
        => Register(typeof(T), storage);

    /// <summary>
    /// Add a custom asset storage for a specific asset type. <br/>
    /// Only needed for assets that require custom logic when accessed,
    /// any asset that doesn't need additional logic after loading is will automatically use a default storage system.
    /// </summary>
    /// <param name="type">The type to register the storage for.</param>
    /// <param name="storage">The asset storage object.</param>
    public void Register(Type type, IAssetStorage storage) {
        if (Containers.ContainsKey(type)) {
            Log.Warning($"Asset Storage: Attempting to add a new storage object for type {type} when one already exists, skipping.");
            return;
        }
        Containers.Add(type, storage);
    }

    /// <summary>
    /// Info to append to exceptions when searching for an asset of a specified type. <br/>
    /// Used mostly for informing programs searching for the wrong asset types.
    /// </summary>
    /// <typeparam name="T">The asset type to show the warning for.</typeparam>
    /// <param name="warning">The warning to show in exceptions.</param>
    public void SetTypeWarning<T>(string warning) => SetTypeWarning(typeof(T), warning);

    /// <summary>
    /// Info to append to exceptions when searching for an asset of a specified type. <br/>
    /// Used mostly for informing programs searching for the wrong asset types.
    /// </summary>
    /// <param name="type">The asset type to show the warning for.</param>
    /// <param name="warning">The warning to show in exceptions.</param>
    public void SetTypeWarning(Type type, string warning) => TypeWarnings[type] = warning;

    internal IAssetStorage Get<T>(RequestFailResponse failResponse = RequestFailResponse.THROW)
        => Get(typeof(T), failResponse);

    internal IAssetStorage Get(Type type, RequestFailResponse failResponse = RequestFailResponse.THROW) {
        if (!Containers.TryGetValue(type, out IAssetStorage? value)) {
            switch (failResponse) {
                case RequestFailResponse.THROW:
                    TypeWarnings.TryGetValue(type, out string? warning);
                    throw new Exception($"Assets: No assets initialized for type {type}, could not send fallback. {warning}");
                case RequestFailResponse.CREATE:
                    value = new AssetStorage(type);
                    Containers.Add(type, value);
                    break;
                default:
                    value = new AssetStorage(type);
                    break;
            }
        }
        return value;
    }
}