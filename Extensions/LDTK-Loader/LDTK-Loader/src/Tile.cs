namespace LittleLib.Loader.LDTK;

/// <summary>
/// An instance of a single tile loaded from an LDTK level.
/// Natively supports sprite rendering and tile collision.
/// </summary>
public class LDTKTile() {
    readonly List<LDTKTileElement> Elements = [];
    public Sprite? Sprite;
    public Collider? Collider;

    //! FIXME (Alex): Flip should probably be more editable?
    int Flip;
    public bool Empty => Elements.Count == 0;

    public LDTKTile(LDTKTileElement element, int flip = 0) : this() {
        Add(element);
        Flip = flip;
    }

    public LDTKTile(LDTKTileElement[] elements, int flip = 0) : this() {
        foreach (LDTKTileElement element in elements) {
            Add(element);
        }

        Flip = flip;
    }

    public bool Add(LDTKTileElement element) {
        Elements.Add(element);
        SetValues();
        return true;
    }

    public bool Remove() {
        if (Empty) { return false; }

        Elements.RemoveAt(Elements.Count - 1);
        SetValues();
        return true;
    }
    public bool RemoveAt(int index) {
        if (Elements.Count <= index) { return false; }
        Elements.RemoveAt(index);
        SetValues();
        return true;
    }

    public int ElementCount => Elements.Count;
    public LDTKTileElement? Element(int index) {
        if (index < 0 || index >= Elements.Count) { return null; }
        return Elements[index];
    }

    void SetValues() {
        Sprite[] sprites = [.. Elements.Where(e => e.Sprite != null).Select(e => e.Sprite)];
        if (sprites.Length == 0) { Sprite = null; }
        else if (sprites.Length == 1) { Sprite = sprites[0]; }
        else { Sprite = new SpriteStack(sprites); }

        Collider[] colliders = [.. Elements.Where(e => e.Collider != null).Select(e => e.Collider)];
        if (colliders.Length == 0) { Collider = null; }
        else if (colliders.Length == 1) { Collider = colliders[0]; }
        else { Collider = new MultiCollider(colliders); }

        //! FIXME (Alex): Untested
        //! FIXME (Alex): Sprite flipping should be able to flip the collider as well
        if (Sprite != null) {
            Sprite.FlipX = (Flip & 1 << 0) != 0;
            Sprite.FlipY = (Flip & 1 << 1) != 0;
        }
    }
}

public class LDTKTileElement() {
    public int ID;
    public Sprite? Sprite;
    public Collider? Collider;
    public string[] EnumValues = [];
    public Dictionary<string, string> CustomData = [];

    public string GetCustomData(string ID) {
        if (!CustomData.TryGetValue(ID, out string? value)) { return string.Empty; }
        return value;
    }
}