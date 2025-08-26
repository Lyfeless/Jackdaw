using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public abstract class ActorRenderAction() {
    public bool ApplyToComponents = true;
    public bool ApplyToChildren = true;

    public virtual Matrix3x2 PositionOffset { get; } = Matrix3x2.Identity;
    public virtual void PreRender(RenderActionContainer container, Batcher batcher) { }
    public virtual void PostRender(RenderActionContainer container, Batcher batcher) { }
    public abstract void PreRenderPhase(RenderActionContainer container, Batcher batcher);
    public abstract void PostRenderPhase(RenderActionContainer container, Batcher batcher);
}