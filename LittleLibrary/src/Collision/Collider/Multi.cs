using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class MultiCollider(params Collider[] colliders) : Collider {
    readonly Collider[] Colliders = colliders;

    public override bool Multi => true;
    public override Collider[] GetSubColliders(Rect bounds) => Colliders;

    public override Rect Bounds { get; } = new BoundsBuilder(colliders.Select(e => e.Bounds).ToArray()).Rect;
    public override Vector2 Center => throw new NotImplementedException();

    public override Vector2 Support(Vector2 position, Vector2 direction) {
        throw new NotImplementedException();
    }
}