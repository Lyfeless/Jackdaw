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

    public override void Added() {
        ActorBounds ??= (BoundsComponent)(Actor.FindComponent(e => e.ByType<BoundsComponent>()) ?? new BoundsComponent(Game, new Rect()));
        ParentBounds ??= (BoundsComponent)(Actor.Parent.FindComponent(e => e.ByType<BoundsComponent>()) ?? new BoundsComponent(Game, new Rect()));
    }

    public override void Update() {
        //! FIXME (Alex): Cache these values to only update when needed?
        if (ScalerX != null) { Actor.Position.SetX(ScalerX.Get()); }
        if (ScalerY != null) { Actor.Position.SetY(ScalerY.Get()); }
        if (ScalerW != null) { ActorBounds.SetWidth(ScalerW.Get()); }
        if (ScalerH != null) { ActorBounds.SetHeight(ScalerH.Get()); }
    }
}

public abstract class ScalerElement(ScalerComponent component) {
    protected ScalerComponent Component = component;

    public abstract float Get();
}

public class FixedScalerElement(ScalerComponent component, float value) : ScalerElement(component) {
    float Value = value;
    public override float Get() => Value;
}

public class LeftOffsetScalerElement(ScalerComponent component, ScalerElement offsetValue) : ScalerElement(component) {
    ScalerElement OffsetValue = offsetValue;
    public override float Get() => Component.ParentBounds.Rect.Left + OffsetValue.Get();
}

public class LeftFixedOffsetScalerElement(ScalerComponent component, float offset) : ScalerElement(component) {
    float Offset = offset;
    public override float Get() => Component.ParentBounds.Rect.Left + Offset;
}

public class RightOffsetScalerElement(ScalerComponent component, ScalerElement offsetValue) : ScalerElement(component) {
    ScalerElement OffsetValue = offsetValue;
    public override float Get() => Component.ParentBounds.Rect.Right + OffsetValue.Get();
}

public class RightFixedOffsetScalerElement(ScalerComponent component, float offset) : ScalerElement(component) {
    float Offset = offset;
    public override float Get() => Component.ParentBounds.Rect.Right + Offset;
}

public class TopOffsetScalerElement(ScalerComponent component, ScalerElement offsetValue) : ScalerElement(component) {
    ScalerElement OffsetValue = offsetValue;
    public override float Get() => Component.ParentBounds.Rect.Top + OffsetValue.Get();
}

public class TopFixedOffsetScalerElement(ScalerComponent component, float offset) : ScalerElement(component) {
    float Offset = offset;
    public override float Get() => Component.ParentBounds.Rect.Top + Offset;
}

public class BottomOffsetScalerElement(ScalerComponent component, ScalerElement offsetValue) : ScalerElement(component) {
    ScalerElement OffsetValue = offsetValue;
    public override float Get() => Component.ParentBounds.Rect.Bottom + OffsetValue.Get();
}

public class BottoFixedOffsetScalerElement(ScalerComponent component, float offset) : ScalerElement(component) {
    float Offset = offset;
    public override float Get() => Component.ParentBounds.Rect.Bottom + Offset;
}

public class PercentParentWidthScalerElement(ScalerComponent component, float percent) : ScalerElement(component) {
    float Percent = percent;
    public override float Get() => Component.ParentBounds.Rect.Width * Percent;
}

public class PercentParentHeightScalerElement(ScalerComponent component, float percent) : ScalerElement(component) {
    float Percent = percent;
    public override float Get() => Component.ParentBounds.Rect.Height * Percent;
}