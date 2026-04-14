using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Basic component for storing a size. <br/>
/// Not ticking or visible by default.
/// </summary>
public class BoundsComponent : Component {
    public Rect Rect;

    /// <param name="game">The current game instance.</param>
    /// <param name="bounds">The rectangle region the bounds cover.</param>
    public BoundsComponent(Game game, Rect bounds) : base(game) {
        Rect = bounds;
        Active = false;
    }

    public Vector2 Position {
        get => Rect.Position;
        set => Rect.Position = value;
    }

    public Vector2 Size {
        get => Rect.Size;
        set => Rect.Size = value;
    }
}