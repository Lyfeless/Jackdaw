using System.Numerics;
using Foster.Framework;

namespace LittleLib;

//! FIXME (Alex): DOC COMMENTS

public struct InvertableMatrix {
    Matrix3x2 matrix;
    Matrix3x2 matrixInverse;

    public InvertableMatrix(Matrix3x2 matrix) {
        this.matrix = matrix;
        Matrix3x2.Invert(matrix, out matrixInverse);
    }

    InvertableMatrix(Matrix3x2 matrix, Matrix3x2 matrixInverse) {
        this.matrix = matrix;
        this.matrixInverse = matrixInverse;
    }

    public Matrix3x2 Matrix {
        readonly get => matrix;
        set {
            matrix = value;
            Matrix3x2.Invert(matrix, out matrixInverse);
        }
    }

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