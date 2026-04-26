using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Utility functions for doing calculations with 2d vectors.
/// </summary>
public static class Vector2Extensions {
    extension(Vector2 v) {
        /// <summary>
        /// Returns the dot product with another vector.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The dot product.</returns>
        public float Dot(Vector2 other) => Vector2.Dot(v, other);


        /// <summary>
        /// If both vectors are less than 90 degrees apart.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>If the two vectors are less than 90 degrees apart.</returns>
        public bool SameDirection(Vector2 other) => v.Dot(other) > 0;

        /// <summary>
        /// If both vectors are less than or exactly 90 degrees apart.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>If the two vectors are less than or exactly 90 degrees apart.</returns>
        public bool SameDirectionInclusive(Vector2 other) => v.Dot(other) >= 0;

        /// <summary>
        /// If both vectors are greater than 90 degrees apart.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>If the two vectors are greater than 90 degrees apart.</returns>
        public bool OppositeDirection(Vector2 other) => !v.SameDirectionInclusive(other);

        /// <summary>
        /// If both vectors are greater than or exactly 90 degrees apart.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>If the two vectors are greater than or exactly 90 degrees apart.</returns>
        public bool OppositeDirectionInclusive(Vector2 other) => !v.SameDirection(other);

        /// <summary>
        /// Get a vector perpendicular to the line between two vectors, facing away from a point.
        /// </summary>
        /// <param name="a">The start of the line.</param>
        /// <param name="b">The end of the line.</param>
        /// <param name="target">The point to face away from.</param>
        /// <returns>The perpendicular line.</returns>
        public static Vector2 PerpendicularAway(Vector2 a, Vector2 b, Vector2 target)
        => -PerpendicularToward(a, b, target);

        /// <summary>
        /// Get a vector perpendicular to the line between two vectors, facing towards from a point.
        /// </summary>
        /// <param name="a">The start of the line.</param>
        /// <param name="b">The end of the line.</param>
        /// <param name="target">The point to face towards from.</param>
        /// <returns>The perpendicular line.</returns>
        public static Vector2 PerpendicularToward(Vector2 a, Vector2 b, Vector2 target) {
            Vector2 perp = (a - b).TurnLeft();
            if (OppositeDirection(perp, target - a)) { return -perp; }
            return perp;
        }

        /// <summary>
        /// Change a vector's rotation.
        /// </summary>
        /// <param name="by">The amount to rotate relatively.</param>
        /// <returns>The rotated vector.</returns>
        public Vector2 Rotate(float by) => Calc.AngleToVector(v.Angle() + by, v.Length());
    }
}