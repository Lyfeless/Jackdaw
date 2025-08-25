using System.Numerics;
using Foster.Framework;

namespace LittleLib;

/// <summary>
/// The primary game object element, responsible for storing, updating, and rending all of its children and components.
/// </summary>
public class Actor {
    /// <summary>
    /// Create a new actor with no children or components.
    /// </summary>
    /// <param name="game">The game instance.</param>
    /// <returns>A new empty actor</returns>
    public static Actor Empty(LittleGame game) => new(game);

    /// <summary>
    /// An invalid actor. Any actors with this as a parent are not part of the scene tree.
    /// </summary>
    public readonly static Actor Invalid = new(null) { IsValid = false, Ticking = false, Visible = false };

    /// <summary>
    /// The active game instance.
    /// </summary>
    public LittleGame Game;

    /// <summary>
    /// The actor's parent in the node tree. <see cref="Invalid"> if no parent is assigned.
    /// </summary>
    public Actor Parent;

    /// <summary>
    /// Actor identifier, used when searching for specific actors in the tree.
    /// </summary>
    public ObjectIdentifier<Actor> Match;

    /// <summary>
    /// All child actors owned by this actor.
    /// </summary>
    public ActorContainer Children;

    /// <summary>
    /// All components attached to the actor.
    /// </summary>
    public ComponentContainer Components;

    /// <summary>
    /// Is the current actor valid and usable. False once the actor has been invalidated.
    /// </summary>
    public bool IsValid { get; private set; } = true;

    /// <summary>
    /// Whether or not the current parent object is an active instance.
    /// </summary>
    public bool ParentValid => Parent != null && Parent.IsValid;

    /// <summary>
    /// The position of the actor. Relative to its current parent.
    /// </summary>
    public RenderablePosition Position = new();

    /// <summary>
    /// The actor's position in world space.
    /// </summary>
    public Vector2 GlobalPosition {
        get => ParentValid
            ? Position.Precise + Parent.GlobalPosition
            : Position.Precise;
    }

    /// <summary>
    /// The actors position in world space, rounded to the nearest integer value.
    /// Used primarily for rendering.
    /// </summary>
    public Point2 GlobalPositionRounded {
        get => ParentValid
            ? Position.Rounded + Parent.GlobalPositionRounded
            : Position.Rounded;
    }

    /// <summary>
    /// If the actor's components should render.
    /// </summary>
    public bool ComponentsVisible = true;

    /// <summary>
    /// If the actor's children should render.
    /// </summary>
    public bool ChildrenVisible = true;

    /// <summary>
    /// If the actor's components and children are rendering.
    /// Assigning changes the value of both <see cref="ComponentsVisible"> and <see cref="ChildrenVisible">
    /// </summary>
    public bool Visible {
        get => ComponentsVisible && ChildrenVisible;
        set {
            ComponentsVisible = value;
            ChildrenVisible = value;
        }
    }

    /// <summary>
    /// If all parents above the actor are rendering their children
    /// </summary>
    public bool GlobalVisible {
        get {
            return ParentValid ? Parent.GlobalVisible : ChildrenVisible;
        }
    }

    /// <summary>
    /// If the actor's components should tick.
    /// </summary>
    public bool ComponentsTicking = true;

    /// <summary>
    /// If the actor's children should tick.
    /// </summary>
    public bool ChildrenTicking = true;

    /// <summary>
    /// If the actor's components and children are ticking.
    /// Assigning changes the value of both <see cref="ComponentsTicking"> and <see cref="ChildrenTicking">
    /// </summary>
    public bool Ticking {
        get => ComponentsTicking && ChildrenTicking;
        set {
            ComponentsTicking = value;
            ChildrenTicking = value;
        }
    }

    /// <summary>
    /// If all parents above the actor are ticking their children
    /// </summary>
    public bool GlobalTicking {
        get {
            return ParentValid ? Parent.GlobalTicking : ChildrenTicking;
        }
    }

    /// <summary>
    /// If the actor is currently in the game's node tree.
    /// </summary>
    public bool InTree => Game != null && ParentMatches(Game.Root);

    bool addedToTree = false;

    public Actor(LittleGame game) {
        Game = game;
        Parent = Invalid;

        Match = new(this);

        Children = new(this);
        Components = new(this);
    }

    /// <summary>
    /// Update all components and children. </br>
    /// Updates all children first, then updates all components. </br>
    /// Both are updated in the order they're stored in their container, from first to last.
    /// </summary>
    internal void Update() {
        if (ChildrenTicking) {
            foreach (Actor child in Children.Elements) {
                child.Update();
            }
        }

        if (ComponentsTicking) {
            foreach (Component component in Components.Elements) {
                if (component.Ticking) { component.OnUpdate(); }
            }
        }
    }

    //! FIXME (Alex): batcher maybe doesnt need to be passed? They already have access to game
    //      Actually, actor should store current batcher and other batcher effects to allow for components to control rendering of others
    /// <summary>
    /// Render all components and children. </br>
    /// Renders all components first, then renders all children. </br>
    /// Both are rendered in the order they're stored in their container, from first to last.
    /// </summary>
    internal void Render(Batcher batcher) {
        batcher.PushMatrix(Position.Rounded);

        if (ComponentsVisible) {
            foreach (Component component in Components.Elements) {
                if (component.Visible) { component.OnRender(batcher); }
            }
        }

        if (ChildrenVisible) {
            foreach (Actor child in Children.Elements) {
                child.Render(batcher);
            }
        }

        batcher.PopMatrix();
    }

