using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

public static class CalcExtra {
    /// <summary>
    /// Calculate the triple product of 3 2D vectors
    /// </summary>
    /// <param name="a">The first vector.</param>
    /// <param name="b">The second vector.</param>
    /// <param name="c">The third vector.</param>
    /// <returns></returns>
    public static Vector2 TripleProduct(Vector2 a, Vector2 b, Vector2 c) {
        Vector3 a3 = new(a.X, a.Y, 0);
        Vector3 b3 = new(b.X, b.Y, 0);
        Vector3 c3 = new(c.X, c.Y, 0);

        Vector3 first = Vector3.Cross(a3, b3);
        Vector3 second = Vector3.Cross(first, c3);

        return new(second.X, second.Y);
    }

    /// <summary>
    /// Get the middle position between two points.
    /// </summary>
    /// <param name="a">The first point of the line.</param>
    /// <param name="b">The second point of the line.</param>
    /// <returns>The midpoint of the line from a to b.</returns>
    public static Vector2 LineMidpoint(Vector2 a, Vector2 b) {
        return ((a - b) / 2) + b;
    }

    /// <summary>
    /// Get intersection between two line segments.
    /// </summary>
    /// <param name="a1">The first point of segment a.</param>
    /// <param name="a2">The second point of segment a.</param>
    /// <param name="b1">The first point of segment b.</param>
    /// <param name="b2">The second point of segment b.</param>
    /// <param name="intersection">The intersection point between the two lines.</param>
    /// <returns>True if a collision exists</returns>
    public static bool LineSegmentIntersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 intersection) {
        float t = (((a1.X - b1.X) * (b1.Y - b2.Y)) - ((a1.Y - b1.Y) * (b1.X - b2.X))) / (((a1.X - a2.X) * (b1.Y - b2.Y)) - ((a1.Y - a2.Y) * (b1.X - b2.X)));
        float u = -((((a1.X - a2.X) * (a1.Y - b1.Y)) - ((a1.Y - a2.Y) * (a1.X - b1.X))) / (((a1.X - a2.X) * (b1.Y - b2.Y)) - ((a1.Y - a2.Y) * (b1.X - b2.X))));
        intersection = new(a1.X + (t * (a2.X - a1.X)), a1.Y + (t * (a2.Y - a1.Y)));
        return 0 <= t && t <= 1 && 0 <= u && u <= 1;
    }

    /// <summary>
    /// Get intersection point between a line and a line segment.
    /// </summary>
    /// <param name="line1">The first point of the line.</param>
    /// <param name="line2">The second point of the line.</param>
    /// <param name="segment1">The first point of the line segment.</param>
    /// <param name="segment2">The second point of the line segment.</param>
    /// <param name="intersection">The intersection point between the two lines, Vector2.NaN if lines intersect infinitely many times.</param>
    /// <returns>True if a collision exists</returns>
    public static bool LineAndLineSegmentIntersection(Vector2 line1, Vector2 line2, Vector2 segment1, Vector2 segment2, out Vector2 intersection) {
        intersection = Vector2.Zero;

        float denominator = ((line1.X - line2.X) * (segment1.Y - segment2.Y)) - ((line1.Y - line2.Y) * (segment1.X - segment2.X));
        if (denominator == 0) {
            // Lines are ontop of one another
            if ((line1.X - segment1.X) * (segment1.Y - line2.Y) == (segment1.X - line2.X) * (line1.Y - segment1.Y)) {
                intersection = Vector2.NaN;
                return true;
            }

            // Lines never intersect
            return false;
        }

        float lineDiff = (line1.X * line2.Y) - (line1.Y * line2.X);
        float segmentDiff = (segment1.X * segment2.Y) - (segment1.Y * segment2.X);
        float numX = (lineDiff * (segment1.X - segment2.X)) - (segmentDiff * (line1.X - line2.X));
        float numY = (lineDiff * (segment1.Y - segment2.Y)) - (segmentDiff * (line1.Y - line2.Y));
        float x = numX / denominator;
        float y = numY / denominator;

        // If the intersection falls between either axis bounds of the segment it is contained
        //  Checking the highest distance axis to reduce the risk of floating point error
        if (
            MathF.Abs(segment1.X - segment2.X) > MathF.Abs(segment1.Y - segment2.Y)
                ? (x < MathF.Min(segment1.X, segment2.X) || x > MathF.Max(segment1.X, segment2.X))
                : (y < MathF.Min(segment1.Y, segment2.Y) || y > MathF.Max(segment1.Y, segment2.Y))
        ) {
            return false;
        }

        intersection = new(x, y);
        return true;
    }

    /// <summary>
    /// Convert a 2D vector's angle without changing its angle.
    /// </summary>
    /// <param name="direction">The direction to transform.</param>
    /// <param name="transform">The transform to apply to the direction.</param>
    /// <returns>The transformed direction.</returns>
    public static Vector2 TransformDirection(Vector2 direction, Transform transform)
        => TransformDirection(direction, transform.Matrix);

    /// <summary>
    /// Convert a 2D vector's angle without changing its angle.
    /// </summary>
    /// <param name="direction">The direction to transform.</param>
    /// <param name="transform">The matrix to transform the direction by.</param>
    /// <returns>The transformed direction.</returns>
    public static Vector2 TransformDirection(Vector2 direction, Matrix3x2 transform) {
        return Vector2.Transform(direction, transform) - transform.Translation;
    }

    /// <summary>
    /// Get axis aligned bounds for a transformed rectangle.
    /// </summary>
    /// <param name="rect">The rectangle to transform.</param>
    /// <param name="transform">The transform to apply to the rectangle.</param>
    /// <returns></returns>
    public static Rect TransformRect(Rect rect, Transform transform)
        => TransformRect(rect, transform.Matrix);

    /// <summary>
    /// Get axis aligned bounds for a transformed rectangle.
    /// </summary>
    /// <param name="rect">The rectangle to transform.</param>
    /// <param name="transform">The matrix to transform the rectangle by.</param>
    /// <returns></returns>
    public static Rect TransformRect(Rect rect, Matrix3x2 transform) {
        return new BoundsBuilder(
            Vector2.Transform(rect.TopLeft, transform),
            Vector2.Transform(rect.TopRight, transform),
            Vector2.Transform(rect.BottomLeft, transform),
            Vector2.Transform(rect.BottomRight, transform)
        ).Rect;
    }

    /// <summary>
    /// Get the sign of a number that is always either 1 or -1.
    /// Functions the same as Math.Sign in all cases except 0.
    /// </summary>
    /// <param name="value">The value to get the sign of.</param>
    /// <returns>The sign of the value, treated as 1 for a value of 0.</returns>
    public static int NormalizedSign(float value) {
        return value >= 0 ? 1 : -1;
    }
}