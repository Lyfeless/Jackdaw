using System.Numerics;
using Foster.Framework;

namespace LittleLib;

internal class FixedViewportRenderer(LittleGame game, Point2 size) : LittleGameRenderer(game) {
    readonly Target Target = new(game.GraphicsDevice, size.X, size.Y);

    public Matrix3x2 DisplayScale { get; private set; }

    public Color ViewportColor = Color.CornflowerBlue;

    //! FIXME (Alex): I have no idea if either of these work
    public override Vector2 WindowToViewspace(Vector2 position) {
        Matrix3x2.Invert(DisplayScale, out Matrix3x2 inv);
        return Vector2.Transform(position, inv);
    }

    public override Vector2 ViewspaceToWindow(Vector2 position) {
        //! FIXME (Alex): UNTESTED
        return Vector2.Transform(position, DisplayScale);

    }

    //! FIXME (Alex): There's redundancy here between different renderers, is that important?
    public override void Render(Batcher batcher, Actor root) {
        Game.Window.Clear(ClearColor);

        batcher.Clear();
        batcher.PushMatrix(Game.Viewspace.RenderPosition);
        root?.Render(batcher);
        batcher.PopMatrix();

        Target.Clear(ViewportColor);
        batcher.Render(Target);
        batcher.Clear();

        float scale = Calc.Min(Game.Window.WidthInPixels / (float)Target.Width, Game.Window.HeightInPixels / (float)Target.Height);
        DisplayScale = Transform.CreateMatrix(Game.Window.SizeInPixels / 2, new Vector2(Target.Width, Target.Height) / 2, Vector2.One * scale, 0);
        batcher.PushMatrix(DisplayScale);

        batcher.Image(Target, Color.White);

        batcher.Render(Game.Window);
    }
}