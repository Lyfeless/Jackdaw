using Foster.Framework;

namespace Jackdaw;

//! FIXME (Alex): untested

/// <summary>
/// Optimization component for pre-rendering a component to avoid rending it every frame.
/// </summary>
public class CachedRenderComponent : Component {
    readonly Component Component;
    Target RenderedComponent;

    Point2 position;

    /// <summary>
    /// Create and cache a component within a rendering bound.
    /// </summary>
    /// <param name="game">The current game instance.</param>
    /// <param name="component">The component to cache.</param>
    /// <param name="bounds">The bounds to render the component in, relative to the component. Anything outside the bounds will be clipped.</param>
    public CachedRenderComponent(Game game, Component component, RectInt bounds) : base(game) {
        Component = component;
        Cache(bounds);
    }

    /// <summary>
    /// Re-render the component.
    /// </summary>
    /// <param name="bounds">The bounds to render the component in, relative to the component. Anything outside the bounds will be clipped.</param>
    public void Cache(RectInt bounds) {
        RenderedComponent = new(Game.GraphicsDevice, bounds.Width, bounds.Height);
        position = bounds.Position;
        Batcher batcher = new(Game.GraphicsDevice);
        batcher.PushMatrix(-position);
        //! FIXME (Alex): Maybe not wise to be using internal utility functions here
        Component.OnRender(batcher);
        batcher.Render(RenderedComponent);
    }

    protected override void Render(Batcher batcher) {
        //! FIXME (Alex): Cull
        batcher.Image(RenderedComponent, position, Color.White);
    }
}