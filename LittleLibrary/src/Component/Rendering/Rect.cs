using Foster.Framework;

namespace LittleLib;

//! FIXME (Alex): Should this be a sprite?

public class RectComponent(LittleGame game, Rect rect, Color color) : Component(game) {
    public Rect Rect = rect;
    public Color Color = color;

    public override void Render(Batcher batcher) {
        //! FIXME (Alex): Verify culling is correct
        if (!Game.Viewspace.Bounds.Overlaps(new Rect(Actor.GlobalPosition + Rect.Position, Rect.Size))) { return; }
        batcher.Rect(Rect, Color);
    }
}