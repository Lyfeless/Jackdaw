namespace Jackdaw;

/// <summary>
/// A collection of assets identifiers, used for loading and unloading assets in the manager.
/// </summary>
/// <param name="name">The name of the collection.</param>
/// <param name="items">The assets in the collection.</param>
public readonly struct AssetCollection(string name, AssetProviderItem[] items) {
    /// <summary>
    /// The name of the collection.
    /// </summary>
    public readonly string Name = name;

    /// <summary>
    /// The assets in the collection.
    /// </summary>
    public readonly AssetProviderItem[] Items = items;

    /// <summary>
    /// Get all items in the collection that are part of a given group.
    /// </summary>
    /// <param name="group">The group to filter for.</param>
    /// <returns>All assets in the collection that are part of the group.</returns>
    public AssetProviderItem[] Filter(string group)
        => Filter(group, []);

    /// <summary>
    /// Get all items in the collection that are part of a given group with any of the given extensions.
    /// </summary>
    /// <param name="group">The group to filter for.</param>
    /// <param name="extensions">The extensions to filter for. Leave empty to allow any extension.</param>
    /// <returns>All assets in the collection that are part of the group with the given extension.</returns>
    public AssetProviderItem[] Filter(string group, params string[] extensions) {
        return [..
            Items.Where(e =>
                e.Group == group &&
                (extensions.Length == 0 || extensions.Contains(e.Extension))
            )
        ];
    }
}