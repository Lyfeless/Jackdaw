namespace Jackdaw;

/// <summary>
/// An asset provider for loading data from individual files on disc.
/// Uses the folder structure to create groups and names.
/// </summary>
/// <param name="rootFolder">The folder all asset groups and items are contained within.</param>
public class FileFolderAssetProvider(string rootFolder) : IAssetProvider {
    public readonly string RootFolder = rootFolder;

    bool RootExists => Directory.Exists(RootFolder);
    bool GroupExists(string group) => RootExists && Directory.Exists(GroupPath(group));
    bool ItemExists(AssetProviderItem item) => GroupExists(item.Group) && File.Exists(ItemPath(item));

    string GroupPath(string group) => Path.Join(RootFolder, group);
    string ItemPath(AssetProviderItem item) => Path.Join(RootFolder, item.Group, $"{item.Name}{item.Extension}");

    public string[] GetGroups() {
        if (!RootExists) { return []; }
        return Directory.GetDirectories(RootFolder);
    }

    public bool HasGroup(string group) => GroupExists(group);

    public AssetProviderItem[] GetItemsInGroup(string group, params string[] extensionFilter) {
        if (!GroupExists(group)) { return []; }
        IEnumerable<string> enumerable = Directory.EnumerateFiles(GroupPath(group), "*.*", SearchOption.AllDirectories);
        if (extensionFilter.Length > 0) { enumerable = enumerable.Where(e => extensionFilter.Any(e.EndsWith)); }
        return [.. enumerable.Select(e => new AssetProviderItem(group, GetAssetName(group, e), Path.GetExtension(e)))];
    }

    public Stream GetItemStream(AssetProviderItem item) => File.OpenRead(ItemPath(item));

    string GetAssetName(string group, string assetPath) {
        string name = Path.Join(Path.GetDirectoryName(assetPath), Path.GetFileNameWithoutExtension(assetPath));
        name = Path.GetRelativePath(GroupPath(group), name);
        name = name.Replace("\\", "/");
        return name;
    }

    public bool HasItem(string group, string name, string extension)
        => HasItem(new(group, name, extension));

    public bool HasItem(AssetProviderItem item)
        => ItemExists(item);
}