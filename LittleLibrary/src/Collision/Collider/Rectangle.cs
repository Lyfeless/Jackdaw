using System.Numerics;
using Foster.Framework;

namespace LittleLib;

/// <summary>
/// A rectangle-shaped collider.
/// </summary>
/// <param name="rect"></param>
public class RectangleCollider(Rect rect) : Collider {
    public Rect Rect = rect;

    public RectangleCollider(Vector2 size) : this(new Rect(Vector2.Zero, size)) { }
    public RectangleCollider(Vector2 position, Vector2 size) : this(new Rect(position, size)) { }

    public override Rect Bounds => Rect;

    public override Vector2 Center => Rect.Center;

    public override bool Multi => false;
    public override Collider[] GetSubColliders(Rect bounds) => [this];

    public override Vector2 Support(Vector2 direction) {
        Vector2 halfRect = Rect.Size / 2;
        return Rect.Position + halfRect + (halfRect * new Vector2(MathF.Sign(direction.X), MathF.Sign(direction.Y)));
    }
}