using LittleLib;

public struct UICreateArgs() {
    public string ID = string.Empty;

    public bool Selectable = false;

    public UIVector2 Position = UIVector2.Pixel(0);
    public UIVector2 Size = UIVector2.BottomRight;
    public UIVector2 Anchor = UIVector2.TopLeft;

    public float Z = 1;

    public UIElement? Parent = null;
    public UIElement[] Children = [];
}