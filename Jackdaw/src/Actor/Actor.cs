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
    public bool ComponentsVisible {
        get => componentsVisible;
        set => componentsVisible = value;
    }

    /// <summary>
    /// If the actor's componenents will render due to local and parent visibility states.
    /// </summary>
    public bool GlobalComponentsVisible => ComponentsVisible && ParentTicking;

    /// <summary>
    /// If the actor's children should render.
    /// </summary>
    public bool ChildrenVisible {
        get => childrenVisible;
        set => childrenVisible = value;
    }

    /// <summary>
    /// If the actor's children will render due to local and parent visibility states.
    /// </summary>
    public bool GlobalChildrenVisible => ChildrenVisible && ParentTicking;

    /// <summary>
    /// If the actor's components and children are rendering.
    /// Assigning changes the value of both <see cref="ComponentsVisible" /> and <see cref="ChildrenVisible" />
    /// </summary>
    public bool Visible {
        get => ComponentsVisible && ChildrenVisible;
        set {
            ComponentsVisible = value;
            ChildrenVisible = value;
        }
    }

    /// <summary>
    /// If the actor will render due to local and parent visibility states.
    /// </summary>
    public bool GlobalVisible => Visible && ParentVisible;

    /// <summary>
    /// If all parents above the actor are rendering their children
    /// </summary>
    public bool ParentVisible { get; internal set; } = true;

    /// <summary>
    /// If the actor's components should tick.
    /// </summary>
    public bool ComponentsTicking {
        get => componentsTicking;
        set {
            componentsTicking = value;
            ComponentTickingChanged();
        }
    }

    /// <summary>
    /// If the actor's componenents will tick due to local and parent visibility states.
    /// </summary>
    public bool GlobalComponentsTicking => ComponentsTicking && ParentTicking;

    /// <summary>
    /// If the actor's children should tick.
    /// </summary>
    public bool ChildrenTicking {
        get => childrenTicking;
        set {
            childrenTicking = value;
            ChildrenTickingChanged();
        }
    }

    /// <summary>
    /// If the actor's children will tick due to local and parent visibility states.
    /// </summary>
    public bool GlobalChildrenTicking => ChildrenTicking && ParentTicking;

    /// <summary>
    /// If the actor's components and children are ticking.
    /// Assigning changes the value of both <see cref="ComponentsTicking" /> and <see cref="ChildrenTicking" />
    /// </summary>
    public bool Ticking {
        get => ComponentsTicking && ChildrenTicking;
        set {
            ComponentsTicking = value;
            ChildrenTicking = value;
        }
    }

    /// <summary>
    /// If the actor will tick due to local and parent visibility states.
    /// </summary>
    public bool GlobalTicking => Ticking && ParentTicking;

    /// <summary>
    /// If all parents above the actor are ticking their children
    /// </summary>
    public bool ParentTicking { get; internal set; } = true;
    // public bool GlobalTicking => ChildrenTicking && (!ParentValid || Parent.GlobalTicking);

    /// <summary>
    /// If the actor is currently in the game's node tree.
    /// </summary>
    public bool InTree { get; internal set; } = false;

    bool componentsVisible = true;
    bool childrenVisible = true;

    bool componentsTicking = true;
    bool childrenTicking = true;

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
        if (ChildrenTicking) { Children.RunAll(e => e.Update()); }
        if (ComponentsTicking) { Components.RunAll(e => e.OnUpdate()); }
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

        if (ComponentsVisible) { Components.RunAll(e => e.OnRender(currentBatcher)); }
        if (ChildrenVisible) { Children.RunAll(e => e.Render(currentBatcher)); }

        RenderActions.PostRender();

        batcher.PopMatrix();
    }

    internal void ApplyChanges() {
        Children.ApplyChanges();
        Children.RunAll(e => e.ApplyChanges());
        Components.ApplyChanges();
    }

    internal void EnterTree() {
        InTree = true;

        if (!addedToTree) {
            EnterTreeFirst();
            addedToTree = true;
        }

        Components.RunAll(e => e.OnEnterTree());
        Children.RunAll(e => e.EnterTree());
    }

    internal void EnterTreeFirst() { }

    internal void ExitTree() {
        InTree = false;

        Components.RunAll(e => e.OnExitTree());
        Children.RunAll(e => e.ExitTree());
    }

    internal void ParentAdded(Actor parent) {
        if (ParentValid) { Parent.Children.Remove(this); }

        Parent = parent;
        if (Parent.InTree) { EnterTree(); }

        ParentTickingChanged();
        ParentVisibilityChanged();

        Position.MakeDirty();
    }

    internal void ParentRemoved() {
        if (Parent.InTree) { ExitTree(); }

        Parent = Invalid;

        ParentTickingChanged();
        ParentVisibilityChanged();

        Position.MakeDirty();
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

        if (invalidateChildren) { Children.RunAll(e => e.Invalidate(invalidateChildren, invalidateComponents)); }
        else { Children.Clear(); }

        if (invalidateComponents) { Components.RunAll(e => e.OnInvalidated()); }
        else { Components.Clear(); }

        Children.ApplyChanges();
        Components.ApplyChanges();

        if (ParentValid) {
            Parent.Children.Remove(this);
        }
    }

    internal void ParentTickingChanged() {
        ParentTicking = Parent.GlobalChildrenTicking;
        ChildrenTickingChanged();
        ComponentTickingChanged();
    }

    internal void ChildrenTickingChanged() => Children.RunAll(e => e.ParentTickingChanged());

    internal void ComponentTickingChanged() => Components.RunAll(e => e.OnTickingChanged());

    internal void ParentVisibilityChanged() {
        ParentVisible = Parent.GlobalChildrenVisible;
        ChildrenVisibilityChanged();
        ComponentVisibilityChanged();
    }

    internal void ChildrenVisibilityChanged() => Children.RunAll(e => e.ParentVisibilityChanged());

    internal void ComponentVisibilityChanged() => Components.RunAll(e => e.OnVisibilityChanged());

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

    /// <summary>
    /// Create a new actor from a single child.
    /// </summary>
    /// <param name="child">The child to add to the actor.</param>
    /// <returns></returns>
    public static Actor From(Actor child) {
        Actor actor = new(child.Game);
        actor.Children.Add(child);
        return actor;
    }

    /// <summary>
    /// Create a new actor from a single component.
    /// </summary>
    /// <param name="component">The component to add to the actor.</param>
    /// <returns></returns>
    public static Actor From(Component component) {
        Actor actor = new(component.Game);
        actor.Components.Add(component);
        return actor;
    }

    public override string ToString() {
        return Match.ToString();
    }
}