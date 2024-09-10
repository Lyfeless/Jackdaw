namespace LittleLib;

public abstract class UIAction(Action callback, UICreateArgs args) : UIElement(args) {
    public readonly Action Callback = callback;

    public override void Update() {
        if (!ParentSelected) { return; }

        if (UIManager.GetUseInput()) {
            Callback();
        }
    }
}