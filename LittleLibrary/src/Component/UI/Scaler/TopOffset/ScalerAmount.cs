namespace LittleLib;

public class TopOffsetScalerElement(ScalerComponent component, ScalerElement offsetValue) : ScalerElement(component) {
    readonly ScalerElement OffsetValue = offsetValue;
    public override float Get() => Component.ParentBounds.Rect.Top + OffsetValue.Get();
}