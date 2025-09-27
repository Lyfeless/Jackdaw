using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A render action that applies a new matrix transform to all component and child elements.
/// </summary>
/// <param name="matrix">The matrix to apply.</param>
public class RenderActionTransform(Matrix3x2 matrix) : ActorRenderAction() {
    public Matrix3x2 Matrix = matrix;

    public RenderActionTransform(Transform transform) : this(transform.Matrix) { }

    public override void PreRender(RenderActionContainer container) {
        container.CurrentBatcher.PushMatrix(Matrix);
    }

    public override void PostRender(RenderActionContainer container) {
        container.CurrentBatcher.PopMatrix();
    }
}