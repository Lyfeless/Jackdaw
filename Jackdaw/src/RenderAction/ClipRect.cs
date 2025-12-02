using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A render action that clips all child and component elements outside of a specified bound.
/// </summary>
/// <param name="device">The game window's graphics device.</param>
/// <param name="bounds">The bounds to clip all rendering into, relative to actor position.</param>
public class RenderActionClipRect(GraphicsDevice device, BoundsComponent bounds) : ActorRenderAction() {
    GraphicsDevice Device = device;
    BoundsComponent Bounds = bounds;

    Target Target = new(device, (int)bounds.Size.X, (int)bounds.Size.Y);
    Batcher Batcher = new(device);

    public override void PreRender(RenderActionContainer container) {
        if (Bounds.Size != Target.SizeInPixels()) {
            Target = new(Device, (int)Bounds.Size.X, (int)Bounds.Size.Y);
        }

        Target.Clear(Color.Transparent);
        Batcher.Clear();
        Batcher.PushMatrix(-Bounds.Position);

        container.PushBatcher(Batcher);
    }

    public override void PostRender(RenderActionContainer container) {
        Batcher.Render(Target);
        container.PopBatcher();
        container.CurrentBatcher.Image(Target, Bounds.Position, Color.White);
    }
}