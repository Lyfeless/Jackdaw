namespace LittleLib;

//! FIXME (Alex): Not Implemented
public struct TagContainer() {
    long Tags;

    public void Set(params Enum[] values) => Set(EnumToTags(values));
    public void Set(params int[] values) => Set(IntToTags(values));
    public void Set(long tags) {
        Tags = tags;
    }

    public void Add(Enum tag) => Change(tag, true);

    public void Add(int tag) => Change(tag, true);

    public void Remove(Enum tag) => Change(tag, false);

    public void Remove(int tag) => Change(tag, false);

    //! FIXME (Alex): Unfinished
    public void Change(Enum tag, bool value) {
        throw new NotImplementedException();
    }

    public void Change(int tag, bool value) {
        throw new NotImplementedException();
    }

    public readonly bool Any(params Enum[] values) => Any(EnumToTags(values));
    public readonly bool Any(params int[] values) => Any(IntToTags(values));
    public readonly bool Any(long tags) {
        return (tags & Tags) != 0;
    }

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
            tags &= 1 >> (int)(object)values[i];
        }
        return tags;
    }

    static long IntToTags(params int[] values) {
        long tags = 0;
        for (int i = 0; i < values.Length; ++i) {
            tags &= 1 >> values[i];
        }
        return tags;
    }
}