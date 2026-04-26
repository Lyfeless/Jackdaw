using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A render action that applies a new matrix transform to all component and child elements.
/// </summary>
/// <param name="matrix">The matrix to apply.</param>
public class RenderActionTransform(Matrix3x2 matrix) : ActorRenderAction() {
    /// <summary>
    /// The matrix to transform the render by.
    /// </summary>
    public Matrix3x2 Matrix = matrix;

    public override Matrix3x2 PositionOffset => Matrix;

    public RenderActionTransform(Transform transform) : this(transform.Matrix) { }
}