namespace LittleLib;

public class RightFixedOffsetScalerElement(ScalerComponent component, float offset) : ScalerElement(component) {
    readonly float Offset = offset;
    public override float Get() => Component.ParentBounds.Rect.Right + Offset;
}