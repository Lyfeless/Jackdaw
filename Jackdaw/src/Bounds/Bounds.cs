using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Basic component for storing a size.
/// </summary>
/// <param name="game">The current game instance.</param>
/// <param name="bounds">The rectangle region the bounds cover.</param>
public class BoundsComponent(Game game, Rect bounds) : Component(game) {
    public Rect Rect = bounds;

    public Vector2 Position {
        get => Rect.Position;
        set => Rect.Position = value;
    }

    public Vector2 Size {
        get => Rect.Size;
        set => Rect.Size = value;
    }
}