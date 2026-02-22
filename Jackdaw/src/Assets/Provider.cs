namespace Jackdaw;

/// <summary>
/// Interface for importing assets from disc. The provider can be assigned in the <see cref="GameContentConfig.AssetProvider"/> on startup.
/// </summary>
public interface IAssetProvider {
    /// <summary>
    /// Get all asset grouping names. <br/>
    /// For file providers, this would be the first level subdirectories in the content folder.
    /// </summary>
    /// <returns>A list of all group names.</returns>
    public string[] GetGroups();

    /// <summary>
    /// If the provider group exists.
    /// </summary>
    /// <param name="group">The group to check for.</param>
    /// <returns>If the provider has a group with a matching name.</returns>
    public bool HasGroup(string group);

    /// <summary>
    /// If an asset exists in the provider.
    /// </summary>
    /// <param name="group">The asset's provider group.</param>
    /// <param name="name">The asset name.</param>
    /// <param name="extension">The file type of the asset (including the '.').</param>
    /// <returns>If the provider has an asset matching the description.</returns>
    public bool HasItem(string group, string name, string extension);

    /// <summary>
    /// If an asset exists in the provider.
    /// </summary>
    /// <param name="item">The asset description.</param>
    /// <returns>If the provider has an asset matching the description.</returns>
    public bool HasItem(AssetProviderItem item);

    /// <summary>
    /// Get all assets in a provider group.
    /// </summary>
    /// <param name="group">The group to search in.</param>
    /// <param name="extensionFilter">
    ///     An optional file type filter.
    ///     If empty all files will be returned, otherwise only files with an extension in the filter will be given.
    /// </param>
    /// <returns>A list of matching item definitions in the provider group.</returns>
    public AssetProviderItem[] GetItemsInGroup(string group, params string[] extensionFilter);

    /// <summary>
    /// Create a stream to load a provider asset.
    /// </summary>
    /// <param name="item">The item definition.</param>
    /// <returns>A stream for loading the asset.</returns>
    public Stream GetItemStream(AssetProviderItem item);
}

/// <summary>
/// A definition for a single item in an asset provider.
/// </summary>
/// <param name="group">The provider group the asset is located in.</param>
/// <param name="name">The name of the asset.</param>
/// <param name="extension">The file type of the asset (including the '.').</param>
public readonly struct AssetProviderItem(string group, string name, string extension) {
    /// <summary>
    /// The provider group the asset is located in. <br/>
    /// For file providers this would be the first level subdirectories in the content folder.
    /// </summary>
    public readonly string Group = group;

    /// <summary>
    /// The name of the asset. <br/>
    /// For file providers this would be the path under the group folder, without the extension.
    /// </summary>
    public readonly string Name = name;

    /// <summary>
    /// The file type of the asset (including the '.').
    /// </summary>
    public readonly string Extension = extension;

    /// <summary>
    /// If the description value is a valid format. <br/>
    /// Does not automatically mean the item exists,
    /// use <see cref="IAssetProvider.HasItem(AssetProviderItem)"/> to be sure item is real. <br/>
    /// Group can be empty and relain valid, useful for accessing files in the content's top level.
    /// </summary>
    public readonly bool IsValid = name != string.Empty && extension != string.Empty;

    public override string ToString() {
        return $"{Group} {Name}{Extension}";
    }
}