using Foster.Framework;

namespace LittleLib;

public class AnimationData {
    public readonly AnimationFrame[] Frames = [];
    public readonly float Duration;
    public readonly bool Looping;
    public readonly float StartDelay;
    public readonly Point2 PositionOffset;

    public AnimationData(
        Assets assets,
        string texture,
        int horizontalFrames,
        int verticalFrames,
        float frameTime,
        int maxFrames = 0,
        float startDelay = 0,
        bool looping = true,
        Point2? positionOffset = null
    ) : this(assets.GetTexture(texture), horizontalFrames, verticalFrames, frameTime, maxFrames, startDelay, looping, positionOffset) { }

    public AnimationData(
        Subtexture texture,
        int horizontalFrames,
        int verticalFrames,
        float frameTime,
        int maxFrames = 0,
        float startDelay = 0,
        bool looping = true,
        Point2? positionOffset = null
    ) : this(startDelay, looping, positionOffset) {
        int frameCount = horizontalFrames * verticalFrames;
        if (maxFrames != 0) { frameCount = Math.Min(frameCount, maxFrames); }
        Frames = new AnimationFrame[frameCount];
        int width = (int)(texture.Width / horizontalFrames);
        int height = (int)(texture.Height / verticalFrames);
        Duration = frameCount * frameTime;
        for (int x = 0; x < horizontalFrames; ++x) {
            for (int y = 0; y < verticalFrames; ++y) {
                int index = (y * horizontalFrames) + x;
                if (index >= frameCount) { return; }
                Frames[index] = new(texture, x * width, y * height, width, height, frameTime);
            }
        }
    }

    public AnimationData(
        AnimationFrame[] frames,
        float startDelay = 0,
        bool looping = false,
        Point2? positionOffset = null
    ) : this(startDelay, looping, positionOffset) {
        Frames = frames;
        Duration = frames.Sum(e => e.Duration);
    }

    AnimationData(float startDelay = 0, bool looping = false, Point2? positionOffset = null) {
        Looping = looping;
        StartDelay = startDelay;
        PositionOffset = positionOffset ?? Point2.Zero;
    }

    //! FIXME (Alex): This can possibly be optimized with some kind of lookup list/tree
    public AnimationFrame GetFrame(float time) {
        float count = 0;
        for (int i = 0; i < Frames.Length; ++i) {
            count += Frames[i].Duration;
            if (time <= count) { return Frames[i]; }
        }
        return Frames[^1];
    }

    // Only used for error fallback
    public AnimationData(Assets assets) {
        Frames = [new(assets.GetTexture("error"), new())];
    }
}