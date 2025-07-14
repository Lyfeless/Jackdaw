using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class MultiCollider(params ICollider[] colliders) : ICollider {
    public readonly ICollider[] Colliders = colliders;

    public bool Multi => true;
    public ICollider[] GetSubColliders(Rect bounds) => Colliders;

    public Rect Bounds { get; } = new BoundsBuilder(colliders.Select(e => e.Bounds).ToArray()).Rect;
    public Vector2 Center => throw new NotImplementedException();

    public Vector2 Support(Vector2 position, Vector2 direction) {
        throw new NotImplementedException();
    }
}