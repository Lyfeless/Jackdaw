using Foster.Framework;

namespace Jackdaw;

public interface IDisplayObjectRect {
    public RectInt Rect { get; set; }
}

public interface IDisplayObjectRoundableRect : IDisplayObjectRect {
    public RectangleCornerRounding Corners { get; set; }
}

public readonly struct RectangleCornerRounding(float topLeft, float topRight, float bottomLeft, float bottomRight) {
    public readonly float TopLeft = topLeft;
    public readonly float TopRight = topRight;
    public readonly float BottomLeft = bottomLeft;
    public readonly float BottomRight = bottomRight;

    public readonly bool Unrounded =
        topLeft == 0 &&
        topRight == 0 &&
        bottomLeft == 0 &&
        bottomRight == 0;

    public readonly bool AllCornersSame =
        topLeft == topRight &&
        topLeft == bottomLeft &&
        topLeft == bottomRight;

    public static RectangleCornerRounding None() => new(0, 0, 0, 0);
    public static RectangleCornerRounding All(float value) => new(value, value, value, value);
    public static RectangleCornerRounding LeftRight(float left, float right) => new(left, right, left, right);
    public static RectangleCornerRounding TopBottom(float top, float bottom) => new(top, top, bottom, bottom);
}

public class DisplayRectangle(RectInt rect)
    : DisplayObject, IDisplayObjectRoundableRect {
    public RectInt Rect { get; set; } = rect;
    public RectangleCornerRounding Corners { get; set; } = RectangleCornerRounding.None();

    public override RectInt Bounds => Rect;

    public override void Render(Batcher batcher) {
        if (Corners.Unrounded) { batcher.Rect(Rect, Color); }
        else { batcher.RectRounded(Rect, Corners.TopLeft, Corners.TopRight, Corners.BottomRight, Corners.BottomLeft, Color); }
    }
}

public class DisplayRectangleLines(RectInt rect, float lineWeight)
    : DisplayObject, IDisplayObjectRoundableRect, IDisplayObjectLines {
    public RectInt Rect { get; set; } = rect;
    public float LineWeight { get; set; } = lineWeight;
    public RectangleCornerRounding Corners { get; set; } = RectangleCornerRounding.None();

    public override RectInt Bounds => Rect;

    public override void Render(Batcher batcher) {
        if (Corners.Unrounded) { batcher.RectLine(Rect, LineWeight, Color); }
        else { batcher.RectRoundedLine(Rect, Corners.TopLeft, Corners.TopRight, Corners.BottomRight, Corners.BottomLeft, LineWeight, Color); }
    }
}

public class DisplayRectangleLinesDashed(RectInt rect, float lineWeight, float dashLength)
    : DisplayObject, IDisplayObjectRect, IDisplayObjectDashedLines {
    public RectInt Rect { get; set; } = rect;
    public float LineWeight { get; set; } = lineWeight;
    public float DashLength { get; set; } = dashLength;
    public float OffsetPercent { get; set; } = 0;
    // Dashed Rectangle doesn't support rounded corners

    public override RectInt Bounds => Rect;

    public override void Render(Batcher batcher) {
        batcher.RectDashed(Rect, LineWeight, Color, DashLength, OffsetPercent);
    }
}