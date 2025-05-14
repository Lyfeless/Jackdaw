using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class Viewspace(Vector2 position, Point2 size) {
    RenderablePosition Position = new(position);
    public Point2 Size { get; } = size;

    public Vector2 TopLeft {
        get => Position.Precise;
        set => Position.Set(value);
    }

    public Vector2 TopRight {
        get => Position.Precise + Size.OnlyX();
        set => Position.Set(value - Size.OnlyX());
    }

    public Vector2 BottomLeft {
        get => Position.Precise + Size.OnlyY();
        set => Position.Set(value - Size.OnlyY());
    }

    public Vector2 BottomRight {
        get => Position.Precise + Size;
        set => Position.Set(value - Size);
    }

    public Vector2 Top {
        get => Position.Precise + (Size.OnlyX() / 2);
        set => Position.Set(value - (Size.OnlyX() / 2));
    }

    public Vector2 Bottom {
        get => Position.Precise + (Size.OnlyX() / 2) + Size.OnlyY();
        set => Position.Set(value - (Size.OnlyX() / 2) - Size.OnlyY());
    }

    public Vector2 Left {
        get => Position.Precise + (Size.OnlyY() / 2);
        set => Position.Set(value - (Size.OnlyY() / 2));
    }

    public Vector2 Right {
        get => Position.Precise + (Size.OnlyY() / 2) + Size.OnlyX();
        set => Position.Set(value - (Size.OnlyY() / 2) - Size.OnlyX());
    }

    public Vector2 Center {
        get => Position.Precise + (Size / 2);
        set => Position.Set(value - (Size / 2));
    }

    public Rect Bounds => new(Position.Precise, Size);

    public Point2 RenderPosition => Position.Rounded;

    public Viewspace(Point2 size) : this(Vector2.Zero, size) { }
}