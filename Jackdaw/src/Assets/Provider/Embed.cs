using System.Reflection;

namespace Jackdaw;

/// <summary>
/// An asset provider for loading data from embedded files inside the project's assemblies.
/// </summary>
public class EmbeddedResourceAssetProvider : IAssetProvider {
    record struct ItemMetadeta(int Assembly, string InternalID);

    readonly string Root;
    readonly List<Assembly> Assemblies;
    readonly Dictionary<string, List<AssetProviderItem>> Groups = [];
    readonly Dictionary<AssetProviderItem, ItemMetadeta> Metadata = [];

    /// <summary>
    /// An asset provider for loading data from embedded files inside the project's assemblies.
    /// </summary>
    /// <param name="root">
    ///     The group name to load asset from. This will be the first element in the resource's name <br/>
    ///     For example, Example.file.name.txt would have the root 'Example', group 'file' and name 'name'.
    /// </param>
    /// <param name="assemblies">The assemblies to load embedded files from.</param>
    public EmbeddedResourceAssetProvider(string root, params Assembly[] assemblies) {
        Root = root;
        Assemblies = [.. assemblies];
        LoadAllAssemblies();
    }

    /// <summary>
    /// An asset provider for loading data from embedded files inside the project's assemblies.
    /// </summary>
    /// <param name="root">
    ///     The group name to load asset from. This will be the first element in the resource's name <br/>
    ///     For example, Example.file.name.txt would have the root 'Example', group 'file' and name 'name'.
    /// </param>
    /// <param name="types">A type from each assembly to load embedded files from.</param>
    public EmbeddedResourceAssetProvider(string root, params Type[] types)
        : this(root, [.. types.Select(e => e.Assembly)]) { }

    public void Open(Game game) { }

    public void Close() { }

    public void AddAssembly(Type type) => AddAssembly(type.Assembly);
    public void AddAssembly(Assembly assembly) {
        if (Assemblies.Contains(assembly)) { return; }
        int index = Assemblies.Count;
        Assemblies.Add(assembly);
        LoadAssemblyData(index);
    }

    void LoadAllAssemblies() {
        for (int i = 0; i < Assemblies.Count; ++i) {
            LoadAssemblyData(i);
        }
    }

    void LoadAssemblyData(int assembly) {
        string prefix = $"{Assemblies[assembly].GetName().Name}.{Root}";

        foreach (string str in Assemblies[assembly].GetManifestResourceNames()) {
            if (!str.StartsWith(prefix)) { continue; }

            string[] split = str.Split(".");
            if (split.Length < 4) { continue; }

            string extension = $".{split[^1]}";
            string group = string.Empty;
            string name;

            if (split.Length == 4) { name = split[2]; }
            else {
                group = split[2];
                name = string.Join("/", split[3..^1]);
            }

            AssetProviderItem item = new(group, name, extension);
            if (!Groups.TryGetValue(group, out List<AssetProviderItem>? value)) {
                value = [];
                Groups.Add(group, value);
            }
            value.Add(item);
            Metadata.Add(item, new(assembly, str));
        }
    }

    public string[] GetGroups() => [.. Groups.Keys];

    public bool HasGroup(string group) => Groups.ContainsKey(group);

    public bool HasItem(string group, string name, string extension)
        => HasItem(new(group, name, extension));

    public bool HasItem(AssetProviderItem item) {
        if (!Groups.TryGetValue(item.Group, out List<AssetProviderItem>? groupValue)) { return false; }
        return groupValue.Any(e => e.Name == item.Name && e.Extension == item.Extension);
    }

    public AssetProviderItem[] GetItemsInGroup(string group, params string[] extensionFilter) {
        if (!Groups.TryGetValue(group, out List<AssetProviderItem>? groupValue)) { return []; }
        if (extensionFilter.Length == 0) { return [.. groupValue]; }
        return [.. groupValue.Where(e => extensionFilter.Any(ext => ext == e.Extension))];
    }

    public Stream GetItemStream(AssetProviderItem item) {
        ItemMetadeta meta = Metadata[item];
        return Assemblies[meta.Assembly].GetManifestResourceStream(meta.InternalID)!;
    }
}