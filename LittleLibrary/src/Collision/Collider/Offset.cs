using System.Numerics;
using Foster.Framework;

namespace LittleLib;

//! FIXME (Alex): Not the most elegant thing ever
public class OffsetCollider(ICollider collider, Vector2 offset) : ICollider {
    readonly ICollider Collider = collider;
    readonly Vector2 Offset = offset;

    public Rect Bounds => new(Collider.Bounds.Position + Offset, Collider.Bounds.Size);
    public Vector2 Center => Collider.Bounds.Center + Offset;

    public bool Multi => Collider.Multi;

    public ICollider[] GetSubColliders(Rect bounds) {
        if (!Collider.Multi) { return [this]; }
        return [.. Collider.GetSubColliders(new(bounds.Position - Offset, bounds.Size)).Select(e => new OffsetCollider(e, Offset))];
    }

    public Vector2 Support(Vector2 position, Vector2 direction) => Collider.Support(position + Offset, direction);
}