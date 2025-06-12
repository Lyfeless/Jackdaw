namespace LittleLib;

public class RightOffsetScalerElement(ScalerComponent component, ScalerElement offsetValue) : ScalerElement(component) {
    readonly ScalerElement OffsetValue = offsetValue;
    public override float Get() => Component.ParentBounds.Rect.Right + OffsetValue.Get();
}