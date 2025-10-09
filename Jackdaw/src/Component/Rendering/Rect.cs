using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A simple rectangle renderer.
/// </summary>
/// <param name="game">The game instance.</param>
/// <param name="rect">The rectangle to render.</param>
/// <param name="color">The color to render the rectangle with.</param>
public class RectComponent(Game game, Rect rect, Color color) : Component(game) {
    /// <summary>
    /// The rendering rectangle.
    /// </summary>
    public Rect Rect = rect;

    /// <summary>
    /// The rectangle color.
    /// </summary>
    public Color Color = color;

    protected override void Render(Batcher batcher) {
        if (!Game.Window.BoundsInPixels().Overlaps(CalcExtra.TransformRect(Rect, Actor.Position.GlobalDisplayMatrix))) { return; }
        batcher.Rect(Rect, Color);
    }
}