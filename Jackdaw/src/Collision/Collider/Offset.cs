using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A collider used to offset an existing collider.
/// Mostly used for internal systems.
/// </summary>
public class OffsetCollider : Collider {
    /// <summary>
    /// The original collider.
    /// </summary>
    public readonly Collider Collider;

    /// <summary>
    /// The offset to apply the the collider's position.
    /// </summary>
    public readonly Vector2 Offset;

    /// <summary>
    /// Create an offset collider from an existing collider.
    /// </summary>
    /// <param name="collider">The collider to offset.</param>
    /// <param name="offset">The offset amount.</param>
    public OffsetCollider(Collider collider, Vector2 offset) {
        Collider = collider;
        Offset = offset;
        Tags = collider.Tags;
        Mask = collider.Mask;
    }

    public override Rect Bounds => Collider.Bounds.Translate(Offset);
    public override Vector2 Center => Collider.Bounds.Center + Offset;

    public override bool Multi => Collider.Multi;

    public override Collider[] GetSubColliders(Rect bounds) {
        if (!Collider.Multi) { return [this]; }
        return [.. Collider.GetSubColliders(new(bounds.Position - Offset, bounds.Size)).Select(e => new OffsetCollider(e, Offset))];
    }

    public override Vector2 Support(Vector2 direction, InvertableMatrix position)
        => GetGlobalPoint(Offset + Collider.Support(GetLocalDirection(direction, position), Matrix3x2.Identity), position);
}