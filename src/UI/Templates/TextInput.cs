using Foster.Framework;
using TextCopy;

namespace LittleLib;

public abstract class UITextInput(string startValue, Action<string> changeCallback, UICreateArgs args, string? allowedCharacters = null) : UIElement(args) {
    public string Value = startValue;
    public Action<string> Callback = changeCallback;

    //! FIXME (Alex): Check if giving a null has issues, ignore check if empty
    readonly bool limitCharacters = allowedCharacters != null;
    readonly System.Buffers.SearchValues<char> s_allowedCharacters = System.Buffers.SearchValues.Create(allowedCharacters);

    //! FIXME (Alex): Max width, word wrapping?

    public override void Update() {
        if (!ParentSelected) { return; }

        if (Input.Keyboard.Text.Length > 0 && (!limitCharacters || !Input.Keyboard.Text.ToString().AsSpan().ContainsAnyExcept(s_allowedCharacters))) { AddText(Input.Keyboard.Text.ToString()); }
        if (Input.Keyboard.CtrlOrCommand && Input.Keyboard.Pressed(Keys.V)) {
            string? clipboardText = ClipboardService.GetText();
            if (clipboardText != null) { AddText(clipboardText); }
        }
        if (Value.Length > 0 && Input.Keyboard.PressedOrRepeated(Keys.Backspace)) {
            SetText(Value[..^1]);
        }
    }

    protected virtual void AddText(string text) {
        SetText(Value + text);
    }

    protected virtual void SetText(string text) {
        Value = text;
        Callback(Value);
    }
}