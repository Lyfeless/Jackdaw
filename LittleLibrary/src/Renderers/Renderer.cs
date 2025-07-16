using System.Numerics;
using Foster.Framework;

namespace LittleLib;

internal abstract class LittleGameRenderer(LittleGame game) {
    protected readonly LittleGame Game = game;
    public Color ClearColor = Color.Black;

    public abstract Vector2 ViewspaceToWindow(Vector2 position);
    public abstract Vector2 WindowToViewspace(Vector2 position);
    public abstract void Render(Batcher batcher, Actor root);
}