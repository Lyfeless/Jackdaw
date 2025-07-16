namespace LittleLib;

public struct TagContainer() {
    public long Tags;
    public readonly bool Empty => Tags == 0;

    public TagContainer(params Enum[] values) : this() => Set(values);
    public TagContainer(params int[] values) : this() => Set(values);

    public void Set(params Enum[] values) => Set(EnumToTags(values));
    public void Set(params int[] values) => Set(IntToTags(values));
    public void Set(long tags) {
        Tags = tags;
    }

    //! FIXME (Alex): params variations of Add and Remove
    public void Add(Enum tag) => Add((int)(object)tag);
    public void Add(int tag) {
        Tags |= Single(tag);
    }

    public void Remove(Enum tag) => Remove((int)(object)tag);
    public void Remove(int tag) {
        Tags &= ~Single(tag);
    }

    //! FIXME (Alex): Unfinished
    public void Change(Enum tag, bool value) {
        throw new NotImplementedException();
    }

    public void Change(int tag, bool value) {
        throw new NotImplementedException();
    }

    public readonly bool Any(TagContainer container) => Any(container.Tags);
    public readonly bool Any(params Enum[] values) => Any(EnumToTags(values));
    public readonly bool Any(params int[] values) => Any(IntToTags(values));
    public readonly bool Any(long tags) {
        return (tags & Tags) != 0;
    }

    public readonly bool All(TagContainer container) => All(container.Tags);
    public readonly bool All(params Enum[] values) => All(EnumToTags(values));
    public readonly bool All(params int[] values) => All(IntToTags(values));
    public readonly bool All(long tags) {
        return (tags & Tags) == tags;
    }

    //! FIXME (Alex): Don't want to implment these until I'm sure they'll get used
    public int[] TagsAsInt() {
        throw new NotImplementedException();
    }

    public T[] TagsAsEnum<T>() where T : Enum {
        throw new NotImplementedException();
    }

    static long EnumToTags(params Enum[] values) {
        long tags = 0;
        for (int i = 0; i < values.Length; ++i) {
            tags |= Single((int)(object)values[i]);
        }
        return tags;
    }

    static long IntToTags(params int[] values) {
        long tags = 0;
        for (int i = 0; i < values.Length; ++i) {
            tags |= Single(values[i]);
        }
        return tags;
    }

    static long Single(int value) {
        return (long)1 << value;
    }

    public override string ToString() => Convert.ToString(Tags, 2);
}