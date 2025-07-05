using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class RectangleCollider(Rect rect) : Collider() {
    public Rect Rect = rect;

    public RectangleCollider(Vector2 size) : this(new Rect(Vector2.Zero, size)) { }
    public RectangleCollider(Vector2 position, Vector2 size) : this(new Rect(position, size)) { }

    public override Rect Bounds => Rect;

    public override Vector2 Center => Rect.Center;

    public override bool Multi => false;
    public override Collider[] GetSubColliders(Collider collider, Vector2 position) => [this];

    public override Vector2 Support(Vector2 position, Vector2 direction) {
        Vector2 halfRect = Rect.Size / 2;
        return position + Rect.Position + halfRect + (halfRect * new Vector2(MathF.Sign(direction.X), MathF.Sign(direction.Y)));
    }
}