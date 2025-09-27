using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A render action that applies a new sampler to all component and child elements.
/// </summary>
/// <param name="sampler">The sampler to apply.</param>
public class RenderActionSampler(TextureSampler sampler) : ActorRenderAction() {
    public TextureSampler Sampler = sampler;

    public override void PreRender(RenderActionContainer container) {
        container.CurrentBatcher.PushSampler(sampler);
    }

    public override void PostRender(RenderActionContainer container) {
        container.CurrentBatcher.PopSampler();
    }
}