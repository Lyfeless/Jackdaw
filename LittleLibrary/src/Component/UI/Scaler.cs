using Foster.Framework;

namespace LittleLib;

public class ScalerComponent(
    LittleGame game,
    ScalerElement? scalerX = null,
    ScalerElement? scalerY = null,
    ScalerElement? scalerW = null,
    ScalerElement? scalerH = null
) : Component(game) {
    public BoundsComponent ActorBounds;
    public BoundsComponent ParentBounds;

    public ScalerElement? ScalerX = scalerX;
    public ScalerElement? ScalerY = scalerY;
    public ScalerElement? ScalerW = scalerW;
    public ScalerElement? ScalerH = scalerH;

    protected override void Added() {
        ActorBounds ??= (BoundsComponent)(Actor.Components.Find(e => e.ByType<BoundsComponent>()) ?? new BoundsComponent(Game, new Rect()));
        ParentBounds ??= (BoundsComponent)(Actor.Parent.Components.Find(e => e.ByType<BoundsComponent>()) ?? new BoundsComponent(Game, new Rect()));
    }

    protected override void Update() {
        //! FIXME (Alex): Cache these values to only update when needed?
        if (ScalerX != null) { Actor.Position.SetX(ScalerX.Get()); }
        if (ScalerY != null) { Actor.Position.SetY(ScalerY.Get()); }
        if (ScalerW != null) { ActorBounds.Size = new(ScalerW.Get(), 0); }
        if (ScalerH != null) { ActorBounds.Size = new(0, ScalerH.Get()); }
    }
}