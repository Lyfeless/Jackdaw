using System.Numerics;
using Foster.Framework;

namespace LittleLib;

/// <summary>
/// A matrix container that automatically stores its inverse.
/// </summary>
public struct InvertableMatrix {
    Matrix3x2 matrix = Matrix3x2.Identity;
    Matrix3x2 matrixInverse = Matrix3x2.Identity;

    /// <summary>
    /// Create a new object from a matrix.
    /// </summary>
    /// <param name="matrix">The matrix to store.</param>
    public InvertableMatrix(Matrix3x2 matrix) {
        this.matrix = matrix;
        Matrix3x2.Invert(matrix, out matrixInverse);
    }

    /// <summary>
    /// Create a new object from a matrix and a precomputed inverse.
    /// </summary>
    /// <param name="matrix">The matrix to store.</param>
    /// <param name="matrixInverse">The matrix inverse.</param>
    InvertableMatrix(Matrix3x2 matrix, Matrix3x2 matrixInverse) {
        this.matrix = matrix;
        this.matrixInverse = matrixInverse;
    }

    /// <summary>
    /// The stored matrix.
    /// </summary>
    public Matrix3x2 Matrix {
        readonly get => matrix;
        set {
            matrix = value;
            Matrix3x2.Invert(matrix, out matrixInverse);
        }
    }

    /// <summary>
    /// The inverted matrix.
    /// </summary>
    public Matrix3x2 MatrixInverse {
        readonly get => matrixInverse;
        set {
            matrixInverse = value;
            Matrix3x2.Invert(matrixInverse, out matrix);
        }
    }

    public static implicit operator InvertableMatrix(Matrix3x2 v) => new(v);
    public static implicit operator InvertableMatrix(Transform v) => new(v.Matrix, v.MatrixInverse);
}