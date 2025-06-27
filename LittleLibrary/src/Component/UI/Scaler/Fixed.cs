namespace LittleLib;

public class FixedScalerElement(ScalerComponent component, float value) : ScalerElement(component) {
    readonly float Value = value;
    public override float Get() => Value;
}