namespace Jackdaw;

/// <summary>
/// A structure for storing and modifying multiple individual bit flags. <br/>
/// Capable of storing up to 64 tags values.
/// </summary>
public struct TagContainer() {
    /// <summary>
    /// All tags stored in the container.
    /// </summary>
    public long Tags;

    /// <summary>
    /// If the container has no tags set.
    /// </summary>
    public readonly bool Empty => Tags == 0;

    /// <summary>
    /// Create a container pre-filled with tag values.
    /// </summary>
    /// <param name="values">Tag values as an enum, all enum values need to be ints between 0 and 63 to function as intended.</param>
    public TagContainer(params Enum[] values) : this() => Set(values);

    /// <summary>
    /// Create a container pre-filled with tag values.
    /// </summary>
    /// <param name="values">Tag values as a ints, all ints values need to be ints between 0 and 63 to function as intended.</param>
    public TagContainer(params int[] values) : this() => Set(values);

    /// <summary>
    /// Set all tag values. All provided values will be true, any not included will be false.
    /// </summary>
    /// <param name="values">Tag values as an enum, all enum values need to be ints between 0 and 63 to function as intended.</param>
    public void Set(params Enum[] values) => Set(EnumToTags(values));

    /// <summary>
    /// Set all tag values. All provided values will be true, any not included will be false.
    /// </summary>
    /// <param name="values">Tag values as a ints, all ints values need to be ints between 0 and 63 to function as intended.</param>
    public void Set(params int[] values) => Set(IntToTags(values));

    /// <summary>
    /// Set all tag values. All provided values will be true, any not included will be false.
    /// </summary>
    /// <param name="tags">Tag values as a bit field, every bit in the long will be treated as a tag.</param>
    public void Set(long tags) {
        Tags = tags;
    }

    //! FIXME (Alex): Test all the add and remove functions

    /// <summary>
    /// Set all given tag values to true, leaving all other values unchanged.
    /// </summary>
    /// <param name="tags">Tag values as an enum, all enum values need to be ints between 0 and 63 to function as intended.</param>
    public void Add(params Enum[] tags) => Add(EnumToTags(tags));

    /// <summary>
    /// Set all given tag values to true, leaving all other values unchanged.
    /// </summary>
    /// <param name="tags">Tag values as a ints, all ints values need to be ints between 0 and 63 to function as intended.</param>
    public void Add(params int[] tags) => Add(IntToTags(tags));

    /// <summary>
    /// Set all given tag values to true, leaving all other values unchanged.
    /// </summary>
    /// <param name="tags">Tag values as a bit field, every bit in the long will be treated as a tag.</param>
    public void Add(long tags) {
        Tags |= tags;
    }

    /// <summary>
    /// Set all given tag values to false, leaving all other values unchanged.
    /// </summary>
    /// <param name="tags">Tag values as an enum, all enum values need to be ints between 0 and 63 to function as intended.</param>
    public void Remove(params Enum[] tags) => Remove(EnumToTags(tags));

    /// <summary>
    /// Set all given tag values to false, leaving all other values unchanged.
    /// </summary>
    /// <param name="tags">Tag values as a ints, all ints values need to be ints between 0 and 63 to function as intended.</param>
    public void Remove(params int[] tags) => Remove(IntToTags(tags));

    /// <summary>
    /// Set all given tag values to false, leaving all other values unchanged.
    /// </summary>
    /// <param name="tags">Tag values as a bit field, every bit in the long will be treated as a tag.</param>
    public void Remove(long tags) {
        Tags &= ~tags;
    }

    public void Change(Enum tag, bool value) {
        throw new NotImplementedException();
    }

    public void Change(int tag, bool value) {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Check if any tags match between two tag containers.
    /// </summary>
    /// <param name="container">The tag container to compare with.</param>
    /// <returns>If any of the tags in the containers match.</returns>
    public readonly bool Any(TagContainer container) => Any(container.Tags);

    /// <summary>
    /// Check if any of the given tags match the current tags.
    /// </summary>
    /// <param name="values">Tag values as an enum, all enum values need to be ints between 0 and 63 to function as intended.</param>
    /// <returns>If any of the tags match.</returns>
    public readonly bool Any(params Enum[] values) => Any(EnumToTags(values));

    /// <summary>
    /// Check if any of the given tags match the current tags.
    /// </summary>
    /// <param name="values">Tag values as a ints, all ints values need to be ints between 0 and 63 to function as intended.</param>
    /// <returns>If any of the tags match.</returns>
    public readonly bool Any(params int[] values) => Any(IntToTags(values));

    /// <summary>
    /// Check if any of the given tags match the current tags.
    /// </summary>
    /// <param name="tags">Tag values as a bit field, every bit in the long will be treated as a tag.</param>
    /// <returns>If any of the tags match.</returns>
    public readonly bool Any(long tags) {
        return (tags & Tags) != 0;
    }

    /// <summary>
    /// Check if all tags match between two tag containers.
    /// </summary>
    /// <param name="container">The tag container to compare with.</param>
    /// <returns>If all of the tags in the containers match.</returns>
    public readonly bool All(TagContainer container) => All(container.Tags);

    /// <summary>
    /// Check if all of the given tags match the current tags.
    /// </summary>
    /// <param name="values">Tag values as an enum, all enum values need to be ints between 0 and 63 to function as intended.</param>
    /// <returns>If any of the tags match.</returns>
    public readonly bool All(params Enum[] values) => All(EnumToTags(values));

    /// <summary>
    /// Check if all of the given tags match the current tags.
    /// </summary>
    /// <param name="values">Tag values as a ints, all ints values need to be ints between 0 and 63 to function as intended.</param>
    /// <returns>If any of the tags match.</returns>
    public readonly bool All(params int[] values) => All(IntToTags(values));

    /// <summary>
    /// Check if all of the given tags match the current tags.
    /// </summary>
    /// <param name="tags">Tag values as a bit field, every bit in the long will be treated as a tag.</param>
    /// <returns>If any of the tags match.</returns>
    public readonly bool All(long tags) {
        return (tags & Tags) == tags;
    }

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

    public override readonly string ToString() => Convert.ToString(Tags, 2);
}