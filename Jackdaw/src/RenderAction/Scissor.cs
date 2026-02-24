using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A render action that applies a scissor to clip the rendering of all components and children into a rectangle.
/// </summary>
/// <param name="scissor">The clip bounds.</param>
public class RenderActionScissor(RectInt scissor) : ActorRenderAction() {
    public RectInt Scissor = scissor;

    public override void PreRender(RenderActionContainer container) {
        container.CurrentBatcher.PushScissor(Scissor);
    }

    public override void PostRender(RenderActionContainer container) {
        container.CurrentBatcher.PopScissor();
    }
}