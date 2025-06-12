using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class RectangleCollider(Rect rect) : ConvexCollider(rect) {
    public RectangleCollider(Vector2 size) : this(new Rect(Vector2.Zero, size)) { }
    public RectangleCollider(Vector2 position, Vector2 size) : this(new Rect(position, size)) { }

    public override Collider Offset(Vector2 amount) {
        return new RectangleCollider(((Rect)Shape).Translate(amount));
    }
}