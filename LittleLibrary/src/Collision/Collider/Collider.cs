using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public abstract class Collider {
    public abstract Rect Bounds { get; }

    public abstract bool Overlaps(Collider with, out Vector2 pushout);
}