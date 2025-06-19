using Foster.Framework;

namespace LittleLib;

public class AnimationData {
    readonly AnimationFrame[] Frames = [];

    public readonly float Duration;
    public readonly bool Looping;
    public readonly float StartDelay;
    public readonly Point2 PositionOffset;

    //! FIXME (Alex): This can possibly be optimized with some kind of lookup list/tree
    public AnimationFrame GetFrame(float time) {
        int index = 0;
        float count = 0;
        while (count < time && index < Frames.Length) {
            count += Frames[index].Duration;
            index++;
        }
        index--;
        return Frames[index];
    }

    public AnimationData(AnimationConfig data, Assets assets) {
        Subtexture[] textures = [.. data.Textures.Select(assets.GetTexture)];

        Looping = data.Looping;
        StartDelay = data.StartDelay;
        PositionOffset = new(data.PositionOffsetX, data.PositionOffsetY);

        if (data.HorizontalFrames > 0 && data.VerticalFrames >= 0) {
            //! FIXME (Alex): This will crash if textures is empty, do we care?
            Subtexture texture = textures[0];
            int width = (int)(texture.Width / data.HorizontalFrames);
            int height = (int)(texture.Height / data.VerticalFrames);
            Frames = new AnimationFrame[data.HorizontalFrames * data.VerticalFrames];
            for (int x = 0; x < data.HorizontalFrames; ++x) {
                for (int y = 0; y < data.VerticalFrames; ++y) {
                    Frames[(y * data.HorizontalFrames) + x] = new AnimationFrame(
                        texture: texture,
                        x: x * width,
                        y: y * height,
                        width: width,
                        height: height,
                        duration: data.FrameTime
                    );
                }
            }

            Duration = data.FrameTime * Frames.Length;
        }
        else {
            Frames = [.. data.Frames.Select(e => new AnimationFrame(textures[e.Texture], e))];
            Duration = Frames.Sum(e => e.Duration);
        }
    }

    // Only used for error fallback
    public AnimationData(Assets assets) {
        Frames = [new(assets.GetTexture("error"), new())];
    }
}