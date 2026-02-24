using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Display configuration for rendering a circular object.
/// </summary>
public interface IDisplayObjectCircle {
    /// <summary>
    /// The circle to render.
    /// </summary>
    public Circle Circle { get; set; }

    /// <summary>
    /// The number of segments to render the circle with. <br/>
    /// More segments make a smoother circle but take longer to render.
    /// </summary>
    public int Resolution { get; set; }
}

/// <summary>
/// Display configuration for rendering a semicircle object.
/// </summary>
public interface IDisplayObjectSemiCircle : IDisplayObjectCircle {
    /// <summary>
    /// The angle the semicircle should start at, in radians.
    /// </summary>
    public float StartRadians { get; set; }

    /// <summary>
    /// The angle the semicircle should stop at, in radians.
    /// </summary>
    public float EndRadians { get; set; }
}

/// <summary>
/// A renderable filled circle.
/// </summary>
/// <param name="circle">The circle to render.</param>
public class DisplayCircle(Circle circle)
    : DisplayObject, IDisplayObjectCircle {
    public Circle Circle { get; set; } = circle;
    public int Resolution { get; set; } = 16;

    public override RectInt Bounds => Circle.Bounds.Int();

    /// <summary>
    /// A renderable filled circle.
    /// </summary>
    /// <param name="position">The position of the circle's center.</param>
    /// <param name="radius">The radius of the circle.</param>
    public DisplayCircle(Vector2 position, float radius)
        : this(new Circle(position, radius)) { }

    /// <summary>
    /// A renderable filled circle.
    /// </summary>
    /// <param name="position">The position of the circle's center.</param>
    /// <param name="radius">The radius of the circle.</param>
    public DisplayCircle(Point2 position, float radius)
        : this(new Circle(position, radius)) { }

    /// <summary>
    /// A renderable filled circle.
    /// </summary>
    /// <param name="x">The x position of the circle's center.</param>
    /// <param name="y">The y position of the circle's center.</param>
    /// <param name="radius">The radius of the circle.</param>
    public DisplayCircle(float x, float y, float radius)
        : this(new Circle(x, y, radius)) { }

    public override void Render(Batcher batcher) {
        batcher.Circle(Circle, Resolution, Color);
    }
}

/// <summary>
/// A renderable circle outline.
/// </summary>
/// <param name="circle">The circle to render.</param>
/// <param name="lineWeight">The width of the rendered lines.</param>
public class DisplayCircleLines(Circle circle, float lineWeight)
    : DisplayObject, IDisplayObjectCircle, IDisplayObjectOutline {
    public Circle Circle { get; set; } = circle;
    public int Resolution { get; set; } = 16;
    public float LineWeight { get; set; } = lineWeight;

    public override RectInt Bounds => Circle.Bounds.Int();

    /// <summary>
    /// A renderable circle outline.
    /// </summary>
    /// <param name="position">The position of the circle's center.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    public DisplayCircleLines(Vector2 position, float radius, float lineWeight)
        : this(new Circle(position, radius), lineWeight) { }

    /// <summary>
    /// A renderable circle outline.
    /// </summary>
    /// <param name="position">The position of the circle's center.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    public DisplayCircleLines(Point2 position, float radius, float lineWeight)
        : this(new Circle(position, radius), lineWeight) { }

    /// <summary>
    /// A renderable circle outline.
    /// </summary>
    /// <param name="x">The x position of the circle's center.</param>
    /// <param name="y">The y position of the circle's center.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    public DisplayCircleLines(float x, float y, float radius, float lineWeight)
        : this(new Circle(x, y, radius), lineWeight) { }

    public override void Render(Batcher batcher) {
        batcher.CircleLine(Circle, LineWeight, Resolution, Color);
    }
}

/// <summary>
/// A renderable dashed circle outline.
/// </summary>
/// <param name="circle">The circle to render.</param>
/// <param name="lineWeight">The width of the rendered lines.</param>
/// <param name="dashLength">The length of every segment and gap pair along the line.</param>
public class DisplayCircleLinesDashed(Circle circle, float lineWeight, float dashLength)
    : DisplayObject, IDisplayObjectCircle, IDisplayObjectDashedOutline {
    public Circle Circle { get; set; } = circle;
    public int Resolution { get; set; } = 16;
    public float LineWeight { get; set; } = lineWeight;
    public float DashLength { get; set; } = dashLength;
    public float OffsetPercent { get; set; } = 0;

    public override RectInt Bounds => Circle.Bounds.Int();

    /// <summary>
    /// A renderable dashed circle outline.
    /// </summary>
    /// <param name="position">The position of the circle's center.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    /// <param name="dashLength">The length of every segment and gap pair along the line.</param>
    public DisplayCircleLinesDashed(Vector2 position, float radius, float lineWeight, float dashLength)
        : this(new Circle(position, radius), lineWeight, dashLength) { }

    /// <summary>
    /// A renderable dashed circle outline.
    /// </summary>
    /// <param name="position">The position of the circle's center.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    /// <param name="dashLength">The length of every segment and gap pair along the line.</param>
    public DisplayCircleLinesDashed(Point2 position, float radius, float lineWeight, float dashLength)
        : this(new Circle(position, radius), lineWeight, dashLength) { }

    /// <summary>
    /// A renderable dashed circle outline.
    /// </summary>
    /// <param name="x">The x position of the circle's center.</param>
    /// <param name="y">The y position of the circle's center.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    /// <param name="dashLength">The length of every segment and gap pair along the line.</param>
    public DisplayCircleLinesDashed(float x, float y, float radius, float lineWeight, float dashLength)
        : this(new Circle(x, y, radius), lineWeight, dashLength) { }

    public override void Render(Batcher batcher) {
        batcher.CircleDashed(Circle, LineWeight, Resolution, Color, DashLength, OffsetPercent);
    }
}

