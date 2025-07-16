namespace LittleLib.Loader.LDTK;

public class LDTKTile() {
    readonly List<LDTKTileElement> Elements = [];
    public Sprite? Sprite;
    public ICollider? Collider;

    public LDTKTile(LDTKTileElement element) : this() {
        Add(element);
    }

    public bool Empty => Elements.Count == 0;

    public LDTKTile(params LDTKTileElement[] elements) : this() {
        foreach (LDTKTileElement element in elements) {
            Add(element);
        }
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

    void SetValues() {
        Sprite[] sprites = [.. Elements.Where(e => e.Sprite != null).Select(e => e.Sprite)];
        if (sprites.Length == 0) { Sprite = null; }
        else if (sprites.Length == 1) { Sprite = sprites[0]; }
        else { Sprite = new SpriteStack(sprites); }

        ICollider[] colliders = [.. Elements.Where(e => e.Collider != null).Select(e => e.Collider)];
        if (colliders.Length == 0) { Collider = null; }
        else if (colliders.Length == 1) { Collider = colliders[0]; }
        else { Collider = new MultiCollider(colliders); }
    }
}

public class LDTKTileElement() {
    public int ID;
    public Sprite? Sprite;
    public ICollider? Collider;
    public Dictionary<string, string> CustomData = [];
}