using System.Numerics;
using Foster.Framework;

namespace LittleLib;

internal class FullWindowRenderer(LittleGame game) : LittleGameRenderer(game) {
    public override Vector2 ViewspaceToWindow(Vector2 position) => position;
    public override Vector2 WindowToViewspace(Vector2 position) => position;

    public override void Render(Batcher batcher, Actor root) {
        //! FIXME (Alex): Gross jank
        Game.Viewspace.Size = Game.Window.SizeInPixels();

        Game.Window.Clear(ClearColor);

        batcher.Clear();
        batcher.PushBlend(BlendMode.NonPremultiplied);
        batcher.PushMatrix(Game.Viewspace.RenderPosition);
        root?.Render(batcher);
        batcher.PopMatrix();

        batcher.Render(Game.Window);
    }
}