namespace LittleLib;

public class LeftOffsetScalerElement(ScalerComponent component, ScalerElement offsetValue) : ScalerElement(component) {
    readonly ScalerElement OffsetValue = offsetValue;
    public override float Get() => Component.ParentBounds.Rect.Left + OffsetValue.Get();
}