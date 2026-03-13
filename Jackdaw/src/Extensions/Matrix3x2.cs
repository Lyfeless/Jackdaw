using System.Numerics;

namespace Jackdaw;

public static class Matrix3x2Extensions {
    extension(Matrix3x2 mat) {
        /// <summary>
        /// Inverts the specified matrix.
        /// If the matrix cannot be inverted, returns itself.
        /// </summary>
        /// <returns>The inverted matrix, or itsself if the matrix could not be inverted.</returns>
        public Matrix3x2 Invert() {
            if (!Matrix3x2.Invert(mat, out Matrix3x2 inv)) { return mat; }
            return inv;
        }

        /// <summary>
        /// Inverts the specified matrix.
        /// If the matrix cannot be inverted, returns itself.
        /// </summary>
        /// <param name="succeeded">f matrix was converted successfully.</param>
        /// <returns>The inverted matrix, or itsself if the matrix could not be inverted.</returns>
        public Matrix3x2 Invert(out bool succeeded) {
            succeeded = Matrix3x2.Invert(mat, out Matrix3x2 inv);
            if (!succeeded) { return mat; }
            return inv;
        }
    }
}