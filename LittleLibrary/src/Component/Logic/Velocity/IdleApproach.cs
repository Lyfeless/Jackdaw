using Foster.Framework;

namespace LittleLib;

public class DampenerIdleApproach(float speed, float target) : VelocityDampener() {
    readonly float Speed = speed;
    readonly float Target = target;

    public override float Apply(float value, float delta) {
        if (delta != 0) { return value; }
        return Calc.Approach(value, Target, Speed);
    }
}