using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

public static class CalcExtensions {
    extension(Calc) {
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
        /// Get the sign of a number that is always either 1 or -1.
        /// Functions the same as Math.Sign in all cases except 0.
        /// </summary>
        /// <param name="value">The value to get the sign of.</param>
        /// <returns>The sign of the value, treated as 1 for a value of 0.</returns>
        public static int NormalizedSign(float value) {
            return value >= 0 ? 1 : -1;
        }
    }
}