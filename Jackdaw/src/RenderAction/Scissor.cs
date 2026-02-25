using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A render action that applies a scissor to clip the rendering of all components and children into a rectangle.
/// </summary>
/// <param name="bounds">The clip bounds.</param>
public class RenderActionScissor(BoundsComponent bounds) : ActorRenderAction() {
    /// <summary>
    /// The clip bounds.
    /// </summary>
    public BoundsComponent Bounds = bounds;

    /// <summary>
    /// If the scissor should be relative to the actor it's attached to.
    /// </summary>
    public bool Relative = true;

    public override void PreRender(RenderActionContainer container) {
        Rect rect = Bounds.Rect;
        if (Relative) { rect = rect.Translate(container.Owner.GlobalPosition); }
        container.CurrentBatcher.PushScissor(rect.Int());
    }

    public override void PostRender(RenderActionContainer container) {
        container.CurrentBatcher.PopScissor();
    }
}