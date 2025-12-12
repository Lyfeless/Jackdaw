using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Information about a single position along a path.
/// </summary>
/// <param name="Position">The position offset from the path origin.</param>
/// <param name="Segment">The segment of the path the point falls on.</param>
/// <param name="SegmentPercent">The point's percent distance along the segment.</param>
public record struct MultiSegmentPathPosition(Vector2 Position, IPathSegment Segment, float SegmentPercent);

/// <summary>
/// A utility for creating and navigating static paths made of multiple segments.
/// </summary>
public readonly struct MultiSegmentPath {
    /// <summary>
    /// Create a new path from premade segments.
    /// </summary>
    /// <param name="start">The start position of the path.</param>
    /// <param name="segments">The segments, all are positioned relative to the end of the previous path.</param>
    public MultiSegmentPath(Vector2 start, params IPathSegment[] segments) {
        Start = start;
        Segments = segments;

        EndRelative = Vector2.Zero;

        Length = 0;

        AbsolutePositions = new Vector2[segments.Length + 1];
        AbsolutePositions[0] = start;

        SegmentLengths = new float[segments.Length];

        for (int i = 0; i < segments.Length; ++i) {
            EndRelative += segments[i].EndOffset;
            SegmentLengths[i] = segments[i].Length;
            Length += SegmentLengths[i];
            AbsolutePositions[i + 1] = AbsolutePositions[i] + segments[i].EndOffset;
        }

        End = start + EndRelative;
    }

    /// <summary>
    /// The start position of the path.
    /// </summary>
    public readonly Vector2 Start;

    /// <summary>
    /// The end position of the path.
    /// </summary>
    public readonly Vector2 End;

    /// <summary>
    /// The end position of the path, relative to the start.
    /// </summary>
    public readonly Vector2 EndRelative;

    /// <summary>
    /// The segments of the full path.
    /// The start position of each segment is relative to the end of the last.
    /// </summary>
    public readonly IPathSegment[] Segments;

    /// <summary>
    /// The positions of each segment start, relative to the start of the path.
    /// </summary>
    public readonly Vector2[] AbsolutePositions;

    /// <summary>
    /// The length of each segment of the path.
    /// </summary>
    public readonly float[] SegmentLengths;

    /// <summary>
    /// The total length of the entire path.
    /// </summary>
    public readonly float Length;

    /// <summary>
    /// Get the positional information of the path at distance along the distance.
    /// </summary>
    /// <param name="distance">The percent along the path from the start, range 0-1.</param>
    /// <returns>The positional information at the given point in the path</returns>
    public MultiSegmentPathPosition DistancePosition(float distance) => PercentPosition(Calc.Clamp(distance, 0, Length) / Length);

    /// <summary>
    /// Get the positional information of the path at a percent of the distance.
    /// </summary>
    /// <param name="percent">The percent along the path from the start, range 0-1.</param>
    /// <returns>The positional information at the given point in the path</returns>
    public MultiSegmentPathPosition PercentPosition(float percent) {
        percent = Calc.Clamp(percent, 0, 1);

        float length = 0;
        for (int i = 0; i < SegmentLengths.Length; ++i) {
            if ((length + SegmentLengths[i]) / Length >= percent) {
                float percentAdjusted = (percent - (length / Length)) / (SegmentLengths[i] / Length);
                return new(AbsolutePositions[i] + Segments[i].Position(percentAdjusted), Segments[i], percentAdjusted);
            }
            length += SegmentLengths[i];
        }

        return new(End, Segments[^1], 1);
    }

    /// <summary>
    /// Convert an array of point to offsets from the previous point in the array.
    /// </summary>
    /// <param name="points">The points to convert.</param>
    /// <returns>The points converted to relative.</returns>
    public static Vector2[] AbsoluteToRelative(Vector2[] points) {
        if (points.Length > 1) {
            for (int i = points.Length - 1; i >= 1; --i) {
                points[i] = points[i] - points[i - 1];
            }
        }
        return points;
    }
}

/// <summary>
/// A segment on a multi-segment path.
/// </summary>
public interface IPathSegment {
    /// <summary>
    /// The Relative offset from the start of the path to the end.
    /// </summary>
    public Vector2 EndOffset { get; }

