using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

public static class Vector2Extensions {
    extension(Vector2 v) {
        /// <summary>
        /// Convert a 2D vector's angle without changing its length.
        /// </summary>
        /// <param name="transform">The transform to apply to the direction.</param>
        /// <returns>The transformed direction.</returns>
        public Vector2 TransformDirection(Transform transform)
            => TransformDirection(v, transform.Matrix);

        /// <summary>
        /// Convert a 2D vector's angle without changing its length.
        /// </summary>
        /// <param name="transform">The matrix to transform the direction by.</param>
        /// <returns>The transformed direction.</returns>
        public Vector2 TransformDirection(Matrix3x2 transform) {
            return Vector2.Transform(v, transform) - transform.Translation;
        }
    }
}