using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Display configuration for rendering a rectangle.
/// </summary>
public interface IDisplayObjectRect {
    /// <summary>
    /// The rect to render.
    /// </summary>
    public RectInt Rect { get; set; }
}

/// <summary>
/// Display configuration for rendering a rectangle with rounded corners.
/// </summary>
public interface IDisplayObjectRoundableRect : IDisplayObjectRect {
    /// <summary>
    /// The rectangle's corner rounding data.
    /// </summary>
    public RectangleCornerRounding Corners { get; set; }
}

/// <summary>
/// A configuration for how a rendered rectangle's corners should be rounded.
/// </summary>
/// <param name="topLeft">The top left corner's rounding radius.</param>
/// <param name="topRight">The top right corner's rounding radius.</param>
/// <param name="bottomLeft">The bottom left corner's rounding radius.</param>
/// <param name="bottomRight">The bottom right corner's rounding radius.</param>
public readonly struct RectangleCornerRounding(float topLeft, float topRight, float bottomLeft, float bottomRight) {
    /// <summary>
    /// The top left corner's rounding radius.
    /// </summary>
    public readonly float TopLeft = topLeft;

    /// <summary>
    /// The top right corner's rounding radius.
    /// </summary>
    public readonly float TopRight = topRight;

    /// <summary>
    /// he bottom left corner's rounding radius.
    /// </summary>
    public readonly float BottomLeft = bottomLeft;

    /// <summary>
    /// The bottom right corner's rounding radius.
    /// </summary>
    public readonly float BottomRight = bottomRight;

    /// <summary>
    /// If none of the corners are rounded.
    /// </summary>
    public readonly bool Unrounded =
        topLeft == 0 &&
        topRight == 0 &&
        bottomLeft == 0 &&
        bottomRight == 0;

    /// <summary>
    /// If all of the corners have the same rounding radius.
    /// </summary>
    public readonly bool AllCornersSame =
        topLeft == topRight &&
        topLeft == bottomLeft &&
        topLeft == bottomRight;

    /// <summary>
    /// Set all corners to unrounded.
    /// </summary>
    /// <returns>An unrounded rectangle corner configuration.</returns>
    public static RectangleCornerRounding None() => new(0, 0, 0, 0);

    /// <summary>
    /// Set all corners to the same value.
    /// </summary>
    /// <param name="value">The rounding radius of all corners.</param>
    /// <returns>A rectangle corner configuration.</returns>
    public static RectangleCornerRounding All(float value) => new(value, value, value, value);

    /// <summary>
    /// Set both left corners and both right corners to the same values.
    /// </summary>
    /// <param name="left">The rounding radius of both left corners.</param>
    /// <param name="right">he rounding radius of both right corners.</param>
    /// <returns>A rectangle corner configuration.</returns>
    public static RectangleCornerRounding LeftRight(float left, float right) => new(left, right, left, right);

    /// <summary>
    /// Set both top corners and both bottom corners to the same values.
    /// </summary>
    /// <param name="top">The rounding radius of both top corners.</param>
    /// <param name="bottom">he rounding radius of both bottom corners.</param>
    /// <returns>A rectangle corner configuration.</returns>
    public static RectangleCornerRounding TopBottom(float top, float bottom) => new(top, top, bottom, bottom);
}

/// <summary>
/// A renderable filled rectangle.
/// </summary>
/// <param name="rect">The rect to render.</param>
public class DisplayRectangle(RectInt rect)
    : DisplayObject, IDisplayObjectRoundableRect {
    public RectInt Rect { get; set; } = rect;
    public RectangleCornerRounding Corners { get; set; } = RectangleCornerRounding.None();

    public override RectInt Bounds => Rect;

    /// <summary>
    /// A renderable filled rectangle.
    /// </summary>
    /// <param name="rect">The rect to render.</param>
    public DisplayRectangle(Rect rect)
        : this(rect.Int()) { }

    /// <summary>
    /// A renderable filled rectangle.
    /// </summary>
    /// <param name="position">The position of the top left corner of the rectangle.</param>
    /// <param name="size">The size of the rectangle.</param>
    public DisplayRectangle(Vector2 position, Vector2 size)
        : this(new Rect(position, size)) { }

    /// <summary>
    /// A renderable filled rectangle.
    /// </summary>
    /// <param name="position">The position of the top left corner of the rectangle.</param>
    /// <param name="size">The size of the rectangle.</param>
    public DisplayRectangle(Point2 position, Point2 size)
        : this(new RectInt(position, size)) { }

    /// <summary>
    /// A renderable filled rectangle.
    /// </summary>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    public DisplayRectangle(float width, float height)
        : this(new Rect(width, height)) { }

    /// <summary>
    /// A renderable filled rectangle.
    /// </summary>
    /// <param name="x">The x position of the left side of the rectangle.</param>
    /// <param name="y">The y position of the top of the rectangle.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    public DisplayRectangle(float x, float y, float width, float height)
        : this(new Rect(x, y, width, height)) { }

    public override void Render(Batcher batcher) {
        if (Corners.Unrounded) { batcher.Rect(Rect, Color); }
        else { batcher.RectRounded(Rect, Corners.TopLeft, Corners.TopRight, Corners.BottomRight, Corners.BottomLeft, Color); }
    }
}

