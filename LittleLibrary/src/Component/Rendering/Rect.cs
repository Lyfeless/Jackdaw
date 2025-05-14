using Foster.Framework;

namespace LittleLib;

public class RectComponent(LittleGame game, Rect rect, Color color) : Component(game) {
    Rect Rect = rect;
    Color Color = color;

    public override void Render(Batcher batcher) {
        if (!Game.Viewspace.Bounds.Contains(new Rect(Actor.GlobalPosition + Rect.Position, Rect.Size))) { return; }
        batcher.Rect(Rect, Color);
    }
}