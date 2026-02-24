using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A component that renders a <see cref="Jackdaw.DisplayObject" />.
/// </summary>
/// <param name="game">The current game instance.</param>
/// <param name="displayObject">The object to render.</param>
public class DisplayObjectRenderComponent(Game game, DisplayObject displayObject) : Component(game) {
    /// <summary>
    /// The current rendering sprite.
    /// </summary>
    public DisplayObject DisplayObject = displayObject;

    /// <summary>
    /// The amount to offset the object being rendered.
    /// </summary>
    public Point2 Offset = Point2.Zero;

    protected override void Render(Batcher batcher) {
        if (!DisplayObject.IsOnScreen(Game, Actor, Offset)) { return; }
        batcher.PushMatrix(Offset);
        DisplayObject.Render(batcher);
        batcher.PopMatrix();
    }
}