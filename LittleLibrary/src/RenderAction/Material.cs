using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class RenderActionMaterial(Material material) : ActorRenderAction() {
    public Material Material = material;

    public override void PreRender(RenderActionContainer container) {
        container.CurrentBatcher.PushMaterial(Material);
    }

    public override void PostRender(RenderActionContainer container) {
        container.CurrentBatcher.PopMaterial();
    }
}