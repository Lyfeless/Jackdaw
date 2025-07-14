namespace LittleLib;

public struct ObjectIdentifier<T>(T obj, Guid guid, string? name = null) where T : class {
    public Guid Guid = guid;
    public string? Name = name;
    T Obj = obj;
    Type ObjType = obj.GetType();
    TagContainer Tags = new();

    public ObjectIdentifier(T obj, string? name = null) : this(obj, Guid.NewGuid(), name) { }

    public readonly bool ByGuid(Guid guid) {
        return guid == Guid;
    }

    public readonly bool ByName(string name) {
        return name == Name;
    }

    public readonly bool ByType(Type type) {
        return type == ObjType;
    }

    public readonly bool ByType<Tcheck>() {
        return typeof(Tcheck) == ObjType;
    }

    public readonly bool ByObject(T obj) {
        return obj == Obj;
    }

    public readonly bool ByAnyTags(long tags) {
        return Tags.Any(tags);
    }

    public readonly bool ByAnyTags(params int[] values) {
        return Tags.Any(values);
    }

    public readonly bool ByAnyTags(params Enum[] values) {
        return Tags.Any(values);
    }

    public readonly bool ByAllTags(long tags) {
        return Tags.All(tags);
    }

    public readonly bool ByAllTags(params int[] values) {
        return Tags.All(values);
    }

    public readonly bool ByAllTags(params Enum[] values) {
        return Tags.All(values);
    }

    public override readonly string ToString() {
        return Name ?? Guid.ToString();
    }
}