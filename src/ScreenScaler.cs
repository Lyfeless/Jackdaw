using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public static class ScreenScaler {
    public static Matrix3x2 DisplayScale { get; private set; }

    public static Vector2 GetScaledMousePosition() {
        Matrix3x2.Invert(DisplayScale, out Matrix3x2 inv);
        return Vector2.Transform(Input.Mouse.Position, inv);
    }

    public static void Render(Target target, Batcher batcher, Color? letterboxColor = null) {
        Graphics.Clear(letterboxColor ?? Color.Black);

        batcher.Clear();

        float scale = Calc.Min(App.WidthInPixels / (float)target.Width, App.HeightInPixels / (float)target.Height);
        DisplayScale = Transform.CreateMatrix(App.SizeInPixels / 2, new Vector2(target.Width, target.Height) / 2, Vector2.One * scale, 0);
        batcher.PushMatrix(DisplayScale);

        batcher.Image(target, Color.White);

        batcher.Render();
        batcher.Clear();
    }
}