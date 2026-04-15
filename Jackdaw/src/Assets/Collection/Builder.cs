using System.Text.Json;

namespace Jackdaw;

public readonly struct AssetCollectionBuilderContainer {
    internal readonly AssetCollectionBuilder[] Builders = [];

    readonly bool UseProvider = false;
    readonly AssetProviderItem ProviderItem;

    internal AssetCollectionBuilderContainer(AssetCollectionBuilder[] builders) {
        Builders = builders;
    }

    internal AssetCollectionBuilderContainer(AssetProviderItem providerItem) {
        UseProvider = true;
        ProviderItem = providerItem;
    }

    internal Dictionary<string, AssetCollection> FilterAll(Assets assets, AssetProviderItem[] items) {
        AssetCollectionBuilderContainer container = this;
        if (UseProvider && assets.Provider.HasItem(ProviderItem)) {
            using Stream stream = assets.Provider.GetItemStream(ProviderItem);
            container = AssetCollectionBuilder.FromStream(stream);
        }

        Dictionary<string, AssetCollection> collections = [];
        foreach (AssetCollectionBuilder builder in container.Builders) {
            collections.Add(builder.Name, builder.Filter(items));
        }
        return collections;
    }
}

public readonly struct AssetCollectionBuilder {
    public readonly string Name;
    readonly IAssetCollectionBuilderElement[] Elements;

    internal AssetCollectionBuilder(string name, params IAssetCollectionBuilderElement[] elements) {
        Name = name;
        Elements = elements;
    }

    internal AssetCollection Filter(AssetProviderItem[] items) {
        IEnumerable<AssetProviderItem> filtered = [];
        foreach (IAssetCollectionBuilderElement element in Elements) {
            filtered = filtered.Concat(element.Filter(items));
        }
        return new(Name, [.. filtered]);
    }

    public static AssetCollectionBuilderContainer FromAll()
        => new([new(string.Empty, new AssetCollectionBuilderElementAll())]);

    public static AssetCollectionBuilderContainer FromFile(string path) {
        if (!Path.Exists(path)) { return new([]); }
        using FileStream stream = File.OpenRead(path);
        return FromStream(stream);
    }

    public static AssetCollectionBuilderContainer FromProviderFile(string group, string name, string extension)
        => FromProviderFile(new(group, name, extension));

    public static AssetCollectionBuilderContainer FromProviderFile(AssetProviderItem item) => new(item);

    public static AssetCollectionBuilderInstance NewCollection(string name) => new(name);

    internal static AssetCollectionBuilderContainer FromStream(Stream stream) {
        Dictionary<string, string[]> collections;

        try {
            Dictionary<string, string[]>? output = JsonSerializer.Deserialize(
                stream, SourceGenerationContext.Default.DictionaryStringStringArray);
            if (output == null) { return new([]); }
            collections = output;
        } catch { return new([]); }

        return new([.. collections.Select(e => ParseElements(e.Key, e.Value))]);
    }

    static AssetCollectionBuilder ParseElements(string name, string[] entries)
        => new(name, [.. entries.Select(ParseSingleElement)]);

    static IAssetCollectionBuilderElement ParseSingleElement(string entry) {
        if (entry == string.Empty) { return new AssetCollectionBuilderElementAllIn(string.Empty, string.Empty, []); }

        bool isFilter = entry.StartsWith('>');
        if (isFilter) { entry = entry[1..]; }
        AssetProviderItem item = AssetProviderItem.FromString(entry);

        if (!isFilter) { return new AssetCollectionBuilderSingle(item); }

        string[] extensionFilter = item.Extension == string.Empty ? [] : [item.Extension];

        // paths with no subfolders are treated as full group searches
        if (item.Group == string.Empty) { return new AssetCollectionBuilderElementAllIn(item.Name, string.Empty, extensionFilter); }

        return new AssetCollectionBuilderElementAllIn(item.Group, item.Name, extensionFilter);
    }
}

public class AssetCollectionBuilderInstance {
    readonly List<AssetCollectionBuilder> builders = [];

    string newName = string.Empty;
    readonly List<IAssetCollectionBuilderElement> newElements = [];

    internal AssetCollectionBuilderInstance(string name) {
        NewCollection(name);
    }

    public AssetCollectionBuilderInstance NewCollection(string name) {
        AddBuilder();
        newName = name;
        newElements.Clear();
        return this;
    }

    public AssetCollectionBuilderInstance All() {
        newElements.Add(new AssetCollectionBuilderElementAll());
        return this;
    }

    public AssetCollectionBuilderInstance AllInGroup(string group) {
        newElements.Add(new AssetCollectionBuilderElementAllIn(group, string.Empty, []));
        return this;
    }

    public AssetCollectionBuilderInstance AllInGroupPath(string group, string path) {
        newElements.Add(new AssetCollectionBuilderElementAllIn(group, path, []));
        return this;
    }

    public AssetCollectionBuilderInstance AllInGroupPathExtension(string group, string path, params string[] extensions) {
        newElements.Add(new AssetCollectionBuilderElementAllIn(group, path, extensions));
        return this;
    }

    public AssetCollectionBuilderInstance AllInGroupExtension(string group, params string[] extensions) {
        newElements.Add(new AssetCollectionBuilderElementAllIn(group, string.Empty, extensions));
        return this;
    }

    public AssetCollectionBuilderInstance Single(string group, string name, string extension)
        => Single(new(group, name, extension));

    public AssetCollectionBuilderInstance Single(AssetProviderItem item) {
        newElements.Add(new AssetCollectionBuilderSingle(item));
        return this;
    }

    public AssetCollectionBuilderInstance Multiple(params AssetProviderItem[] items) {
        newElements.Add(new AssetCollectionBuilderMultiple(items));
        return this;
    }

    public AssetCollectionBuilder[] Finish() {
        AddBuilder();
        return [.. builders];
    }

    void AddBuilder() {
        if (newElements.Count > 0) { builders.Add(new AssetCollectionBuilder(newName, [.. newElements])); }
    }

    public static implicit operator AssetCollectionBuilder[](AssetCollectionBuilderInstance instance) => instance.Finish();
}

internal interface IAssetCollectionBuilderElement {
    public AssetProviderItem[] Filter(AssetProviderItem[] items);
}

internal struct AssetCollectionBuilderElementAll : IAssetCollectionBuilderElement {
    public readonly AssetProviderItem[] Filter(AssetProviderItem[] items) => items;
}

internal struct AssetCollectionBuilderElementAllIn(string group, string path, string[] extensions) : IAssetCollectionBuilderElement {
    public readonly AssetProviderItem[] Filter(AssetProviderItem[] items) {
        // CS1673.... hm
        string compareGroup = group;
        string comparePath = path;
        string[] compareExtensions = extensions;
        return [.. items.Where(e => Compare(e, compareGroup, comparePath, compareExtensions))];
    }

    static bool Compare(AssetProviderItem item, string group, string path, string[] extensions) {
        return
            item.Group == group &&
            (path == string.Empty || item.Name.StartsWith(path)) &&
            (extensions.Length == 0 || extensions.Contains(item.Extension));
    }
}

internal struct AssetCollectionBuilderSingle(AssetProviderItem asset) : IAssetCollectionBuilderElement {
    public readonly AssetProviderItem[] Filter(AssetProviderItem[] items) => items.Contains(asset) ? [asset] : [];
}

internal struct AssetCollectionBuilderMultiple(AssetProviderItem[] assets) : IAssetCollectionBuilderElement {
    public readonly AssetProviderItem[] Filter(AssetProviderItem[] items) => [.. assets.Where(e => items.Contains(e))];
}