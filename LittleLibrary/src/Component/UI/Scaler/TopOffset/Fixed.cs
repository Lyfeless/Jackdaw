namespace LittleLib;

public class TopFixedOffsetScalerElement(ScalerComponent component, float offset) : ScalerElement(component) {
    readonly float Offset = offset;
    public override float Get() => Component.ParentBounds.Rect.Top + Offset;
}