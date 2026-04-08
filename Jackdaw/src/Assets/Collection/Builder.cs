namespace Jackdaw;


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
        return new([.. filtered]);
    }

    public static AssetCollectionBuilder[] FromAll()
        => [SingleFromAll()];

    public static AssetCollectionBuilder SingleFromAll()
        => new(string.Empty, new AssetCollectionBuilderElementAll());

    public static AssetCollectionBuilder[] FromFile(string path) {
        throw new NotImplementedException();
    }

    public static AssetCollectionBuilder[] FromFiles(params string[] paths) {
        throw new NotImplementedException();
    }

    public static AssetCollectionBuilder[] FromProviderFile(AssetProviderItem item) {
        throw new NotImplementedException();
    }

    public static AssetCollectionBuilder[] FromProviderFiles(params AssetProviderItem[] items) {
        throw new NotImplementedException();
    }

    public static AssetCollectionBuilderInstance NewCollection(string name)
        => new(name);
}

public class AssetCollectionBuilderInstance {
    readonly List<AssetCollectionBuilder> builders = [];

    string newName = string.Empty;
    readonly List<IAssetCollectionBuilderElement> newElements = [];

    internal AssetCollectionBuilderInstance(string name) {
        NewCollection(name);
    }

    public AssetCollectionBuilderInstance NewCollection(string name) {
        if (newElements.Count > 0) { builders.Add(new AssetCollectionBuilder(newName, [.. newElements])); }
        newName = name;
        newElements.Clear();
        return this;
    }

    public AssetCollectionBuilderInstance All() {
        newElements.Add(new AssetCollectionBuilderElementAll());
        return this;
    }

    public AssetCollectionBuilderInstance AllWithin(string group, string path) {
        newElements.Add(new AssetCollectionBuilderElementAllWithin(group, path, []));
        return this;
    }

    public AssetCollectionBuilderInstance AllWithin(string group, string path, params string[] extensions) {
        newElements.Add(new AssetCollectionBuilderElementAllWithin(group, path, extensions));
        return this;
    }

    public AssetCollectionBuilderInstance Single(AssetProviderItem item) {
        newElements.Add(new AssetCollectionBuilderSingle(item));
        return this;
    }

    public AssetCollectionBuilderInstance Multiple(params AssetProviderItem[] items) {
        newElements.Add(new AssetCollectionBuilderMultiple(items));
        return this;
    }

    public AssetCollectionBuilder[] Finish()
        => [.. builders];

    public static implicit operator AssetCollectionBuilder[](AssetCollectionBuilderInstance instance) => instance.Finish();
}

internal interface IAssetCollectionBuilderElement {
    public AssetProviderItem[] Filter(AssetProviderItem[] items);
}

internal struct AssetCollectionBuilderElementAll : IAssetCollectionBuilderElement {
    public readonly AssetProviderItem[] Filter(AssetProviderItem[] items) => items;
}

internal struct AssetCollectionBuilderElementAllWithin(string group, string path, string[] extensions) : IAssetCollectionBuilderElement {
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
            item.Name.StartsWith(path) &&
            (extensions.Length == 0 || extensions.Contains(item.Extension));
    }
}

internal struct AssetCollectionBuilderSingle(AssetProviderItem asset) : IAssetCollectionBuilderElement {
    public readonly AssetProviderItem[] Filter(AssetProviderItem[] items) => items.Contains(asset) ? [asset] : [];
}

internal struct AssetCollectionBuilderMultiple(AssetProviderItem[] assets) : IAssetCollectionBuilderElement {
    public readonly AssetProviderItem[] Filter(AssetProviderItem[] items) => [.. assets.Where(e => items.Contains(e))];
}