/// <summary>
/// A renderable filled semicircle.
/// </summary>
/// <param name="circle">The circle to render a segment of.</param>
/// <param name="startRadians">The angle the semicircle should start at, in radians.</param>
/// <param name="endRadians">The angle the semicircle should stop at, in radians.</param>
public class DisplaySemiCircle(Circle circle, float startRadians, float endRadians)
    : DisplayObject, IDisplayObjectSemiCircle, IDisplayObjectCircle {
    public Circle Circle { get; set; } = circle;
    public int Resolution { get; set; } = 16;
    public float StartRadians { get; set; } = startRadians;
    public float EndRadians { get; set; } = endRadians;

    public override RectInt Bounds => Circle.Bounds.Int();

    /// <summary>
    /// A renderable filled semicircle.
    /// </summary>
    /// <param name="position">The position of the circle's center.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="startRadians">The angle the semicircle should start at, in radians.</param>
    /// <param name="endRadians">The angle the semicircle should stop at, in radians.</param>
    public DisplaySemiCircle(Vector2 position, float radius, float startRadians, float endRadians)
        : this(new Circle(position, radius), startRadians, endRadians) { }

    /// <summary>
    /// A renderable filled semicircle.
    /// </summary>
    /// <param name="position">The position of the circle's center.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="startRadians">The angle the semicircle should start at, in radians.</param>
    /// <param name="endRadians">The angle the semicircle should stop at, in radians.</param>
    public DisplaySemiCircle(Point2 position, float radius, float startRadians, float endRadians)
        : this(new Circle(position, radius), startRadians, endRadians) { }

    /// <summary>
    /// A renderable filled semicircle.
    /// </summary>
    /// <param name="x">The x position of the circle's center.</param>
    /// <param name="y">The y position of the circle's center.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="startRadians">The angle the semicircle should start at, in radians.</param>
    /// <param name="endRadians">The angle the semicircle should stop at, in radians.</param>
    public DisplaySemiCircle(float x, float y, float radius, float startRadians, float endRadians)
        : this(new Circle(x, y, radius), startRadians, endRadians) { }

    public override void Render(Batcher batcher) {
        batcher.SemiCircle(Circle.Position, StartRadians, EndRadians, Circle.Radius, Resolution, Color);
    }
}

/// <summary>
/// A renderable semicircle outline.
/// </summary>
/// <param name="circle">The circle to render a segment of.</param>
/// <param name="startRadians">The angle the semicircle should start at, in radians.</param>
/// <param name="endRadians">The angle the semicircle should stop at, in radians.</param>
/// <param name="lineWeight">The width of the rendered lines.</param>
public class DisplaySemiCircleLines(Circle circle, float startRadians, float endRadians, float lineWeight)
    : DisplayObject, IDisplayObjectSemiCircle, IDisplayObjectOutline {
    public Circle Circle { get; set; } = circle;
    public int Resolution { get; set; } = 16;
    public float StartRadians { get; set; } = startRadians;
    public float EndRadians { get; set; } = endRadians;
    public float LineWeight { get; set; } = lineWeight;

    public override RectInt Bounds => Circle.Bounds.Int();

    /// <summary>
    /// A renderable semicircle outline.
    /// </summary>
    /// <param name="position">The position of the circle's center.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="startRadians">The angle the semicircle should start at, in radians.</param>
    /// <param name="endRadians">The angle the semicircle should stop at, in radians.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    public DisplaySemiCircleLines(Vector2 position, float radius, float startRadians, float endRadians, float lineWeight)
        : this(new Circle(position, radius), startRadians, endRadians, lineWeight) { }

    /// <summary>
    /// A renderable semicircle outline.
    /// </summary>
    /// <param name="position">The position of the circle's center.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="startRadians">The angle the semicircle should start at, in radians.</param>
    /// <param name="endRadians">The angle the semicircle should stop at, in radians.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    public DisplaySemiCircleLines(Point2 position, float radius, float startRadians, float endRadians, float lineWeight)
        : this(new Circle(position, radius), startRadians, endRadians, lineWeight) { }

    /// <summary>
    /// A renderable semicircle outline.
    /// </summary>
    /// <param name="x">The x position of the circle's center.</param>
    /// <param name="y">The y position of the circle's center.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="startRadians">The angle the semicircle should start at, in radians.</param>
    /// <param name="endRadians">The angle the semicircle should stop at, in radians.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    public DisplaySemiCircleLines(float x, float y, float radius, float startRadians, float endRadians, float lineWeight)
        : this(new Circle(x, y, radius), startRadians, endRadians, lineWeight) { }

    public override void Render(Batcher batcher) {
        batcher.SemiCircleLine(Circle.Position, StartRadians, EndRadians, Circle.Radius, Resolution, LineWeight, Color);
    }
}