using System.Numerics;
using Foster.Framework;

namespace LittleLib;

/// <summary>
/// A collider made up of multiple colliders.
/// </summary>
/// <param name="colliders">A list of colliders.</param>
public class MultiCollider(params Collider[] colliders) : Collider {
    public readonly Collider[] Colliders = colliders;

    public override bool Multi => true;
    public override Collider[] GetSubColliders(Rect bounds) => Colliders;

    public override Rect Bounds { get; } = new BoundsBuilder(colliders.Select(e => e.Bounds).ToArray()).Rect;
    public override Vector2 Center => throw new NotImplementedException();

    public override Vector2 Support(Vector2 direction) {
        throw new NotImplementedException();
    }
}