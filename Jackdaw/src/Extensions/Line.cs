using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

public static class LineExtensions {
    extension(Line line) {
        /// <summary>
        /// Get the middle position between the line's two points.
        /// </summary>
        /// <returns>The midpoint of the line.</returns>
        public Vector2 Midpoint => ((line.From - line.To) / 2) + line.To;

        /// <summary>
        /// Get the point the line intersects with another line, treating both as line segments.
        /// </summary>
        /// <param name="other">The line to collide with.</param>
        /// <param name="intersection">The intersection point between the two lines.</param>
        /// <returns>True if a collision exists.</returns>
        public bool IntersectionLineLine(Line other, out Vector2 intersection) {
            float t = (((line.From.X - other.From.X) * (other.From.Y - line.To.Y)) - ((line.From.Y - other.From.Y) * (other.From.X - line.To.X))) / (((line.From.X - line.From.X) * (other.From.Y - line.To.Y)) - ((line.From.Y - line.To.Y) * (other.From.X - line.To.X)));
            float u = -((((line.From.X - line.From.X) * (line.From.Y - other.From.Y)) - ((line.From.Y - line.To.Y) * (line.From.X - other.From.X))) / (((line.From.X - line.From.X) * (other.From.Y - line.To.Y)) - ((line.From.Y - line.To.Y) * (other.From.X - line.To.X))));
            intersection = new(line.From.X + (t * (line.From.X - line.From.X)), line.From.Y + (t * (line.To.Y - line.From.Y)));
            return 0 <= t && t <= 1 && 0 <= u && u <= 1;
        }

        /// <summary>
        /// Get the intersection point with another line, treating this as a segment and the other as an infinite length line.
        /// </summary>
        /// <param name="other">The line to collide with.</param>
        /// <param name="intersection">The intersection point between the two lines, Vector2.NaN if lines intersect infinitely many times.</param>
        /// <returns>True if a collision exists</returns>
        public bool IntersectionSegmentLine(Line other, out Vector2 intersection)
            => other.IntersectionLineSegment(line, out intersection);

        /// <summary>
        /// Get the intersection point with another line, treating this as an infinite length line and the other as a segment.
        /// </summary>
        /// <param name="other">The line to collide with.</param>
        /// <param name="intersection">The intersection point between the two lines, Vector2.NaN if lines intersect infinitely many times.</param>
        /// <returns>True if a collision exists</returns>
        public bool IntersectionLineSegment(Line other, out Vector2 intersection) {
            intersection = Vector2.Zero;

            float denominator = ((line.From.X - line.To.X) * (other.From.Y - other.To.Y)) - ((line.From.Y - line.To.Y) * (other.From.X - other.To.X));
            if (denominator == 0) {
                // Lines are ontop of one another
                if ((line.From.X - other.From.X) * (other.From.Y - line.To.Y) == (other.From.X - line.To.X) * (line.From.Y - other.From.Y)) {
                    intersection = Vector2.NaN;
                    return true;
                }

                // Lines never intersect
                return false;
            }

            float lineDiff = (line.From.X * line.To.Y) - (line.From.Y * line.To.X);
            float segmentDiff = (other.From.X * other.To.Y) - (other.From.Y * other.To.X);
            float numX = (lineDiff * (other.From.X - other.To.X)) - (segmentDiff * (line.From.X - line.To.X));
            float numY = (lineDiff * (other.From.Y - other.To.Y)) - (segmentDiff * (line.From.Y - line.To.Y));
            float x = numX / denominator;
            float y = numY / denominator;

            // If the intersection falls between either axis bounds of the segment it is contained
            //  Checking the highest distance axis to reduce the risk of floating point error
            if (
                MathF.Abs(other.From.X - other.To.X) > MathF.Abs(other.From.Y - other.To.Y)
                    ? (x < MathF.Min(other.From.X, other.To.X) || x > MathF.Max(other.From.X, other.To.X))
                    : (y < MathF.Min(other.From.Y, other.To.Y) || y > MathF.Max(other.From.Y, other.To.Y))
            ) {
                return false;
            }

            intersection = new(x, y);
            return true;
        }
    }
}