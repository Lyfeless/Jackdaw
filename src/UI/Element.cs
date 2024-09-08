using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class UIElement {
    public string Id;

    public bool Selectable { get; private set; }
    public bool Active { get; private set; } = false;
    public bool Selected { get; private set; } = false;
    public bool ParentSelected => Selected || (Parent != null && Parent.ParentSelected);

    public UIElement? Parent;

    public UIElement? ConnectionUp = null;
    public UIElement? ConnectionDown = null;
    public UIElement? ConnectionLeft = null;
    public UIElement? ConnectionRight = null;

    UIVector2 position;
    UIVector2 size;
    UIVector2 anchor;

    public void SetPosition(UIVector2 value) => position = value;
    public void SetSize(UIVector2 value) => size = value;
    public void SetAnchor(UIVector2 value) => anchor = value;

    readonly HashSet<UIElement> children = [];

    public Vector2 RelativePosition {
        // get => position.GetAppliedValue(Parent?.Size ?? Vector2.Zero);
        get {
            // Console.WriteLine(position.GetAppliedValue(Parent?.Size ?? Vector2.Zero));
            return position.GetAppliedValue(Parent?.Size ?? Vector2.Zero);
        }
        set => position.value = value;
    }
    public Vector2 AbsolutePosition => RelativePosition + (Parent?.AbsoluteTopLeft ?? Vector2.Zero);

    public Vector2 RelativeTopLeft => RelativePosition - anchor.GetAppliedValue(Size);
    public Vector2 AbsoluteTopLeft => RelativeTopLeft + (Parent?.AbsoluteTopLeft ?? Vector2.Zero);

    public virtual Vector2 Size {
        //! FIXME (Alex): Root can be avoided as a concept if the root case was just handled as a null check
        get => size.GetAppliedValue(Parent?.Size ?? Vector2.Zero);
        set => size.value = value;
    }

    public Rect RelativeBounds {
        get {
            Vector2 topLeft = RelativeTopLeft;
            return new(topLeft, topLeft + Size);
        }
    }
    public Rect AbsoluteBounds {
        get {
            Vector2 topLeft = AbsoluteTopLeft;
            return new(topLeft, topLeft + Size);
        }
    }

    float z;
    public float Z {
        get => z + (Parent?.Z ?? 0);
        set => z = value;
    }

    public UIElement() : this(new()) { }
    public UIElement(UICreateArgs args) {

        Id = args.ID;

        Selectable = args.Selectable;

        Parent = args.Parent;

        position = args.Position;
        size = args.Size;
        anchor = args.Anchor;

        z = args.Z;

        foreach (UIElement child in args.Children) {
            AddChild(child);
        }
    }

    public UIElement ConnectUp(UIElement element) {
        ConnectionUp = element;
        element.ConnectionDown = this;
        return this;
    }

    public UIElement ConnectDown(UIElement element) {
        ConnectionDown = element;
        element.ConnectionUp = this;
        return this;
    }

    public UIElement ConnectLeft(UIElement element) {
        ConnectionLeft = element;
        element.ConnectionRight = this;
        return this;
    }

    public UIElement ConnectRight(UIElement element) {
        ConnectionRight = element;
        element.ConnectionLeft = this;
        return this;
    }

    public virtual void Select() { }
    public void OnSelect() {
        Select();
        Selected = true;
    }

    public virtual void Deselect() { }
    public void OnDeselect() {
        Deselect();
        Selected = false;
    }

    public virtual void Activate() { }
    public void OnActivate() {
        Activate();
        Active = true;

        foreach (UIElement element in children) {
            UIManager.AddElement(element);
        }
    }
    public virtual void Deactivate() { }
    public void OnDeactivate() {
        Deactivate();
        Active = false;

        foreach (UIElement element in children) {
            UIManager.RemoveElement(element);
        }
    }

    public UIElement SplitHorizontal(UIElement Left, UIElement Right, float split = 0.5f) =>
        AddNewChild(new() { Children = [Left], Parent = this, Size = UIVector2.Scaling(split, 1) })
        .AddNewChild(new() { Children = [Right], Parent = this, Size = UIVector2.Scaling(1 - split, 1), Position = UIVector2.Scaling(split, 0) });

    public UIElement SplitVertical(UIElement Top, UIElement Bottom) =>
        AddNewChild(new() { Children = [Top], Parent = this, Size = UIVector2.Right })
        .AddNewChild(new() { Children = [Bottom], Parent = this, Size = UIVector2.Right, Position = UIVector2.Left });

    public UIElement ContainedListHorizontal(params UIElement[] elements) {
        float width = 1 / elements.Length;
        for (int i = 0; i < elements.Length; ++i) {
            AddNewChild(new() { Children = [elements[i]], Size = UIVector2.Scaling(width, 1), Position = UIVector2.Scaling(i * width, 0) });
        }

        return this;
    }

    public UIElement ContainedListVertical(params UIElement[] elements) {
        float height = 1.0f / elements.Length;
        for (int i = 0; i < elements.Length; ++i) {
            AddNewChild(new() { Children = [elements[i]], Size = UIVector2.Scaling(1, height), Position = UIVector2.Scaling(0, i * height) });
        }

        return this;
    }

    // FixedListHorizontal
    // FixedListVertical
    // ContainedGrid
    // FixedGrid

    public UIElement AddNewChild(UICreateArgs args, bool setParent = true) => AddChild(new(args), setParent);
    public UIElement AddChild(UIElement element, bool setParent = true) {
        children.Add(element);
        if (setParent) { element.Parent = this; }
        if (Active) { UIManager.AddElement(element); }

        return this;
    }

    public UIElement RemoveChild(UIElement element) {
        children.Remove(element);
        if (Active) { UIManager.RemoveElement(element); }

        return this;
    }

    public virtual void Update() { }

    public virtual void Render(Batcher batcher) {
        // batcher.RectLine(RelativeBounds, 1, Color.Yellow);
    }
}