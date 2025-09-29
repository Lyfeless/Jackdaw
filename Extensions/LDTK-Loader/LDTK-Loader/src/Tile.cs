namespace Jackdaw.Loader.LDTK;

/// <summary>
/// An instance of a single tile loaded from an LDTK level.
/// Natively supports sprite rendering and tile collision.
/// </summary>
public class LDTKTile() {
    readonly List<LDTKTileElement> Elements = [];

    /// <summary>
    /// The sprite the tile should render as.
    /// </summary>
    public Sprite? Sprite;

    /// <summary>
    /// The tile's collider.
    /// </summary>
    public Collider? Collider;

    /// <summary>
    /// If the tile has no layered elements.
    /// </summary>
    public bool Empty => Elements.Count == 0;

    int Flip;

    /// <summary>
    /// An instance of a single tile loaded from an LDTK level.
    /// Natively supports sprite rendering and tile collision.
    /// </summary>
    /// <param name="element">The tile element data.</param>
    /// <param name="flip">The tile's flip, stored as X on the first bit and Y on the second bit.</param>
    public LDTKTile(LDTKTileElement element, int flip = 0) : this() {
        Add(element);
        Flip = flip;
    }

    /// <summary>
    /// An instance of a single tile loaded from an LDTK level.
    /// Natively supports sprite rendering and tile collision.
    /// </summary>
    /// <param name="elements">The tile elements in the tile's stack.</param>
    /// <param name="flip">The tile's flip, stored as X on the first bit and Y on the second bit.</param>
    public LDTKTile(LDTKTileElement[] elements, int flip = 0) : this() {
        foreach (LDTKTileElement element in elements) {
            Add(element);
        }

        Flip = flip;
    }

    /// <summary>
    /// Add a tile element to the element stack.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public bool Add(LDTKTileElement element) {
        Elements.Add(element);
        SetValues();
        return true;
    }

    /// <summary>
    /// Remove the top element in the element stack.
    /// </summary>
    /// <returns></returns>
    public bool Remove() {
        if (Empty) { return false; }

        Elements.RemoveAt(Elements.Count - 1);
        SetValues();
        return true;
    }

    /// <summary>
    /// Remove the element at a specified index in the element stack.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool RemoveAt(int index) {
        if (Elements.Count <= index) { return false; }
        Elements.RemoveAt(index);
        SetValues();
        return true;
    }

    /// <summary>
    /// The number of elements in the tile's element stack.
    /// </summary>
    public int ElementCount => Elements.Count;

    /// <summary>
    /// Get the element at a specific index in the element stack.
    /// </summary>
    /// <param name="index">The index to get the element at.</param>
    /// <returns>The element at the index, null if the index is out of range.</returns>
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

        if (Sprite != null) {
            Sprite.FlipX = (Flip & 1 << 0) != 0;
            Sprite.FlipY = (Flip & 1 << 1) != 0;
        }
    }
}

/// <summary>
/// A single element in a tile's element stack.
/// </summary>
public class LDTKTileElement() {
    /// <summary>
    /// The element's id value.
    /// </summary>
    public int ID;

    /// <summary>
    /// The sprite the element should render as.
    /// </summary>
    public Sprite? Sprite;

    /// <summary>
    /// The element's collider.
    /// </summary>
    public Collider? Collider;

    /// <summary>
    /// The element's custom enum values.
    /// </summary>
    public string[] EnumValues = [];

    /// <summary>
    /// Any custom string data added to the tile element.
    /// </summary>
    public Dictionary<string, string> CustomData = [];

    /// <summary>
    /// Get custom data from a specific data identifier.
    /// </summary>
    /// <param name="ID">The identifier to get the data at.</param>
    /// <returns>The data from the id, empty if no data is found.</returns>
    public string GetCustomData(string ID) {
        if (!CustomData.TryGetValue(ID, out string? value)) { return string.Empty; }
        return value;
    }
}