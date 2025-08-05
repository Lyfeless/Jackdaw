using System.Net;
using System.Numerics;
using System.Runtime.Intrinsics;
using Foster.Framework;

namespace LittleLib;

public record struct MultiSegmentPathPosition(Vector2 Position, IPathSegment Segment, float SegmentPercent);

//! FIXME (Alex): Deadful name, but calling it just path is also bad
public readonly struct MultiSegmentPath {
    public MultiSegmentPath(Vector2 start, params IPathSegment[] segments) {
        Start = start;
        Segments = segments;

        End = Vector2.Zero;

        Length = 0;

        AbsolutePositions = new Vector2[segments.Length + 1];
        AbsolutePositions[0] = start;

        SegmentLengths = new float[segments.Length];

        for (int i = 0; i < segments.Length; ++i) {
            End += segments[i].EndOffset;
            SegmentLengths[i] = segments[i].Length;
            Length += SegmentLengths[i];
            AbsolutePositions[i + 1] = AbsolutePositions[i] + segments[i].EndOffset;
        }
    }

    public readonly Vector2 Start;
    public readonly Vector2 End;
    public readonly IPathSegment[] Segments;
    public readonly Vector2[] AbsolutePositions;
    public readonly float[] SegmentLengths;
    public readonly float Length;

    public MultiSegmentPathPosition Position(float percent) {
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

    public static Vector2[] AbsoluteToRelative(Vector2[] points) {
        if (points.Length > 1) {
            for (int i = points.Length - 1; i >= 1; --i) {
                points[i] = points[i] - points[i - 1];
            }
        }
        return points;
    }
}

public interface IPathSegment {
    public Vector2 EndOffset { get; }
    public float Length { get; }
    public Vector2 Position(float percent);
    public Vector2 Tangent(float percent);
}

public readonly struct PathSegmentLinear(Vector2 offset) : IPathSegment {
    public static MultiSegmentPath FromAbsolute(params Vector2[] points)
        => FromRelative(MultiSegmentPath.AbsoluteToRelative(points));
    public static MultiSegmentPath FromRelative(params Vector2[] points) {
        if (points.Length < 2) {
            Console.WriteLine("PATH: Not enough points provided to make a path of segments");
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

public readonly struct PathSegmentQuadraticBezier : IPathSegment {
    public static MultiSegmentPath FromAbsolute(params Vector2[] points)
        => FromRelative(MultiSegmentPath.AbsoluteToRelative(points));
    public static MultiSegmentPath FromRelative(params Vector2[] points) {
        if (points.Length < 3) {
            Console.WriteLine("PATH: Not enough points provided to make a path of quadratic beziers");
            return new(points.Length == 0 ? Vector2.Zero : points[0], []);
        }

        int length = points.Length;
        if (length % 2 != 1) {
            Console.WriteLine($"PATH: Point count must be odd to properly make a quadratic bezier path, ignoring the last point");
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

public readonly struct PathSegmentCubicBezier : IPathSegment {
    public static MultiSegmentPath FromAbsolute(params Vector2[] points)
        => FromRelative(MultiSegmentPath.AbsoluteToRelative(points));
    public static MultiSegmentPath FromRelative(params Vector2[] points) {
        if (points.Length < 4) {
            Console.WriteLine("PATH: Not enough points provided to make a path of cubic beziers");
            return new(points.Length == 0 ? Vector2.Zero : points[0], []);
        }

        int length = points.Length;
        if (length % 3 != 1) {
            int pointReduction = length - ((length - 1) / 3 * 3) + 1;
            Console.WriteLine($"PATH: Point count must be 3n + 1 to properly make a cubic bezier path, ignoring the last {pointReduction} points");
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