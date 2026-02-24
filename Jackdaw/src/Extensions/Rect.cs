using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

public static class RectExtensions {
    extension(Rect rect) {
        /// <summary>
        /// Get axis aligned bounds for a transformed rectangle.
        /// </summary>
        /// <param name="transform">The transform to apply to the rectangle.</param>
        /// <returns>The AABB that contains the rotated rectangle.</returns>
        public Rect TransformAABB(Transform transform)
            => rect.TransformAABB(transform.Matrix);

        /// <summary>
        /// Get axis aligned bounds for a transformed rectangle.
        /// </summary>
        /// <param name="transform">The matrix to transform the rectangle by.</param>
        /// <returns>The AABB that contains the rotated rectangle.</returns>
        public Rect TransformAABB(Matrix3x2 transform) {
            return new BoundsBuilder(
                Vector2.Transform(rect.TopLeft, transform),
                Vector2.Transform(rect.TopRight, transform),
                Vector2.Transform(rect.BottomLeft, transform),
                Vector2.Transform(rect.BottomRight, transform)
            ).Rect;
        }
    }

    extension(RectInt rect) {
        /// <summary>
        /// Get axis aligned bounds for a transformed rectangle.
        /// </summary>
        /// <param name="transform">The transform to apply to the rectangle.</param>
        /// <returns>The AABB that contains the rotated rectangle.</returns>
        public Rect TransformAABB(Transform transform)
            => ((Rect)rect).TransformAABB(transform);

        /// <summary>
        /// Get axis aligned bounds for a transformed rectangle.
        /// </summary>
        /// <param name="transform">The matrix to transform the rectangle by.</param>
        /// <returns>The AABB that contains the rotated rectangle.</returns>
        public Rect TransformAABB(Matrix3x2 transform)
            => ((Rect)rect).TransformAABB(transform);
    }
}