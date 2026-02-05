using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// An object that can be rendered to the screen. <br/>
/// Use a <see cref="DisplayObjectRenderComponent" /> to render.
/// </summary>
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

    /// <summary>
    /// Check if the object is currently inside the window's view bounds.
    /// </summary>
    /// <param name="game">The current game instance.</param>
    /// <param name="actor">The actor the object is rendered relative to.</param>
    /// <param name="offset">The renderer component's relative offset.</param>
    /// <returns>If the object is onscreen.</returns>
    public bool IsOnScreen(Game game, Actor actor, Point2 offset) {
        Rect globalBounds = GetGlobalRenderBounds(actor, offset);
        return game.Window.BoundsInPixels().Overlaps(globalBounds);
    }

    /// <summary>
    /// Get the global bounds of the object in window coordinates.
    /// </summary>
    /// <param name="actor">The actor the object is rendered relative to.</param>
    /// <param name="offset">The renderer component's relative offset.</param>
    /// <returns>The display object's bounds as an AABB.</returns>
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