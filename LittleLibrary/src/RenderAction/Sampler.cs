using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class RenderActionSampler(TextureSampler sampler) : ActorRenderAction() {
    public TextureSampler Sampler = sampler;

    public override void PreRender(RenderActionContainer container) {
        container.CurrentBatcher.PushSampler(sampler);
    }

    public override void PostRender(RenderActionContainer container) {
        container.CurrentBatcher.PopSampler();
    }
}