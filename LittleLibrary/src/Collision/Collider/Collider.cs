using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public abstract class Collider {
    public abstract bool Multi { get; }
    public abstract Collider[] GetSubColliders(Collider collider, Vector2 position);

    public abstract Rect Bounds { get; }
    public abstract Vector2 Center { get; }

    public abstract Vector2 Support(Vector2 position, Vector2 direction);
}