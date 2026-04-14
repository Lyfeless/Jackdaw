namespace Jackdaw;

public readonly struct AssetCollection(string name, AssetProviderItem[] items) {
    public readonly string Name = name;
    public readonly AssetProviderItem[] Items = items;

    public AssetProviderItem[] Filter(string group)
        => Filter(group, []);

    public AssetProviderItem[] Filter(string group, params string[] extensions) {
        return [..
            Items.Where(e =>
                e.Group == group &&
                (extensions.Length == 0 || extensions.Contains(e.Extension))
            )
        ];
    }
}