using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class Actor {
    public static Actor Empty(LittleGame game) => new(game);
    public readonly static Actor Invalid = new(null) { IsValid = false };

    public LittleGame Game;

    public Actor Parent;
    public ActorContainer Children;

    bool isValid = true;
    public bool IsValid {
        get => isValid;
        //! FIXME (Alex): Stop actor from being re-validated, is this smart?
        protected set { if (!value) { isValid = false; } }
    }

    public ComponentContainer Components;

    public RenderablePosition Position = new();
    //! FIXME (Alex): Only gives precise position because I can't image this will be rendered, is that a bad assumption?
    public Vector2 GlobalPosition {
        get => Parent.IsValid
            ? Position.Precise + Parent.GlobalPosition
            : Position.Precise;
    }
    //! FIXME (Alex): Need utils for converting positions between local and global

    public ObjectIdentifier<Actor> Match;

    public bool Visible = true;
    public bool ChildrenVisible = true;
    public bool Ticking = true;
    public bool ChildrenTicking = true;

    public bool InTree {
        get => ParentMatches(Game.Root);
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
        Components.QueueEvents = true;
        Children.QueueEvents = true;

        if (Ticking) {
            foreach (Component component in Components.Elements) {
                if (component.Ticking) { component.Update(); }
            }
        }

        //! FIXME (Alex): Should children tick if parent isn't ticking?
        if (ChildrenTicking) {
            foreach (Actor child in Children.Elements) {
                child.Update();
            }
        }

        Components.QueueEvents = false;
        Children.QueueEvents = false;

        Children.ApplyChanges();
        Components.ApplyChanges();
    }

    //! FIXME (Alex): batcher maybe doesnt need to be passed? They already have access to game
    //      Actually, actor should store current batcher and other batcher effects to allow for components to control rendering of others
    public void Render(Batcher batcher) {
        batcher.PushMatrix(Position.Rounded);

        Components.QueueEvents = true;
        Children.QueueEvents = true;

        if (Visible) {
            foreach (Component component in Components.Elements) {
                if (component.Visible) { component.Render(batcher); }
            }
        }

        //! FIXME (Alex): Should children render if parent isn't visible?
        if (ChildrenVisible) {
            foreach (Actor child in Children.Elements) {
                child.Render(batcher);
            }
        }

        Components.QueueEvents = false;
        Children.QueueEvents = false;

        batcher.PopMatrix();
    }

    public void EnterTree() {
        Components.QueueEvents = true;
        Children.QueueEvents = true;

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

        Components.QueueEvents = false;
        Children.QueueEvents = false;
    }

    public void EnterTreeFirst() { }

    public void ExitTree() {
        Components.QueueEvents = true;
        Children.QueueEvents = true;

        foreach (Component component in Components.Elements) {
            component.ExitTree();
        }

        foreach (Actor child in Children.Elements) {
            child.ExitTree();
        }

        Components.QueueEvents = false;
        Children.QueueEvents = false;
    }

    public void Invalidate(bool invalidateChildren = true) {
        if (!IsValid) { return; }

        if (invalidateChildren) {
            foreach (Actor child in Children.Elements) {
                child.Invalidate(true);
            }
        }

        Children.Clear();
        Components.Clear();
        IsValid = false;
    }

    //! FIXME (Alex): I don't like these find functions being here but moving them to the containers makes it even messier unfortunately
    public Actor? FindChild(Func<ObjectIdentifier<Actor>, bool> func) {
        foreach (Actor child in Children.Elements) {
            if (func(child.Match)) {
                return child;
            }
        }

        return null;
    }

    public Actor? FindChildRecursive(Func<ObjectIdentifier<Actor>, bool> func) {
        foreach (Actor child in Children.Elements) {
            if (func(child.Match)) { return child; }
            Actor? childResult = child.FindChildRecursive(func);
            if (childResult != null) { return childResult; }
        }

        return null;
    }

    public Component? FindComponent(Func<ObjectIdentifier<Component>, bool> func) {
        foreach (Component component in Components.Elements) {
            if (func(component.Match)) {
                return component;
            }
        }

        return null;
    }

    public Component? FindComponentRecursive(Func<ObjectIdentifier<Component>, bool> func) {
        Component? component = FindComponent(func);
        if (component != null) { return component; }

        foreach (Actor child in Children.Elements) {
            component = child.FindComponentRecursive(func);
            if (component != null) { return component; }
        }

        return null;
    }

    bool ParentMatches(Actor check) {
        if (this == check) { return true; }
        if (Parent.IsValid) { return Parent.ParentMatches(check); }
        return false;
    }

    public override string ToString() {
        return Match.ToString();
    }
}