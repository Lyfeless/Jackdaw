using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class RenderActionLetterbox(GraphicsDevice device, BoundsComponent fillBounds, Point2 viewportSize, Color backgroundColor) : ActorRenderAction() {
    GraphicsDevice Device = device;
    BoundsComponent Bounds = fillBounds;

    Target Target = new(device, viewportSize.X, viewportSize.Y);
    Batcher Batcher = new(device);

    Color BackgroundColor = backgroundColor;

    Matrix3x2 DisplayScale;

    public override Matrix3x2 PositionOffset => DisplayScale;

    public override void PreRender(RenderActionContainer container) {
        Target.Clear(BackgroundColor);
        Batcher.Clear();
        Batcher.PushMatrix(-Bounds.Position);

        float scale = Calc.Min(Bounds.Size.X / Target.Width, Bounds.Size.Y / Target.Height);
        DisplayScale = Transform.CreateMatrix(Bounds.Position + ((Point2)Bounds.Size / 2), new Vector2(Target.Width, Target.Height) / 2, Vector2.One * scale, 0);

        container.PushBatcher(Batcher);
    }

    public override void PostRender(RenderActionContainer container) {
        container.PopBatcher();

        Batcher.Render(Target);
        container.CurrentBatcher.PushMatrix(DisplayScale);
        container.CurrentBatcher.Image(Target, Color.White);
    }
}