    /// <summary>
    /// The overall length of the path segment.
    /// </summary>
    public float Length { get; }

    /// <summary>
    /// Get the position of the path at the given percent.
    /// </summary>
    /// <param name="percent">The percent distance along the segment.</param>
    /// <returns>The position at the given percent.</returns>
    public Vector2 Position(float percent);

    /// <summary>
    /// Get the tangent of the path at the given percent, facing towards the end of the path.
    /// </summary>
    /// <param name="percent">The percent distance along the segment.</param>
    /// <returns>The tangent at the given percent.</returns>
    public Vector2 Tangent(float percent);
}

/// <summary>
/// A simple linear path segment, goes directly from the start to the end.
/// </summary>
/// <param name="offset">The end point of the segment, relative to the start.</param>
public readonly struct PathSegmentLinear(Vector2 offset) : IPathSegment {
    /// <summary>
    /// Create a full path made out of linear segments, with all points being relative to the start point.
    /// </summary>
    /// <param name="points">The points of the path, all relative to the origin.</param>
    /// <returns>A multi-segment path.</returns>
    public static MultiSegmentPath FromAbsolute(params Vector2[] points)
        => FromRelative(MultiSegmentPath.AbsoluteToRelative(points));

    /// <summary>
    /// Create a full path made out of linear segments, with all points being relative to the previous point.
    /// </summary>
    /// <param name="points">The points of the path, all relative to the previous point.</param>
    /// <returns>A multi-segment path.</returns>
    public static MultiSegmentPath FromRelative(params Vector2[] points) {
        if (points.Length < 2) {
            Log.Warning("Path: Not enough points provided to make a path of segments");
            return new(points.Length == 0 ? Vector2.Zero : points[0], []);
        }

        IPathSegment[] segments = new IPathSegment[points.Length - 1];
        for (int i = 0; i < segments.Length; ++i) {
            segments[i] = new PathSegmentLinear(points[i + 1]);
        }

        return new MultiSegmentPath(points[0], segments);
    }

    public readonly Vector2 EndOffset { get; } = offset;
    public readonly float Length { get; } = offset.Length();

    readonly Vector2 tangent = offset.Normalized();

    public readonly Vector2 Position(float percent) {
        return EndOffset * percent;
    }

    public readonly Vector2 Tangent(float percent) => tangent;
}

// Derivative math adapted from https://pomax.github.io/bezierinfo

/// <summary>
/// A quadradic bezier segment, goes from the start to the end with 1 control point.
/// </summary>
public readonly struct PathSegmentQuadraticBezier : IPathSegment {
    /// <summary>
    /// Create a full path made out of quadratic bezier segments, with all points being relative to the start point.
    /// </summary>
    /// <param name="points">The points of the path, all relative to the origin.</param>
    /// <returns>A multi-segment path.</returns>
    public static MultiSegmentPath FromAbsolute(params Vector2[] points)
        => FromRelative(MultiSegmentPath.AbsoluteToRelative(points));

    /// <summary>
    /// Create a full path made out of quadratic bezier segments, with all points being relative to the previous point.
    /// </summary>
    /// <param name="points">The points of the path, all relative to the previous point.</param>
    /// <returns>A multi-segment path.</returns>
    public static MultiSegmentPath FromRelative(params Vector2[] points) {
        if (points.Length < 3) {
            Log.Warning("Path: Not enough points provided to make a path of quadratic beziers");
            return new(points.Length == 0 ? Vector2.Zero : points[0], []);
        }

        int length = points.Length;
        if (length % 2 != 1) {
            Log.Warning($"Path: Point count must be odd to properly make a quadratic bezier path, ignoring the last point");
            length--;
        }

        IPathSegment[] segments = new IPathSegment[(length - 1) / 2];
        for (int i = 1; i < length; i += 2) {
            segments[(i - 1) / 2] = new PathSegmentQuadraticBezier(points[i], points[i] + points[i + 1]);
        }

        return new(points[0], segments);
    }

    const int LENGTH_APPROX_STEPS = 10;

    readonly Vector2 Control;
    public readonly Vector2 EndOffset { get; }
    public float Length { get; }

    public PathSegmentQuadraticBezier(Vector2 control, Vector2 end) {
        Control = control;
        EndOffset = end;

        float step = 1f / LENGTH_APPROX_STEPS;
        Vector2 lastPoint = Vector2.Zero;
        Length = 0;
        for (int i = 1; i <= LENGTH_APPROX_STEPS; ++i) {
            Vector2 newPoint = Position(i * step);
            Length += (newPoint - lastPoint).Length();
            lastPoint = newPoint;
        }
    }

    public readonly Vector2 Position(float percent) {
        Vector2 a = Control * percent;
        Vector2 b = ((EndOffset - Control) * percent) + Control;
        return ((b - a) * percent) + a;
    }

    public Vector2 Tangent(float percent) {
        float percentInverse = 1 - percent;

        Vector2 d0 = 2 * Control;
        Vector2 d1 = 2 * (EndOffset - Control);

        return (percentInverse * d0 + percent * d1).Normalized();
    }
}

