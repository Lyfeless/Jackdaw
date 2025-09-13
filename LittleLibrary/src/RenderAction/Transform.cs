using System.Numerics;
using Foster.Framework;

namespace LittleLib;

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