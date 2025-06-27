namespace LittleLib;

public class OffsetScalerElement(ScalerComponent component, ScalerElement value, ScalerElement offset) : ScalerElement(component) {
    readonly ScalerElement Value = value;
    readonly ScalerElement Offset = offset;
    public override float Get() => Value.Get() + Offset.Get();
}