using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A render action that applies a new material to all component and child elements.
/// </summary>
/// <param name="material">The material to apply.</param>
public class RenderActionMaterial(Material material) : ActorRenderAction() {
    public Material Material = material;

    public override void PreRender(RenderActionContainer container) {
        container.CurrentBatcher.PushMaterial(Material);
    }

    public override void PostRender(RenderActionContainer container) {
        container.CurrentBatcher.PopMaterial();
    }
}