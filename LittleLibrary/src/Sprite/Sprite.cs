using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public abstract class Sprite {
    public abstract Vector2 Size { get; }
    public abstract Rect Bounds { get; }
    public abstract void Render(Batcher batcher);
}