using System.Numerics;
using Foster.Framework;

namespace LittleLib;

internal abstract class LittleGameRenderer(LittleGame game) {
    protected readonly LittleGame Game = game;
    public Color ClearColor = Color.Black;

    public Vector2 GetScaledMousePosition() => ViewspaceToScreenSpace(Game.Input.Mouse.Position);
    public abstract Vector2 ViewspaceToScreenSpace(Vector2 position);
    public abstract void Render(Batcher batcher, Actor root);
}