using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public abstract class ActorRenderAction() {
    public ObjectIdentifier<ActorRenderAction> Match;
    public virtual Matrix3x2 PositionOffset { get; } = Matrix3x2.Identity;
    public virtual void PreRender(RenderActionContainer container) { }
    public virtual void PostRender(RenderActionContainer container) { }
}