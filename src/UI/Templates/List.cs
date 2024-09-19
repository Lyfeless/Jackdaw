namespace LittleLib;

public class UIList(UICreateArgs args) : UIElement(args) {
    UIElement? defaultSelect;

    public UIList() : this(new()) { }

    public UIList ContainedListHorizontal(bool connect, bool loop, bool setDefault, params UIElement[] elements) {
        float width = 1.0f / elements.Length;
        int lastSelectable = -1;

        for (int i = 0; i < elements.Length; ++i) {
            AddNewChild(new() { Children = [elements[i]], Size = UIVector2.Scaling(width, 1), Position = UIVector2.Scaling(i * width, 0) });
            if (connect) {
                if (elements[i].ParentSelectable) {
                    if (defaultSelect == null) { defaultSelect = elements[i]; }
                    else {
                        elements[i].ConnectUp(elements[lastSelectable]);
                    }

                    lastSelectable = i;
                }
            }
        }

        if (loop && defaultSelect != null && defaultSelect != elements[lastSelectable]) {
            defaultSelect?.ConnectUp(elements[lastSelectable]);
        }

        if (setDefault) { SetDefaultSelect(); }

        return this;
    }

    public UIList ContainedListVertical(bool connect, bool loop, bool setDefault, params UIElement[] elements) {
        float height = 1.0f / elements.Length;
        int lastSelectable = -1;

        for (int i = 0; i < elements.Length; ++i) {
            AddNewChild(new() { Children = [elements[i]], Size = UIVector2.Scaling(1, height), Position = UIVector2.Scaling(0, i * height) });
            if (connect) {
                if (elements[i].ParentSelectable) {
                    if (defaultSelect == null) { defaultSelect = elements[i]; }
                    else {
                        elements[i].ConnectUp(elements[lastSelectable]);
                    }

                    lastSelectable = i;
                }
            }
        }

        if (loop && defaultSelect != null && defaultSelect != elements[lastSelectable]) {
            defaultSelect?.ConnectUp(elements[lastSelectable]);
        }

        if (setDefault) { SetDefaultSelect(); }

        return this;
    }

    public void SetDefaultSelect() {
        if (defaultSelect == null) { return; }
        UIManager.DefaultElement = defaultSelect;
        UIManager.SetSelected(UIManager.DefaultElement);
    }

    //HorizontalFixed
    //VerticalFixed
}