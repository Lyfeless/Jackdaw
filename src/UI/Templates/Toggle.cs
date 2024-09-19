namespace LittleLib;

public abstract class UIToggle(bool startValue, Action<bool> changeCallback, UICreateArgs args) : UIElement(args) {
    public bool Value = startValue;
    public Action<bool> Callback = changeCallback;

    public override void Update() {
        if (!ParentSelected) { return; }

        if (UIManager.GetUseInput()) {
            Value = !Value;
            Callback(Value);
        }
    }
}