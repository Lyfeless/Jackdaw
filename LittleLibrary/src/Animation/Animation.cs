using Foster.Framework;

namespace LittleLib;

/// <summary>
/// Sprite-based animation data
/// </summary>
public class AnimationData {
    public readonly AnimationFrame[] Frames = [];
    public readonly float Duration;
    public readonly bool Looping;
    //! FIXME (Alex): Should start delay be here, or only in the actual component?
    public readonly float StartDelay;
    public readonly Point2 PositionOffset;

    /// <summary>
    /// Create an animation by dividing a single sprite sheet into a grid of frames
    /// </summary>
    /// <param name="assets">The asset container for the current game instance.</param>
    /// <param name="texture">The sprite sheet asset id.</param>
    /// <param name="horizontalFrames">The number of frames the fit horizontally across the spritesheet.</param>
    /// <param name="verticalFrames">The number of frames the fit vertically across the spritesheet.</param>
    /// <param name="frameTime">The time to spend on each frame in the animation, in milliseconds..</param>
    /// <param name="maxFrames">The total number of frames, useful if the spritesheet doesnt use the entire grid. Set to 0 to use every available space on the spritesheet</param>
    /// <param name="startDelay">The time to wait before starting the animation, in milliseconds.. Defaults to 0.</param>
    /// <param name="looping">If the animation should loop back to the start when finished.</param>
    /// <param name="positionOffset">The amount to offset the entire animation from the origin.</param>
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

    /// <summary>
    /// Create an animation by dividing a single sprite sheet into a grid of frames
    /// </summary>
    /// <param name="texture">The sprite sheet.</param>
    /// <param name="horizontalFrames">The number of frames the fit horizontally across the spritesheet.</param>
    /// <param name="verticalFrames">The number of frames the fit vertically across the spritesheet.</param>
    /// <param name="frameTime">The time to spend on each frame in the animation, in milliseconds..</param>
    /// <param name="maxFrames">The total number of frames, useful if the spritesheet doesnt use the entire grid. Set to 0 to use every available space on the spritesheet</param>
    /// <param name="startDelay">The time to wait before starting the animation, in milliseconds.. Defaults to 0.</param>
    /// <param name="looping">If the animation should loop back to the start when finished.</param>
    /// <param name="positionOffset">The amount to offset the entire animation from the origin.</param>
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
                Frames[index] = new(texture, frameTime, new(x * width, y * height, width, height));
            }
        }
    }

    /// <summary>
    /// Create an animation using predefined frame data.
    /// </summary>
    /// <param name="frames">Each individual frame to display in order.</param>
    /// <param name="startDelay">The time to wait before starting the animation, in milliseconds.. Defaults to 0.</param>
    /// <param name="looping">If the animation should loop back to the start when finished.</param>
    /// <param name="positionOffset">The amount to offset the entire animation's position from the origin.</param>
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
    /// <summary>
    /// Get a frame from the animation.
    /// </summary>
    /// <param name="time">The elapsed duration of the animation, in milliseconds.</param>
    /// <returns></returns>
    public AnimationFrame GetFrame(float time) {
        float count = 0;
        for (int i = 0; i < Frames.Length; ++i) {
            count += Frames[i].Duration;
            if (time <= count) { return Frames[i]; }
        }
        return Frames[^1];
    }

    // Only used for error fallback
    internal AnimationData(Assets assets) {
        Frames = [new(assets.GetTexture("error"), new())];
    }
}