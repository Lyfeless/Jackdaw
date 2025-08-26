using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class RenderActionClipRect(GraphicsDevice device, BoundsComponent bounds) : ActorRenderAction() {
    GraphicsDevice Device = device;
    BoundsComponent Bounds = bounds;

    Target Target = new(device, (int)bounds.Size.X, (int)bounds.Size.Y);
    Batcher Batcher = new(device);

    public override void PreRender(RenderActionContainer container, Batcher batcher) {
        if (Bounds.Size != Target.SizeInPixels()) {
            Target = new(Device, (int)Bounds.Size.X, (int)Bounds.Size.Y);
        }

        Target.Clear(Color.Transparent);
        Batcher.Clear();
        Batcher.PushMatrix(-Bounds.Position);
    }

    public override void PreRenderPhase(RenderActionContainer container, Batcher batcher) {
        container.PushBatcher(Batcher);
    }

    public override void PostRenderPhase(RenderActionContainer container, Batcher batcher) {
        container.PopBatcher();
    }

    public override void PostRender(RenderActionContainer container, Batcher batcher) {
        Batcher.Render(Target);
        batcher.Image(Target, Bounds.Position, Color.White);
    }
}