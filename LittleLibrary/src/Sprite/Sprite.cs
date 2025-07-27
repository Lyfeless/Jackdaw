using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public abstract class Sprite {
    public Color Color = Color.White;
    public Point2 Offset = Point2.Zero;
    public bool FlipX = false;
    public bool FlipY = false;

    public abstract Point2 Size { get; }
    public abstract RectInt Bounds { get; }
    public abstract void Render(Batcher batcher);

    protected Point2 FlipScale() => FlipScale(FlipX, FlipY);
    protected static Point2 FlipScale(bool x, bool y) {
        return new(x ? -1 : 1, y ? -1 : 1);
    }
}