/// <summary>
/// A renderable rectangle outline.
/// </summary>
/// <param name="rect">The rect to render.</param>
/// <param name="lineWeight">The width of the rendered lines.</param>
public class DisplayRectangleLines(RectInt rect, float lineWeight)
    : DisplayObject, IDisplayObjectRoundableRect, IDisplayObjectOutline {
    public RectInt Rect { get; set; } = rect;
    public float LineWeight { get; set; } = lineWeight;
    public RectangleCornerRounding Corners { get; set; } = RectangleCornerRounding.None();

    public override RectInt Bounds => Rect;

    /// <summary>
    /// A renderable rectangle outline.
    /// </summary>
    /// <param name="rect">The rect to render.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    public DisplayRectangleLines(Rect rect, float lineWeight)
        : this(rect.Int(), lineWeight) { }

    /// <summary>
    /// A renderable rectangle outline.
    /// </summary>
    /// <param name="position">The position of the top left corner of the rectangle.</param>
    /// <param name="size">The size of the rectangle.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    public DisplayRectangleLines(Vector2 position, Vector2 size, float lineWeight)
        : this(new Rect(position, size), lineWeight) { }

    /// <summary>
    /// A renderable rectangle outline.
    /// </summary>
    /// <param name="position">The position of the top left corner of the rectangle.</param>
    /// <param name="size">The size of the rectangle.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    public DisplayRectangleLines(Point2 position, Point2 size, float lineWeight)
        : this(new RectInt(position, size), lineWeight) { }

    /// <summary>
    /// A renderable rectangle outline.
    /// </summary>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    public DisplayRectangleLines(float width, float height, float lineWeight)
        : this(new Rect(width, height), lineWeight) { }

    /// <summary>
    /// A renderable rectangle outline.
    /// </summary>
    /// <param name="x">The x position of the left side of the rectangle.</param>
    /// <param name="y">The y position of the top of the rectangle.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    public DisplayRectangleLines(float x, float y, float width, float height, float lineWeight)
        : this(new Rect(x, y, width, height), lineWeight) { }

    public override void Render(Batcher batcher) {
        if (Corners.Unrounded) { batcher.RectLine(Rect, LineWeight, Color); }
        else { batcher.RectRoundedLine(Rect, Corners.TopLeft, Corners.TopRight, Corners.BottomRight, Corners.BottomLeft, LineWeight, Color); }
    }
}

/// <summary>
/// A renderable dashed rectangle outline.
/// </summary>
/// <param name="rect">The rect to render.</param>
/// <param name="lineWeight">The width of the rendered lines.</param>
/// <param name="dashLength">The length of every segment and gap pair along the line.</param>
public class DisplayRectangleLinesDashed(RectInt rect, float lineWeight, float dashLength)
    : DisplayObject, IDisplayObjectRect, IDisplayObjectDashedOutline {
    public RectInt Rect { get; set; } = rect;
    public float LineWeight { get; set; } = lineWeight;
    public float DashLength { get; set; } = dashLength;
    public float OffsetPercent { get; set; } = 0;
    // Dashed Rectangle doesn't support rounded corners

    public override RectInt Bounds => Rect;

    /// <summary>
    /// A renderable dashed rectangle outline.
    /// </summary>
    /// <param name="rect">The rect to render.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    /// <param name="dashLength">The length of every segment and gap pair along the line.</param>
    public DisplayRectangleLinesDashed(Rect rect, float lineWeight, float dashLength)
        : this(rect.Int(), lineWeight, dashLength) { }

    /// <summary>
    /// A renderable dashed rectangle outline.
    /// </summary>
    /// <param name="position">The position of the top left corner of the rectangle.</param>
    /// <param name="size">The size of the rectangle.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    /// <param name="dashLength">The length of every segment and gap pair along the line.</param>
    public DisplayRectangleLinesDashed(Vector2 position, Vector2 size, float lineWeight, float dashLength)
        : this(new Rect(position, size), lineWeight, dashLength) { }

    /// <summary>
    /// A renderable dashed rectangle outline.
    /// </summary>
    /// <param name="position">The position of the top left corner of the rectangle.</param>
    /// <param name="size">The size of the rectangle.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    /// <param name="dashLength">The length of every segment and gap pair along the line.</param>
    public DisplayRectangleLinesDashed(Point2 position, Point2 size, float lineWeight, float dashLength)
        : this(new RectInt(position, size), lineWeight, dashLength) { }

    /// <summary>
    /// A renderable dashed rectangle outline.
    /// </summary>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    /// <param name="dashLength">The length of every segment and gap pair along the line.</param>
    public DisplayRectangleLinesDashed(float width, float height, float lineWeight, float dashLength)
        : this(new Rect(width, height), lineWeight, dashLength) { }

    /// <summary>
    /// A renderable dashed rectangle outline.
    /// </summary>
    /// <param name="x">The x position of the left side of the rectangle.</param>
    /// <param name="y">The y position of the top of the rectangle.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    /// <param name="dashLength">The length of every segment and gap pair along the line.</param>
    public DisplayRectangleLinesDashed(float x, float y, float width, float height, float lineWeight, float dashLength)
        : this(new Rect(x, y, width, height), lineWeight, dashLength) { }

    public override void Render(Batcher batcher) {
        batcher.RectDashed(Rect, LineWeight, Color, DashLength, OffsetPercent);
    }
}