/// <summary>
/// A cubic bezier segment, goes from the start to the end with 2 control points.
/// </summary>
public readonly struct PathSegmentCubicBezier : IPathSegment {
    /// <summary>
    /// Create a full path made out of cubic bezier segments, with all points being relative to the start point.
    /// </summary>
    /// <param name="points">The points of the path, all relative to the origin.</param>
    /// <returns>A multi-segment path.</returns>
    public static MultiSegmentPath FromAbsolute(params Vector2[] points)
        => FromRelative(MultiSegmentPath.AbsoluteToRelative(points));

    /// <summary>
    /// Create a full path made out of cubic bezier segments, with all points being relative to the previous point.
    /// </summary>
    /// <param name="points">The points of the path, all relative to the previous point.</param>
    /// <returns>A multi-segment path.</returns>
    public static MultiSegmentPath FromRelative(params Vector2[] points) {
        if (points.Length < 4) {
            Log.Warning("Path: Not enough points provided to make a path of cubic beziers");
            return new(points.Length == 0 ? Vector2.Zero : points[0], []);
        }

        int length = points.Length;
        if (length % 3 != 1) {
            int pointReduction = length - ((length - 1) / 3 * 3) + 1;
            Log.Warning($"Path: Point count must be 3n + 1 to properly make a cubic bezier path, ignoring the last {pointReduction} points");
            length -= pointReduction;
        }

        IPathSegment[] segments = new IPathSegment[(length - 1) / 3];
        for (int i = 1; i < length; i += 3) {
            Vector2 a = points[i];
            Vector2 b = a + points[i + 1];
            Vector2 c = b + points[i + 2];
            segments[(i - 1) / 2] = new PathSegmentCubicBezier(a, b, c);
        }

        return new(points[0], segments);
    }

    const int LENGTH_APPROX_STEPS = 10;

    readonly Vector2 ControlA;
    readonly Vector2 ControlB;
    public readonly Vector2 EndOffset { get; }
    public float Length { get; }

    public PathSegmentCubicBezier(Vector2 controlA, Vector2 controlB, Vector2 end) {
        ControlA = controlA;
        ControlB = controlB;
        EndOffset = end;

        float step = 1f / LENGTH_APPROX_STEPS;
        Vector2 lastPoint = Vector2.Zero;
        Length = 0;
        for (int i = 1; i <= LENGTH_APPROX_STEPS; ++i) {
            Vector2 newPoint = Position(i * step);
            Length += (newPoint - lastPoint).Length();
            lastPoint = newPoint;
        }
    }

    public readonly Vector2 Position(float percent) {
        Vector2 a = ControlA * percent;
        Vector2 b = ((ControlB - ControlA) * percent) + ControlA;
        Vector2 c = ((EndOffset - ControlB) * percent) + ControlB;

        a = ((b - a) * percent) + a;
        b = ((c - b) * percent) + b;

        return ((b - a) * percent) + a;
    }

    public Vector2 Tangent(float percent) {
        float percentInverse = 1 - percent;
        float a = percentInverse * percentInverse;
        float b = 2 * percentInverse * percent;
        float c = percent * percent;

        Vector2 d0 = 3 * ControlA;
        Vector2 d1 = 3 * (ControlB - ControlA);
        Vector2 d2 = 3 * (EndOffset - ControlB);

        return (a * d0 + b * d1 + c * d2).Normalized();
    }
}