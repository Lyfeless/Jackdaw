using System.Numerics;
using Foster.Framework;

namespace LittleLib;

/// <summary>
/// Basic component for storing a size.
/// </summary>
/// <param name="game"></param>
/// <param name="bounds"></param>
public class BoundsComponent(LittleGame game, Rect bounds) : Component(game) {
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