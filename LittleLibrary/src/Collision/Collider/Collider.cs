using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public abstract class Collider {
    public TagContainer Tags = new();
    public TagContainer Mask = new();

    public abstract bool Multi { get; }

    // Return all useful subcolliders for a given bound, local to collider
    public abstract Collider[] GetSubColliders(Rect bounds);

    public abstract Rect Bounds { get; }
    public abstract Vector2 Center { get; }

    public abstract Vector2 Support(Vector2 position, Vector2 direction);
}