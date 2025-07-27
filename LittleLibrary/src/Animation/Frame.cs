using Foster.Framework;

namespace LittleLib;

public readonly struct AnimationFrame {
    public readonly Subtexture Texture;
    public readonly float Duration = 1;
    public readonly Point2 PositionOffset = Point2.Zero;
    public readonly bool FlipX = false;
    public readonly bool FlipY = false;
    public readonly string EmbeddedData = string.Empty;

    public AnimationFrame(
        Subtexture texture,
        float duration,
        Point2? positionOffset = null,
        RectInt? clip = null,
        bool flipX = false,
        bool flipY = false,
        string? embeddedData = null
    ) {
        Duration = duration;
        PositionOffset = positionOffset ?? Point2.Zero;
        FlipX = flipX;
        FlipY = flipY;
        EmbeddedData = embeddedData ?? string.Empty;
        if (clip != null) {
            RectInt clipRect = (RectInt)clip;
            Texture = texture.Clip(clipRect);
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
        Texture = texture.Clip(x, y, width, height);
    }
}