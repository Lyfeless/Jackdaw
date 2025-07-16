using System.Numerics;
using Foster.Framework;

namespace LittleLib;

//! FIXME (Alex): Not the most elegant thing ever
public class OffsetCollider : Collider {
    readonly Collider Collider;
    readonly Vector2 Offset;

    public OffsetCollider(Collider collider, Vector2 offset) {
        Collider = collider;
        Offset = offset;
        Tags = collider.Tags;
        Mask = collider.Mask;
    }

    public override Rect Bounds => new(Collider.Bounds.Position + Offset, Collider.Bounds.Size);
    public override Vector2 Center => Collider.Bounds.Center + Offset;

    public override bool Multi => Collider.Multi;

    public override Collider[] GetSubColliders(Rect bounds) {
        if (!Collider.Multi) { return [this]; }
        return [.. Collider.GetSubColliders(new(bounds.Position - Offset, bounds.Size)).Select(e => new OffsetCollider(e, Offset))];
    }

    public override Vector2 Support(Vector2 position, Vector2 direction) => Collider.Support(position + Offset, direction);
}