using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// An animated sprite, begins animated when created.
/// </summary>
/// <param name="game">The current game instance.</param>
/// <param name="animation">The sprite animation to use.</param>
public class SpriteAnimated(Game game, AnimationData animation) : Sprite {
    readonly AnimationData Animation = animation;

    /// <summary>
    /// The timer controlling the animation playback.
    /// </summary>
    public readonly Timer Timer = new(
        game: game,
        duration: animation.Duration,
        startTime: -animation.StartDelay,
        looping: animation.Looping
    );

    readonly RectInt bounds = (RectInt)new BoundsBuilder([.. animation.Frames.Select(e => new Rect(animation.PositionOffset + e.PositionOffset, e.Texture.Size))]).Rect;
    public override Point2 Size => bounds.Size;
    public override RectInt Bounds => bounds.Translate(Offset);

    public bool Done => Timer.Done;

    /// <summary>
    /// The current animation frame.
    /// </summary>
    public AnimationFrame Frame => Animation.GetFrame((float)Timer.ElapsedTimeClamped);

    /// <summary>
    /// An animated sprite, begins animated when created.
    /// </summary>
    /// <param name="game">The current game instance.</param>
    /// <param name="animation">The animation name.</param>
    public SpriteAnimated(Game game, string animation) : this(game, game.Assets.GetAnimation(animation)) { }

    public override void Render(Batcher batcher) {
        AnimationFrame frame = Frame;
        bool flipX = frame.FlipX != FlipX;
        bool flipY = frame.FlipY != FlipY;
        batcher.Image(frame.Texture, Animation.PositionOffset + frame.PositionOffset + Offset + (bounds.Size / 2), bounds.Center - Animation.PositionOffset - frame.PositionOffset, FlipScale(flipX, flipY), 0, Color);
    }
}