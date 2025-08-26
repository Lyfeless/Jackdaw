using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class RenderActionSampler(TextureSampler sampler) : ActorRenderAction() {
    public TextureSampler Sampler = sampler;

    public override void PostRenderPhase(RenderActionContainer container, Batcher batcher) {
        batcher.PushSampler(sampler);
    }

    public override void PreRenderPhase(RenderActionContainer container, Batcher batcher) {
        batcher.PopSampler();
    }
}