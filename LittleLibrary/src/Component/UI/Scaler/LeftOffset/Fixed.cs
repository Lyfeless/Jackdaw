namespace LittleLib;

public class LeftFixedOffsetScalerElement(ScalerComponent component, float offset) : ScalerElement(component) {
    readonly float Offset = offset;
    public override float Get() => Component.ParentBounds.Rect.Left + Offset;
}