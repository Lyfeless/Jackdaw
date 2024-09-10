using System.Numerics;
using Foster.Framework;

namespace LittleLib;

//! FIXME (Alex): This is identical to the horizontal slider with different values. I think I just need to not care?
public abstract class UISliderVertical(float startValue, float minValue, float maxValue, float stepSize, Action<float> changeCallback, UICreateArgs args) : UIElement(args) {
    public float Value { get; protected set; } = startValue;
    public float MinValue { get; protected set; } = minValue;
    public float MaxValue { get; protected set; } = maxValue;
    public float StepSize { get; protected set; } = stepSize;

    public Action<float> ChangeCallback { get; protected set; } = changeCallback;

    public override void Update() {
        if (!ParentSelected) { return; }

        if (UIManager.GetUpInput()) {
            SetValue(Value - StepSize);
        }
        else if (UIManager.GetDownInput()) {
            SetValue(Value + StepSize);
        }
        else if (UIManager.UseInput?.Down ?? false) {
            //! FIXME (Alex): There should be a utility function for checking mouse in bounds
            Vector2 mousePosition = ScreenScaler.GetScaledMousePosition();
            Rect bounds = AbsoluteBounds;
            SetValue(((mousePosition.Y - bounds.Y) / Size.Y * (MaxValue - MinValue)) + MinValue);
        }
    }

    void SetValue(float value) {
        Value = Calc.Clamp(value, MinValue, MaxValue);
        if (value == Value) { return; }
        ChangeCallback(Value);
    }
}