namespace LittleLib;

public class BottomFixedOffsetScalerElement(ScalerComponent component, float offset) : ScalerElement(component) {
    readonly float Offset = offset;
    public override float Get() => Component.ParentBounds.Rect.Bottom + Offset;
}