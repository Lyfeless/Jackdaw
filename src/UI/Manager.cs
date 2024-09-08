using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public static class UIManager {
    static List<UIElement> elements = [];
    public static UIElement? SelectedElement { get; private set; } = null;
    public static UIElement? DefaultElement = null;
    public static readonly UIRoot Root = new();

    public static VirtualStick? MoveInput = null;
    public static VirtualButton? UseInput = null;
    public static bool CheckMouse = true;

    static List<UIElement> clearList = [];

    public static bool AcceptingInput = true;

    public static void AddElement(UIElement element) {
        elements.Add(element);
        element.OnActivate();
        element.Parent ??= Root;
    }

    public static void RemoveElement(UIElement element) {
        clearList.Add(element);
        element.OnDeactivate();
    }

    public static void ClearElements() {
        clearList = [.. elements];
        DefaultElement = null;
    }

    public static void SetSelected(UIElement? element) {
        if (element == SelectedElement) { return; }
        if (element != null && (!(element?.Selectable ?? true) || !elements.Contains(element))) { return; }

        SelectedElement?.OnDeselect();
        SelectedElement = element;
        SelectedElement?.OnSelect();
    }

    public static bool GetUpInput() {
        return MoveInput?.Vertical.Negative.ConsumePress() ?? false;
    }

    public static bool GetDownInput() {
        return MoveInput?.Vertical.Positive.ConsumePress() ?? false;
    }

    public static bool GetLeftInput() {
        return MoveInput?.Horizontal.Negative.ConsumePress() ?? false;
    }

    public static bool GetRightInput() {
        return MoveInput?.Horizontal.Positive.ConsumePress() ?? false;
    }

    public static bool GetUseInput() {
        return UseInput?.ConsumePress() ?? false;
    }

    public static UIElement? FindElementByID(string id) {
        return elements.Find(e => e.Id == id);
    }

    public static UIElement[] FindElementsByID(string id) {
        return elements.Where(e => e.Id == id).ToArray();
    }

    public static T? FindElementByType<T>() where T : UIElement {
        return (T?)elements.Find(e => e.GetType() == typeof(T));
    }

    public static T[] FindElementsByID<T>() where T : UIElement {
        return elements.Where(e => e.GetType() == typeof(T)).Cast<T>().ToArray();
    }

    public static void Update() {
        UIElement? mouseHover = null;
        bool mouseMoved = Util.MouseMoved();
        Vector2 mousePos = ScreenScaler.GetScaledMousePosition();

        if (AcceptingInput && MoveInput != null && MoveInput?.Value != Vector2.Zero) {
            if (SelectedElement != null) {
                if (SelectedElement.ConnectionUp != null && GetUpInput()) {
                    SetSelected(SelectedElement.ConnectionUp);
                }
                else if (SelectedElement.ConnectionDown != null && GetDownInput()) {
                    SetSelected(SelectedElement.ConnectionDown);
                }
                else if (SelectedElement.ConnectionLeft != null && GetLeftInput()) {
                    SetSelected(SelectedElement.ConnectionLeft);
                }
                else if (SelectedElement.ConnectionRight != null && GetRightInput()) {
                    SetSelected(SelectedElement.ConnectionRight);
                }
            }
            else {
                SetSelected(DefaultElement);
            }
        }

        foreach (UIElement element in elements) {
            element.Update();

            if (AcceptingInput && CheckMouse && element.Selectable && mouseMoved) {
                Rect bounds = element.AbsoluteBounds;
                if (
                    mousePos.X >= bounds.Left &&
                    mousePos.X <= bounds.Right &&
                    mousePos.Y >= bounds.Top &&
                    mousePos.Y <= bounds.Bottom
                ) {
                    mouseHover = element;
                }
            }
        }

        if (mouseMoved) {
            SetSelected(mouseHover);
        }

        foreach (UIElement element in clearList) {
            elements.Remove(element);
        }
        clearList.Clear();
    }

    public static void Render(Batcher batcher) {
        elements = [.. elements.OrderBy(e => e.Z)];
        foreach (UIElement element in elements) {
            //! FIXME (Alex): This will likely need to be adjusted once the scaling is figured out
            batcher.PushMatrix(element.Parent?.AbsoluteTopLeft ?? Vector2.Zero);
            element.Render(batcher);
            batcher.PopMatrix();
        }
    }
}