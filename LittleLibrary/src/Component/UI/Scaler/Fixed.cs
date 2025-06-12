namespace LittleLib;

public class FixedScalerElement(ScalerComponent component, float value) : ScalerElement(component) {
    float Value = value;
    public override float Get() => Value;
}