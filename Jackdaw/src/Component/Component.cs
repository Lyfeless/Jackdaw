using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Logic component structure for controlling actor behavior.
/// </summary>
public abstract class Component {
    /// <summary>
    /// The current game instance.
    /// </summary>
    public Game Game { get; internal set; }

    /// <summary>
    /// The actor that currently owns this component.
    /// </summary>
    public Actor Actor { get; internal set; } = Actor.Invalid;

    /// <summary>
    /// If the component is a valid, usable instance.
    /// </summary>
    public bool IsValid { get; internal set; } = true;

    /// <summary>
    /// If the actor is a currently active, non-invalidated instance.
    /// </summary>
    public bool ActorValid => Actor.IsValid;

    /// <summary>
    /// If the component is currently attached to an actor inside the game node tree.
    /// </summary>
    public bool InTree => Actor.InTree;

    /// <summary>
    /// Component identifier, used when searching for specific components in the tree.
    /// </summary>
    public readonly ObjectIdentifier<Component> Match;

    /// <summary>
    /// Should the component tick while attached to an actor.
    /// </summary>
    public bool Ticking {
        get => ticking;
        set {
            if (ticking == value) { return; }
            ticking = value;
            OnTickingChanged();
        }
    }

    /// <summary>
    /// Should the component render while attached to an actor.
    /// </summary>
    public bool Visible {
        get => visible;
        set {
            if (visible == value) { return; }
            visible = value;
            OnVisibilityChanged();
        }
    }

    bool ticking = true;
    bool globalTicking = true;

    bool visible = true;
    bool globalVisible = true;

    bool addedToActor = false;
    bool addedToTree = false;

    public Component(Game game) {
        Game = game;
        Match = new(this);
    }

    /// <summary>
    /// Ticking function, called once per tree tick when attached to an actor in the node tree.
    /// Doesn't run if <see cref="Ticking"> is false.
    /// </summary>
    protected virtual void Update() { }

    /// <summary>
    /// Display function, called once per tree render when attached to an actor in the node tree.
    /// Doesn't run if <see cref="Visible"> is false.
    /// </summary>
    /// <param name="batcher"></param>
    protected virtual void Render(Batcher batcher) { }

    /// <summary>
    /// Runs the first time the component is added to an actor. Will not run if removed and re-added. <br/>
    /// All actions in this function can assume a valid owning actor, but can't guarantee the actor is part of the node tree yet.
    /// </summary>
    protected virtual void AddedFirst() { }

    /// <summary>
    /// Runs any time the component is added to an actor. <br/>
    /// All actions in this function can assume a valid owning actor, but can't guarantee the actor is part of the node tree yet.
    /// </summary>
    protected virtual void Added() { }

    /// <summary>
    /// Runs any time the component is removed to an actor. <br/>
    /// All actions in this function can assume a valid owning actor, but can't guarantee the actor is part of the node tree.
    /// </summary>
    protected virtual void Removed() { }

    /// <summary>
    /// Runs the first time the component becomes part of the node tree, either by adding the owner to the tree or being added to an actor already in the tree. <br/>
    /// All actions in this function can assume a valid owning actor within the node tree.
    /// </summary>
    protected virtual void EnterTreeFirst() { }

    /// <summary>
    /// Runs any time the component becomes part of the node tree, either by adding the owner to the tree or being added to an actor already in the tree. <br/>
    /// All actions in this function can assume a valid owning actor within the node tree.
    /// </summary>
    protected virtual void EnterTree() { }

    /// <summary>
    /// Runs any time the component exits the node tree, either by removing its owning actor from the tree or being removed from its owner. <br/>
    /// All actions in this function can assume a valid owning actor within the node tree.
    /// </summary>
    protected virtual void ExitTree() { }

    /// <summary>
    /// Runs when the component is invalidated, meaning it cannot be added to any actors. Useful for cleaning up any stored resources.
    /// </summary>
    protected virtual void Invalidated() { }

    /// <summary>
    /// Runs when the component's ticking state changes.
    /// </summary>
    protected virtual void TickingChanged() { }

    /// <summary>
    /// Runs when the component starts ticking. Not ran when the component is created, despite starting active.
    /// </summary>
    protected virtual void TickingOn() { }

    /// <summary>
    /// Runs when the component stops ticking.
    /// </summary>
    protected virtual void TickingOff() { }

    /// <summary>
    /// Runs when the component's visibility state changes.
    /// </summary>
    protected virtual void VisibilityChanged() { }

    /// <summary>
    /// Runs when the component becomes visible. Not ran when the component is created, despite starting visible.
    /// </summary>
    protected virtual void VisibilityOn() { }

    /// <summary>
    /// Runs when the component becomes not visible.
    /// </summary>
    protected virtual void VisibilityOff() { }

    public override string ToString() {
        return Match.ToString();
    }

    internal void OnUpdate() { if (Ticking) { Update(); } }
    internal void OnRender(Batcher batcher) { if (Visible) { Render(batcher); } }
    internal void OnRemoved() => Removed();
    internal void OnExitTree() => ExitTree();
    internal void OnInvalidated() {
        if (!IsValid) { return; }

        IsValid = false;

        Invalidated();

        Actor?.Components.Remove(this);
    }

    internal void OnAdded() {
        if (!addedToActor) {
            addedToActor = true;
            AddedFirst();
        }
        Added();
    }


    internal void OnEnterTree() {
        if (!addedToTree) {
            addedToTree = true;
            EnterTreeFirst();
        }
        EnterTree();
    }

    internal void OnTickingChanged() {
        bool newTickingState = Ticking && Actor.GlobalComponentsTicking;
        if (newTickingState == globalTicking) { return; }
        globalTicking = newTickingState;

        if (globalTicking) { TickingOn(); }
        else { TickingOff(); }

        TickingChanged();
    }

    internal void OnVisibilityChanged() {
        bool newVisibleState = Visible && Actor.GlobalComponentsVisible;
        if (newVisibleState == globalVisible) { return; }
        globalVisible = newVisibleState;

        if (globalVisible) { VisibilityOn(); }
        else { VisibilityOff(); }

        VisibilityChanged();
    }

    /// <summary>
    /// Mark the component for cleanup at the end of the tick.
    /// </summary>
    public void QueueInvalidate() => Game.QueueInvalidate(this);
}