using System.Numerics;
using Foster.Framework;

namespace LittleLib;

/// <summary>
/// A render action that scales a view into a bound while maintaining an aspect ratio.
/// </summary>
/// <param name="device">The game window's graphics device.</param>
/// <param name="fillBounds">The bounds that the letterboxed viewport should fill, relative to the actor position.</param>
/// <param name="viewportSize">The size of the viewport to be scaled.</param>
/// <param name="backgroundColor">The background color of the viewport contents.</param>
public class RenderActionLetterbox(GraphicsDevice device, BoundsComponent fillBounds, Point2 viewportSize, Color backgroundColor) : ActorRenderAction() {
    /// <summary>
    /// The bounds that the letterboxed viewport should fill, relative to the actor position.
    /// </summary>
    public BoundsComponent Bounds = fillBounds;

    /// <summary>
    /// The size of the viewport to be scaled.
    /// </summary>
    public readonly Point2 Size = new(viewportSize.X, viewportSize.Y);

    readonly Target Target = new(device, viewportSize.X, viewportSize.Y);
    readonly Batcher Batcher = new(device);

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