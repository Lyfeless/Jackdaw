using Foster.Framework;

namespace LittleLib;

//! FIXME (Alex): untested
//! FIXME (Alex): Doc comments

public class CachedRenderComponent : Component {
    readonly Component Component;
    Target RenderedComponent;

    Point2 position;

    public CachedRenderComponent(LittleGame game, Component component, RectInt bounds) : base(game) {
        Component = component;
        Cache(bounds);
    }

    void Cache(RectInt bounds) {
        RenderedComponent = new(Game.GraphicsDevice, bounds.Width, bounds.Height);
        position = bounds.Position;
        Batcher batcher = new(Game.GraphicsDevice);
        batcher.PushMatrix(-position);
        //! FIXME (Alex): Maybe not wise to be using internal utility functions here
        Component.OnRender(batcher);
        batcher.Render(RenderedComponent);
    }

    protected override void Render(Batcher batcher) {
        batcher.Image(RenderedComponent, position, Color.White);
    }
}