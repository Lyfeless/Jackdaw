using Foster.Framework;

namespace LittleLib;

public readonly struct AnimationFrame {
    public readonly Subtexture Texture;
    public readonly float Duration = 1;
    public readonly Point2 PositionOffset = Point2.Zero;

    public AnimationFrame(Subtexture texture, AnimationConfigFrame frame) {
        Duration = frame.Duration;
        PositionOffset = new(frame.PositionOffsetX, frame.PositionOffsetY);
        bool clip = frame.ClipX != 0 && frame.ClipY != 0 && frame.ClipWidth != 0 && frame.ClipHeight != 0;
        Texture = clip ? Clip(texture, frame.ClipX, frame.ClipY, frame.ClipWidth, frame.ClipHeight) : texture;
    }

    public AnimationFrame(Subtexture texture, int x, int y, int width, int height, float duration) {
        Duration = duration;
        Texture = Clip(texture, x, y, width, height);
    }

    //! FIXME (Alex): Should probably be an extension or util function
    public static Subtexture Clip(Subtexture texture, int x, int y, int width, int height) =>
        new(texture.Texture, new(texture.Source.X + x, texture.Source.Y + y, width, height));
}