    internal void ApplyChanges() {
        Children.ApplyChanges();
        foreach (Actor child in Children.Elements) {
            child.ApplyChanges();
        }
        Components.ApplyChanges();
    }

    internal void EnterTree() {
        if (!addedToTree) {
            EnterTreeFirst();
            addedToTree = true;
        }

        foreach (Component component in Components.Elements) {
            component.OnEnterTree();
        }

        foreach (Actor child in Children.Elements) {
            child.EnterTree();
        }
    }

    internal void EnterTreeFirst() { }

    internal void ExitTree() {
        foreach (Component component in Components.Elements) {
            component.OnExitTree();
        }

        foreach (Actor child in Children.Elements) {
            child.ExitTree();
        }
    }

    public void QueueInvalidate(bool invalidateChildren = true) {
        if (!IsValid) { return; }
        Game.QueueInvalidate(this, invalidateChildren);
    }

    internal void Invalidate(bool invalidateChildren = true, bool invalidateComponents = true) {
        if (!IsValid) { return; }

        if (invalidateChildren) {
            InvalidateChildren(invalidateComponents);
        }

        if (invalidateComponents) {
            foreach (Component component in Components.Elements) {
                component.OnInvalidated();
            }
        }

        Children.Clear();
        Components.Clear();

        if (ParentValid) {
            Parent.Children.Remove(this);
        }
    }

    void InvalidateChildren(bool invalidateComponents = true) {
        foreach (Actor child in Children.Elements) {
            child.Invalidate(false, invalidateComponents);
        }

        IsValid = false;
    }

    bool ParentMatches(Actor check) {
        if (this == check) { return true; }
        if (ParentValid) { return Parent.ParentMatches(check); }
        return false;
    }

    /// <summary>
    /// Convert a local coordinate to global space.
    /// This functionality is also available in <seealso cref="CoordSpace(in LittleGame)"> for consistency.
    /// </summary>
    /// <param name="position">The local coordinate.</param>
    /// <returns>The coordinate in global space.</returns>
    public Vector2 LocalToGlobal(Vector2 position) {
        return position + GlobalPosition;
    }

    /// <summary>
    /// Convert a global coordinate to local space.
    /// This functionality is also available in <seealso cref="CoordSpace(in LittleGame)"> for consistency.
    /// </summary>
    /// <param name="position">The global coordinate.</param>
    /// <returns>The coordinate in local space.</returns>
    public Vector2 GlobalToLocal(Vector2 position) {
        return position - GlobalPosition;
    }

    /// <summary>
    /// Convert a coordinate local to another actor to this actor's local space.
    /// This functionality is also available in <seealso cref="CoordSpace(in LittleGame)"> for consistency.
    /// </summary>
    /// <param name="position">The coordinate in the other actor's local space.</param>
    /// <returns>The coordinate local to this actor.</returns>
    public Vector2 FromOtherLocal(Actor originLocal, Vector2 position) {
        return GlobalToLocal(originLocal.LocalToGlobal(position));
    }

    /// <summary>
    /// Convert another actor's local position to this actor's local space.
    /// This functionality is also available in <seealso cref="CoordSpace(in LittleGame)"> for consistency.
    /// </summary>
    /// <returns>The other actor's position local to this actor.</returns>
    public Vector2 FromOtherLocal(Actor originLocal) {
        return GlobalToLocal(originLocal.GlobalPosition);
    }

    /// <summary>
    /// Create a new actor pre-assigned with the supplied children.
    /// </summary>
    /// <param name="game">The game instance</param>
    /// <param name="children">The children to add to the actor.</param>
    /// <returns>A new actor, with all of the given children added to it.</returns>
    public static Actor From(LittleGame game, params Actor[] children) => With(new(game), children);

    /// <summary>
    /// Assign all children to the given actor.
    /// </summary>
    /// <param name="actor">The actor,</param>
    /// <param name="children">The children to add to the actor.</param>
    /// <returns>The actor with all of the given children added to it.</returns>
    public static Actor With(Actor actor, params Actor[] children) {
        foreach (Actor child in children) {
            actor.Children.Add(child);
        }
        return actor;
    }

    /// <summary>
    /// Create a new actor pre-assigned with the supplied components.
    /// </summary>
    /// <param name="game">The game instance</param>
    /// <param name="components">The components to add to the actor.</param>
    /// <returns>A new actor, with all of the given components added to it.</returns>
    public static Actor From(LittleGame game, params Component[] components) => With(new(game), components);

    /// <summary>
    /// Assign all components to the given actor.
    /// </summary>
    /// <param name="actor">The actor,</param>
    /// <param name="components">The components to add to the actor.</param>
    /// <returns>The actor with all of the given components added to it.</returns>
    public static Actor With(Actor actor, params Component[] components) {
        foreach (Component component in components) {
            actor.Components.Add(component);
        }
        return actor;
    }

    public override string ToString() {
        return Match.ToString();
    }
}