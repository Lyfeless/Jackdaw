using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class BoundsComponent(LittleGame game, Rect bounds) : Component(game) {
    public Rect Rect = bounds;
    public Vector2 Position => Rect.Position;
    public Vector2 Size => Rect.Size;

    public void SetPosition(Vector2 position) {
        Rect.Position = position;
    }

    public void SetPosition(float x, float y) => SetPosition(new(x, y));
    public void SetPositionX(float x) => SetPosition(new(x, Position.Y));
    public void SetPositionY(float y) => SetPosition(new(Position.X, y));

    public void SetSize(Vector2 size) {
        Rect.Size = size;
    }

    public void SetSize(float width, float height) => SetSize(new(width, height));
    public void SetWidth(float width) => SetSize(new(width, Size.Y));
    public void SetHeight(float height) => SetSize(new(Size.X, height));
}