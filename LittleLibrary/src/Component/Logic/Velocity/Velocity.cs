using System.Numerics;

namespace LittleLib;

public class VelocityComponent(LittleGame game) : Component(game) {
    public Vector2 Value { get; private set; }
    public Vector2 OldValue { get; private set; }
    public Vector2 Delta { get; private set; }

    public VelocityDampener? DampenerX;
    public VelocityDampener? DampenerY;

    protected override void Update() {
        Delta = Value;
        Actor.Position += Value;
        if (DampenerX != null || DampenerY != null) {
            Value = new(DampenerX?.Apply(Value.X, Delta.X) ?? Value.X, DampenerY?.Apply(Value.Y, Delta.X) ?? Value.Y);
        }
    }

    public void Set(Vector2 value) {
        Value = value;
    }

    public void Change(Vector2 amount) {
        Value += amount;
    }
}