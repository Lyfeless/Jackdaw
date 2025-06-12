namespace LittleLib;

public class BottomOffsetScalerElement(ScalerComponent component, ScalerElement offsetValue) : ScalerElement(component) {
    readonly ScalerElement OffsetValue = offsetValue;
    public override float Get() => Component.ParentBounds.Rect.Bottom + OffsetValue.Get();
}