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

    public override void Render(Batcher batcher) {
        batcher.SemiCircleLine(Circle.Position, StartRadians, EndRadians, Circle.Radius, Resolution, LineWeight, Color);
    }
}