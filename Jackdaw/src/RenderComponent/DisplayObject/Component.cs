using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A component that renders a <see cref="Jackdaw.DisplayObject" />. <br/>
/// Not ticking by default.
/// </summary>
public class DisplayObjectRenderComponent : Component {
    /// <summary>
    /// The current rendering sprite.
    /// </summary>
    public DisplayObject DisplayObject;

    /// <summary>
    /// The amount to offset the object being rendered.
    /// </summary>
    public Point2 Offset = Point2.Zero;

    /// <param name="game">The current game instance.</param>
    /// <param name="displayObject">The object to render.</param>
    public DisplayObjectRenderComponent(Game game, DisplayObject displayObject) : base(game) {
        DisplayObject = displayObject;
        Ticking = false;
    }

    protected override void Render(Batcher batcher) {
        if (!DisplayObject.IsOnScreen(Game, Actor, Offset)) { return; }
        batcher.PushMatrix(Offset);
        DisplayObject.Render(batcher);
        batcher.PopMatrix();
    }
}