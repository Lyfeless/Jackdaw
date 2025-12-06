using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Optimization component for pre-rendering objects to avoid rending it every frame.
/// </summary>
public class CachedRenderComponent : Component {
    readonly ICacheableObject CachedObject;
    Target RenderedComponent;

    Point2 position;

    bool uncached = true;
    RectInt bounds;

    /// <summary>
    /// The bounds to render the object in, relative to the component. Anything outside the bounds will be clipped.
    /// </summary>
    public RectInt Bounds {
        get => bounds;
        set {
            bounds = value;
            uncached = true;
        }
    }

    /// <summary>
    /// Create and cache a component within a rendering bound.
    /// </summary>
    /// <param name="game">The current game instance.</param>
    /// <param name="component">The component to cache.</param>
    /// <param name="bounds">The bounds to render the component in, relative to the component. Anything outside the bounds will be clipped.</param>
    public CachedRenderComponent(Game game, Component component, RectInt bounds) : base(game) {
        CachedObject = new CachableComponent(component);
        this.bounds = bounds;
    }

    /// <summary>
    /// Create and cache an actor within a rendering bound.
    /// </summary>
    /// <param name="game">The current game instance.</param>
    /// <param name="actor">The actor to cache.</param>
    /// <param name="bounds">The bounds to render the component in, relative to the component. Anything outside the bounds will be clipped.</param>
    public CachedRenderComponent(Game game, Actor actor, RectInt bounds) : base(game) {
        CachedObject = new CachableActor(actor);
        this.bounds = bounds;
    }

    protected override void Update() {
        if (!uncached) { return; }

        uncached = false;

        RenderedComponent = new(Game.GraphicsDevice, bounds.Width, bounds.Height);
        Batcher batcher = new(Game.GraphicsDevice);
        batcher.PushMatrix(-bounds.Position);
        CachedObject.Render(batcher);
        batcher.Render(RenderedComponent);
    }

    /// <summary>
    /// Manually re-render the component.
    /// </summary>
    public void Cache() {
        uncached = true;
    }

    protected override void Render(Batcher batcher) {
        if (!Game.Window.BoundsInPixels().Overlaps(CalcExtra.TransformRect(new Rect(position, RenderedComponent.SizeInPixels()), Actor.Transform.GlobalDisplayMatrix))) { return; }
        batcher.Image(RenderedComponent, position, Color.White);
    }
}

internal interface ICacheableObject {
    public void Render(Batcher batcher);
}

internal class CachableComponent(Component component) : ICacheableObject {
    readonly Component component = component;
    public void Render(Batcher batcher) => component.OnRender(batcher);
}

internal class CachableActor(Actor actor) : ICacheableObject {
    readonly Actor actor = actor;
    public void Render(Batcher batcher) => actor.Render(batcher);
}