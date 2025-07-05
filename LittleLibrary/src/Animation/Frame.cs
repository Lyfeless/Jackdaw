using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public readonly struct AnimationFrame {
    public readonly Subtexture Texture;
    public readonly float Duration = 1;
    public readonly Point2 PositionOffset = Point2.Zero;

    public AnimationFrame(
        Subtexture texture,
        float duration,
        Point2? positionOffset = null,
        RectInt? clip = null
    ) {
        Duration = duration;
        PositionOffset = positionOffset ?? Point2.Zero;
        if (clip != null) {
            RectInt clipRect = (RectInt)clip;
            Texture = Clip(texture, clipRect.X, clipRect.Y, clipRect.Width, clipRect.Height);
        }
        else {
            Texture = texture;
        }
    }

    public AnimationFrame(
        Subtexture texture,
        int x,
        int y,
        int width,
        int height,
        float duration
    ) {
        Duration = duration;
        Texture = Clip(texture, x, y, width, height);
    }

    //! FIXME (Alex): Should probably be an extension or util function
    public static Subtexture Clip(Subtexture texture, int x, int y, int width, int height) =>
        new(texture.Texture, new(texture.Source.X + x, texture.Source.Y + y, width, height));
}