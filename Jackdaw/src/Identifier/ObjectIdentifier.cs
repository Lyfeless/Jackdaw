namespace Jackdaw;

/// <summary>
/// Identification system for objects allowing several different methods of searching for matching elements.
/// </summary>
/// <typeparam name="T">The type of object to identify.</typeparam>
/// <param name="obj">The object to identify.</param>
/// <param name="guid">A randomly generated unique identifier.</param>
/// <param name="name">A string identifier for the object.</param>
public struct ObjectIdentifier<T>(T obj, Guid guid, string? name = null) where T : class {
    /// <summary>
    /// A unique identifier.
    /// </summary>
    public Guid Guid = guid;

    /// <summary>
    /// An assignable custom name for easier human-readable searching.
    /// Set to null if no name is assigned.
    /// </summary>
    public string? Name = name;

    /// <summary>
    /// A container object used for storing assignable tags.
    /// </summary>
    public TagContainer Tags = new();

    /// <summary>
    /// The object owning the identifiers.
    /// </summary>
    readonly T Obj = obj;

    /// <summary>
    /// The type of the owning object.
    /// </summary>
    readonly Type ObjType = obj.GetType();

    /// <summary>
    /// Create a new Identifier with an auto-generated uuid.
    /// </summary>
    /// <param name="obj">The owning object.</param>
    /// <param name="name">A cusom identifier name. Defaults to null.</param>
    public ObjectIdentifier(T obj, string? name = null) : this(obj, Guid.NewGuid(), name) { }

    /// <summary>
    /// Check if a guid matches the object.
    /// </summary>
    /// <param name="guid">The guid to compare with.</param>
    /// <returns>If the guid matches.</returns>
    public readonly bool ByGuid(Guid guid) {
        return guid == Guid;
    }

    /// <summary>
    /// Check if a name matches the object.
    /// </summary>
    /// <param name="name">The name to compare with.</param>
    /// <returns>If the name matches.</returns>
    public readonly bool ByName(string name) {
        return name == Name;
    }

    /// <summary>
    /// Check if a type matches the object.
    /// </summary>
    /// <param name="type">The type to compare with.</param>
    /// <returns>If the type matches.</returns>
    public readonly bool ByType(Type type) {
        return type == ObjType;
    }

    /// <summary>
    /// Check if a type matches the object.
    /// </summary>
    /// <typeparam name="Tcheck">The type to compare with.</typeparam>
    /// <returns>If the type matches.</returns>
    public readonly bool ByType<Tcheck>() {
        return typeof(Tcheck) == ObjType;
    }

    /// <summary>
    /// Check if an object matches the current object.
    /// </summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns>If the object matches.</returns>
    public readonly bool ByObject(T obj) {
        return obj == Obj;
    }

    /// <summary>
    /// Check if the object has any of the given tags.
    /// </summary>
    /// <param name="tags">The tags to compare with, stored as individual bits.</param>
    /// <returns>If any of the tags match.</returns>
    public readonly bool ByAnyTags(long tags) {
        return Tags.Any(tags);
    }

    /// <summary>
    /// Check if the object has any of the given tags.
    /// </summary>
    /// <param name="values">The tags to compare with.</param>
    /// <returns>If any of the tags match.</returns>
    public readonly bool ByAnyTags(params int[] values) {
        return Tags.Any(values);
    }

    /// <summary>
    /// Check if the object has any of the given tags.
    /// </summary>
    /// <param name="values">The tags to compare with.</param>
    /// <returns>If any of the tags match.</returns>
    public readonly bool ByAnyTags(params Enum[] values) {
        return Tags.Any(values);
    }

    /// <summary>
    /// Check if the object has all of the given tags.
    /// </summary>
    /// <param name="tags">The tags to compare with, stored as individual bits.</param>
    /// <returns>If all of the tags match.</returns>
    public readonly bool ByAllTags(long tags) {
        return Tags.All(tags);
    }

    /// <summary>
    /// Check if the object has all of the given tags.
    /// </summary>
    /// <param name="values">The tags to compare with.</param>
    /// <returns>If all of the tags match.</returns>
    public readonly bool ByAllTags(params int[] values) {
        return Tags.All(values);
    }

    /// <summary>
    /// Check if the object has all of the given tags.
    /// </summary>
    /// <param name="values">The tags to compare with.</param>
    /// <returns>If all of the tags match.</returns>
    public readonly bool ByAllTags(params Enum[] values) {
        return Tags.All(values);
    }

    public override readonly string ToString() {
        return Name ?? $"{ObjType} {Guid}";
    }
}