using Foster.Framework;

namespace LittleLib;

public static class SubtextureExtensions {
    public static Subtexture Clip(this Subtexture texture, int x, int y, int width, int height) =>
        new(texture.Texture, new(texture.Source.X + x, texture.Source.Y + y, width, height));

    public static Subtexture Clip(this Subtexture texture, RectInt clip) => texture.Clip(clip.X, clip.Y, clip.Width, clip.Height);
}