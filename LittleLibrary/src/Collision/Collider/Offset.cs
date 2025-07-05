using System.Numerics;
using Foster.Framework;

namespace LittleLib;

//! FIXME (Alex): Not the most elegant thing ever
public class OffsetCollider(Collider collider, Vector2 offset) : Collider() {
    readonly Collider Collider = collider;
    readonly Vector2 Offset = offset;

    public override Rect Bounds => new(Collider.Bounds.Position + Offset, Collider.Bounds.Size);
    public override Vector2 Center => Collider.Bounds.Center + Offset;

    public override bool Multi => Collider.Multi;
    //! FIXME (Alex): Does this work?
    public override Collider[] GetSubColliders(Collider collider, Vector2 position)
        => [.. Collider.GetSubColliders(collider, position - Offset).Select(e => new OffsetCollider(e, Offset))];

    public override Vector2 Support(Vector2 position, Vector2 direction) => Collider.Support(position + Offset, direction);
}