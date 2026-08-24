using Foster.Framework;

namespace Jackdaw;

public abstract class StorageObjectAssetProvider() : IAssetProvider {
    protected StorageContainer? FileStorage = null;

    protected abstract void AssignStorage(Game game);

    public void Open(Game game) {
        AssignStorage(game);
    }

    public void Close() {
        FileStorage?.Dispose();
        FileStorage = null;
    }

    bool GroupExists(string group) => FileStorage?.DirectoryExists(GroupPath(group)) ?? false;
    bool ItemExists(AssetProviderItem item) => FileStorage?.FileExists(ItemPath(item)) ?? false;

    static string GroupPath(string group) => group;
    static string ItemPath(AssetProviderItem item) => Path.Join(item.Group, $"{item.Name}{item.Extension}");

    public string[] GetGroups() {
        if (FileStorage == null) { return []; }
        HashSet<string> groups = [];
        foreach (string str in FileStorage.EnumerateDirectory("/", "*.*", SearchOption.AllDirectories)) {
            // Remove the first 2 elements of the string because this search prefixes all the names with '//'
            string[] elements = str[2..].Split("/");
            string group = elements.Length == 1 ? string.Empty : elements[0];
            groups.Add(group);
        }
        return [.. groups];
    }

    public bool HasGroup(string group) => GroupExists(group);

    public AssetProviderItem[] GetItemsInGroup(string group, params string[] extensionFilter) {
        if (!GroupExists(group)) { return []; }
        IEnumerable<string> enumerable = FileStorage!.EnumerateDirectory(GroupPath(group), "*.*", SearchOption.AllDirectories);
        if (extensionFilter.Length > 0) { enumerable = enumerable.Where(e => extensionFilter.Any(e.EndsWith)); }
        return [.. enumerable.Select(e => new AssetProviderItem(group, GetAssetName(group, e), Path.GetExtension(e)))];
    }

    public Stream GetItemStream(AssetProviderItem item) => FileStorage?.OpenRead(ItemPath(item))!;

    public bool HasItem(string group, string name, string extension)
        => HasItem(new(group, name, extension));

    public bool HasItem(AssetProviderItem item)
        => ItemExists(item);

    static string GetAssetName(string group, string assetPath) {
        string name = Path.Join(Path.GetDirectoryName(assetPath), Path.GetFileNameWithoutExtension(assetPath));
        name = Path.GetRelativePath(GroupPath(group), name);
        name = name.Replace("\\", "/");
        return name;
    }
}