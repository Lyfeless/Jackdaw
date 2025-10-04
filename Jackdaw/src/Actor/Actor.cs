using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// The primary game object element, responsible for storing, updating, and rending all of its children and components.
/// </summary>
public class Actor {
    /// <summary>
    /// Create a new actor with no children or components.
    /// </summary>
    /// <param name="game">The game instance.</param>
    /// <returns>A new empty actor</returns>
    public static Actor Empty(Game game) => new(game);

    /// <summary>
    /// An invalid actor. Any actors with this as a parent are not part of the scene tree.
    /// </summary>
    public readonly static Actor Invalid = new(null) { IsValid = false, Ticking = false, Visible = false };

    /// <summary>
    /// The active game instance.
    /// </summary>
    public Game Game;

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
    /// All custom rendering actions to run on the actor.
    /// </summary>
    public RenderActionContainer RenderActions;

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
    public ActorPosition Position;

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

    public Actor(Game game) {
        Game = game;
        Parent = Invalid;

        Position = new(this);
        Match = new(this);
        Children = new(this);
        Components = new(this);
        RenderActions = new(this);
    }

    /// <summary>
    /// Update all components and children. <br/>
    /// Updates all children first, then updates all components. <br/>
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

    /// <summary>
    /// Render all components and children. <br/>
    /// Renders all components first, then renders all children. <br/>
    /// Both are rendered in the order they're stored in their container, from first to last.
    /// </summary>
    internal void Render(Batcher batcher) {
        if (!Visible) { return; }

        Position.CacheDisplay();

        batcher.PushMatrix(Position.LocalMatrix);

        RenderActions.PreRender(batcher);
        Batcher currentBatcher = RenderActions.CurrentBatcher;

        if (ComponentsVisible) {
            foreach (Component component in Components.Elements) {
                if (component.Visible) { component.OnRender(currentBatcher); }
            }
        }

        if (ChildrenVisible) {
            foreach (Actor child in Children.Elements) {
                child.Render(currentBatcher);
            }
        }

        RenderActions.PostRender();

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

    /// <summary>
    /// Mark the actor for cleanup at the end of the tick.
    /// Invalidated actors are removed from scene trees and removed all attached children and components.
    /// </summary>
    /// <param name="invalidateChildren">If all the actor's children should be recursively invalidated as well.</param>
    /// <param name="invalidateComponents">If all the actor's components should be invalidated as well.</param>
    public void QueueInvalidate(bool invalidateChildren = true, bool invalidateComponents = true) {
        if (!IsValid) { return; }
        Game.QueueInvalidate(this, invalidateChildren, invalidateComponents);
    }

    internal void Invalidate(bool invalidateChildren = true, bool invalidateComponents = true) {
        if (!IsValid) { return; }

        IsValid = false;

        if (invalidateChildren) {
            foreach (Actor child in Children.Elements) {
                child.Invalidate(false, invalidateComponents);
            }
        }

        if (invalidateComponents) {
            foreach (Component component in Components.Elements) {
                component.OnInvalidated();
            }
        }

        Children.Clear();
        Components.Clear();

        Children.ApplyChanges();
        Components.ApplyChanges();

        if (ParentValid) {
            Parent.Children.Remove(this);
        }
    }


    bool ParentMatches(Actor check) {
        if (this == check) { return true; }
        if (ParentValid) { return Parent.ParentMatches(check); }
        return false;
    }

    /// <summary>
    /// Create a new actor pre-assigned with the supplied children.
    /// </summary>
    /// <param name="game">The game instance</param>
    /// <param name="children">The children to add to the actor.</param>
    /// <returns>A new actor, with all of the given children added to it.</returns>
    public static Actor From(Game game, params Actor[] children) => With(new(game), children);

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
    public static Actor From(Game game, params Component[] components) => With(new(game), components);

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