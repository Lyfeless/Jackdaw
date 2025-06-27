namespace LittleLib;

public class PercentScalerElement(ScalerComponent component, ScalerElement value, float percent) : ScalerElement(component) {
    readonly ScalerElement Value = value;
    readonly float Percent = percent;
    public override float Get() => Value.Get() * Percent;
}