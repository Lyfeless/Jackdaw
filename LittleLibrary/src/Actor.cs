using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class Actor {
    public static Actor Empty(LittleGame game) => new(game);
    public readonly static Actor Invalid = new(null) { IsValid = false, Ticking = false, Visible = false };

    public LittleGame Game;

    public Actor Parent;
    public ActorContainer Children;

    bool isValid = true;
    public bool IsValid {
        get => isValid;
        //! FIXME (Alex): Stop actor from being re-validated, is this smart?
        protected set { if (!value) { isValid = false; } }
    }

    public bool ParentValid => Parent != null && Parent.IsValid;

    public ComponentContainer Components;

    public RenderablePosition Position = new();

    public Vector2 GlobalPosition {
        get => ParentValid
            ? Position.Precise + Parent.GlobalPosition
            : Position.Precise;
    }

    public Point2 GlobalPositionRounded {
        get => ParentValid
            ? Position.Rounded + Parent.GlobalPositionRounded
            : Position.Rounded;
    }

    public ObjectIdentifier<Actor> Match;

    public bool ComponentsVisible = true;
    public bool ChildrenVisible = true;
    public bool Visible {
        get => ComponentsVisible && ChildrenVisible;
        set {
            ComponentsVisible = value;
            ChildrenVisible = value;
        }
    }
    public bool GlobalVisible {
        get {
            return ParentValid ? Parent.GlobalVisible : Visible;
        }
    }

    public bool ComponentsTicking = true;
    public bool ChildrenTicking = true;
    public bool Ticking {
        get => ComponentsTicking && ChildrenTicking;
        set {
            ComponentsTicking = value;
            ChildrenTicking = value;
        }
    }
    public bool GlobalTicking {
        get {
            return ParentValid ? Parent.GlobalTicking : Ticking;
        }
    }

    public bool InTree {
        get => Game != null && ParentMatches(Game.Root);
    }

    public bool addedToTree = false;
    public bool AddedToTree {
        get => addedToTree;
        protected set { if (value) { addedToTree = true; } }
    }

    public Actor(LittleGame game) {
        Game = game;
        Parent = Invalid;

        Match = new(this);

        Children = new(this);
        Components = new(this);
    }

    public void Update() {
        if (ComponentsTicking) {
            foreach (Component component in Components.Elements) {
                if (component.Ticking) { component.Update(); }
            }
        }

        if (ChildrenTicking) {
            foreach (Actor child in Children.Elements) {
                child.Update();
            }
        }
    }

    //! FIXME (Alex): batcher maybe doesnt need to be passed? They already have access to game
    //      Actually, actor should store current batcher and other batcher effects to allow for components to control rendering of others
    public void Render(Batcher batcher) {
        batcher.PushMatrix(Position.Rounded);

        //! FIXME (Alex): Define culling box for actor?

        if (ComponentsVisible) {
            foreach (Component component in Components.Elements) {
                if (component.Visible) { component.Render(batcher); }
            }
        }

        if (ChildrenVisible) {
            foreach (Actor child in Children.Elements) {
                child.Render(batcher);
            }
        }

        batcher.PopMatrix();
    }

    public void ApplyChanges() {
        Components.ApplyChanges();
        Children.ApplyChanges();
        foreach (Actor child in Children.Elements) {
            child.ApplyChanges();
        }
    }

    public void EnterTree() {
        if (!AddedToTree) {
            EnterTreeFirst();
            AddedToTree = true;
        }

        foreach (Actor child in Children.Elements) {
            child.EnterTree();
        }

        foreach (Component component in Components.Elements) {
            component.EnterTree();
            if (!component.AddedToTree) {
                component.EnterTreeFirst();
                component.AddedToTree = true;
            }
        }
    }

    public void EnterTreeFirst() { }

    public void ExitTree() {
        foreach (Component component in Components.Elements) {
            component.ExitTree();
        }

        foreach (Actor child in Children.Elements) {
            child.ExitTree();
        }
    }

    public void Invalidate(bool invalidateChildren = true) {
        if (!IsValid) { return; }

        if (invalidateChildren) {
            InvalidateChildren();
        }

        Children.Clear();
        Components.Clear();

        if (ParentValid) {
            Parent.Children.Remove(this);
        }
    }

    void InvalidateChildren() {
        foreach (Actor child in Children.Elements) {
            child.InvalidateChildren();
        }

        IsValid = false;
    }

    bool ParentMatches(Actor check) {
        if (this == check) { return true; }
        if (ParentValid) { return Parent.ParentMatches(check); }
        return false;
    }

    public Vector2 LocalToGlobal(Vector2 position) {
        return position + GlobalPosition;
    }

    public Vector2 GlobalToLocal(Vector2 position) {
        return position - GlobalPosition;
    }

    public Vector2 FromOtherLocal(Actor originLocal, Vector2 position) {
        return GlobalToLocal(originLocal.LocalToGlobal(position));
    }

    public Vector2 FromOtherLocal(Actor originLocal) {
        return GlobalToLocal(originLocal.GlobalPosition);
    }

    public override string ToString() {
        return Match.ToString();
    }

    public static Actor From(LittleGame game, params Actor[] children) => With(new(game), children);
    public static Actor With(Actor actor, params Actor[] children) {
        foreach (Actor child in children) {
            actor.Children.Add(child);
        }
        return actor;
    }

    public static Actor From(LittleGame game, params Component[] components) => With(new(game), components);
    public static Actor With(Actor actor, params Component[] components) {
        foreach (Component component in components) {
            actor.Components.Add(component);
        }
        return actor;
    }
}