using Foster.Framework;

namespace Jackdaw;

class ScalingAxis(int value) {
    public int Value = value;
    public int Size => Math.Abs(Value);
    public int Position => Math.Min(Value, 0);
}

class ScalingPoint2(int width, int height) {
    readonly ScalingAxis width = new(width);
    readonly ScalingAxis height = new(height);

    public int Width { get => width.Value; set => width.Value = value; }
    public int Height { get => height.Value; set => height.Value = value; }

    public Point2 Size => new(width.Size, height.Size);
    public int SizeX => width.Size;
    public int SizeY => width.Size;

    public Point2 Position => new(width.Position, height.Position);
    public int PositionX => width.Position;
    public int PositionY => height.Position;

    public ScalingPoint2(Point2 point2) : this(point2.X, point2.Y) { }
}