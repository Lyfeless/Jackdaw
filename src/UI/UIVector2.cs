using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public struct UIVector2(float x, float y, UIVector2.PositionType typeX, UIVector2.PositionType typeY) {
    public static UIVector2 Pixel(float a) => Pixel(a, a);
    public static UIVector2 Pixel(Vector2 a) => Pixel(a.X, a.Y);
    public static UIVector2 Pixel(Point2 a) => Pixel(a.X, a.Y);
    public static UIVector2 Pixel(float x, float y) => new(x, y, PositionType.PIXEL, PositionType.PIXEL);

    public static UIVector2 PixelRelative(float a) => PixelRelative(a, a);
    public static UIVector2 PixelRelative(Vector2 a) => PixelRelative(a.X, a.Y);
    public static UIVector2 PixelRelative(Point2 a) => PixelRelative(a.X, a.Y);
    public static UIVector2 PixelRelative(float x, float y) => new(x, y, PositionType.PIXEL_RELATIVE, PositionType.PIXEL_RELATIVE);

    public static UIVector2 PixelRelativeX(float a) => PixelRelativeX(a, a);
    public static UIVector2 PixelRelativeX(Vector2 a) => PixelRelativeX(a.X, a.Y);
    public static UIVector2 PixelRelativeX(Point2 a) => PixelRelativeX(a.X, a.Y);
    public static UIVector2 PixelRelativeX(float x, float y) => new(x, y, PositionType.PIXEL_RELATIVE, PositionType.PIXEL);

    public static UIVector2 PixelRelativeY(float a) => PixelRelativeY(a, a);
    public static UIVector2 PixelRelativeY(Vector2 a) => PixelRelativeY(a.X, a.Y);
    public static UIVector2 PixelRelativeY(Point2 a) => PixelRelativeY(a.X, a.Y);
    public static UIVector2 PixelRelativeY(float x, float y) => new(x, y, PositionType.PIXEL, PositionType.PIXEL_RELATIVE);

    public static UIVector2 Scaling(float a) => Scaling(a, a);
    public static UIVector2 Scaling(Vector2 a) => Scaling(a.X, a.Y);
    public static UIVector2 Scaling(Point2 a) => Scaling(a.X, a.Y);
    public static UIVector2 Scaling(float x, float y) => new(x, y, PositionType.SCALING, PositionType.SCALING);

    public static readonly UIVector2 TopLeft = Scaling(0);
    public static readonly UIVector2 TopRight = Scaling(1, 0);
    public static readonly UIVector2 BottomLeft = Scaling(0, 1);
    public static readonly UIVector2 BottomRight = Scaling(1);
    public static readonly UIVector2 Top = Scaling(0.5f, 0);
    public static readonly UIVector2 Bottom = Scaling(0.5f, 1);
    public static readonly UIVector2 Left = Scaling(0, 0.5f);
    public static readonly UIVector2 Right = Scaling(1, 0.5f);
    public static readonly UIVector2 Center = Scaling(0.5f);

    public enum PositionType {
        PIXEL,
        PIXEL_RELATIVE,
        SCALING
    }

    public Vector2 value = new(x, y);
    public PositionType TypeX = typeX;
    public PositionType TypeY = typeY;

    public Vector2 GetAppliedValue(Vector2 size) => new(ApplyType(value.X, TypeX, size.X), ApplyType(value.Y, TypeY, size.Y));

    static float ApplyType(float value, PositionType type, float size) =>
        type switch {
            PositionType.PIXEL_RELATIVE => size + value,
            PositionType.SCALING => size * value,
            _ => value
        };

}