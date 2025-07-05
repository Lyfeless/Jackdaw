namespace LittleLib;

public class DampenerFixedAmount(float amount) : VelocityDampener() {
    readonly float Amount = amount;

    public override float Apply(float value, float delta) => value + Amount;
}