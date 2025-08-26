using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class RenderActionTransform(Matrix3x2 matrix) : ActorRenderAction() {
    public Matrix3x2 Matrix = matrix;

    public RenderActionTransform(Transform transform) : this(transform.Matrix) { }

    public override void PostRenderPhase(RenderActionContainer container, Batcher batcher) {
        batcher.PushMatrix(Matrix);
    }

    public override void PreRenderPhase(RenderActionContainer container, Batcher batcher) {
        batcher.PopMatrix();
    }
}