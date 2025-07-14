using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class RectangleCollider(Rect rect) : ICollider {
    public Rect Rect = rect;

    public RectangleCollider(Vector2 size) : this(new Rect(Vector2.Zero, size)) { }
    public RectangleCollider(Vector2 position, Vector2 size) : this(new Rect(position, size)) { }

    public Rect Bounds => Rect;

    public Vector2 Center => Rect.Center;

    public bool Multi => false;
    public ICollider[] GetSubColliders(Rect bounds) => [this];

    public Vector2 Support(Vector2 position, Vector2 direction) {
        Vector2 halfRect = Rect.Size / 2;
        return position + Rect.Position + halfRect + (halfRect * new Vector2(MathF.Sign(direction.X), MathF.Sign(direction.Y)));
    }
}