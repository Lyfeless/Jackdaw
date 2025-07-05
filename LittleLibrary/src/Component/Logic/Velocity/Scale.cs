namespace LittleLib;

public class DampenerScale(float scaler) : VelocityDampener() {
    readonly float Scaler = scaler;

    public override float Apply(float value, float delta) => value * Scaler;
}