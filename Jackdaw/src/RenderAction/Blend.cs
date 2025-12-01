using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A render action that applies a blend mode to all component and child elements.
/// </summary>
/// <param name="blend">The blend mode to apply.</param>
public class RenderActionBlend(BlendMode blend) : ActorRenderAction() {
    public BlendMode Blend = blend;

    public override void PreRender(RenderActionContainer container) {
        container.CurrentBatcher.PushBlend(Blend);
    }

    public override void PostRender(RenderActionContainer container) {
        container.CurrentBatcher.PopBlend();
    }
}