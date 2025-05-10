using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class ScreenScaler {
    readonly LittleGame Game;
    Target Target;

    public ScreenScaler(LittleGame game, Point2 size) {
        Game = game;
        Target = new(Game.GraphicsDevice, size.X, size.Y);
    }

    public Matrix3x2 DisplayScale { get; private set; }

    public Vector2 GetScaledMousePosition() {
        Matrix3x2.Invert(DisplayScale, out Matrix3x2 inv);
        return Vector2.Transform(Game.Input.Mouse.Position, inv);
    }

    public void Render(Batcher batcher) {
        //! FIXME (Alex): Needs background and letterbox colors
        Target.Clear(Color.CornflowerBlue);
        batcher.Render(Target);
        batcher.Clear();

        float scale = Calc.Min(Game.Window.WidthInPixels / (float)Target.Width, Game.Window.HeightInPixels / (float)Target.Height);
        DisplayScale = Transform.CreateMatrix(Game.Window.SizeInPixels / 2, new Vector2(Target.Width, Target.Height) / 2, Vector2.One * scale, 0);
        batcher.PushMatrix(DisplayScale);

        batcher.Image(Target, Color.White);

        batcher.Render(Game.Window);
    }
}