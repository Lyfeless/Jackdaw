using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class RenderActionMaterial(Material material) : ActorRenderAction() {
    public Material Material = material;

    public override void PostRenderPhase(RenderActionContainer container, Batcher batcher) {
        batcher.PushMaterial(Material);
    }

    public override void PreRenderPhase(RenderActionContainer container, Batcher batcher) {
        batcher.PopMaterial();
    }
}