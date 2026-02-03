using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

public abstract class DisplayObject {
    /// <summary>
    /// The bounds the object should use to determine if it should render
    /// </summary>
    public abstract RectInt Bounds { get; }

    /// <summary>
    /// The top-left corner of the object's bounds
    /// </summary>
    public Vector2 Position => Bounds.Position;

    /// <summary>
    /// The size of the object.
    /// </summary>
    public Vector2 Size => Bounds.Size;

    /// <summary>
    /// The color to render the object with. Defaults to white.
    /// </summary>
    public Color Color = Color.White;

    public bool IsOnScreen(Game game, Actor actor, Point2 offset) {
        Rect globalBounds = GetGlobalRenderBounds(actor, offset);
        return game.Window.BoundsInPixels().Overlaps(globalBounds);
    }

    public Rect GetGlobalRenderBounds(Actor actor, Point2 offset)
        => Bounds
            .Translate(offset)
            .TransformAABB(actor.Transform.GlobalDisplayMatrix);

    /// <summary>
    /// Render the object to the given batcher.
    /// </summary>
    /// <param name="batcher">The batcher to render to.</param>
    public abstract void Render(Batcher batcher);
}