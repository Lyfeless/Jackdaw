using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class SpriteAnimated(LittleGame game, AnimationData animation) : Sprite {
    readonly AnimationData Animation = animation;
    public readonly Timer Timer = new(
        game: game,
        duration: animation.Duration,
        startDelay: animation.StartDelay,
        looping: animation.Looping
    );

    readonly RectInt bounds = (RectInt)new BoundsBuilder([.. animation.Frames.Select(e => new Rect(animation.PositionOffset + e.PositionOffset, e.Texture.Size))]).Rect;
    public override Point2 Size => bounds.Size;
    public override RectInt Bounds => bounds.Translate(Offset);

    public bool Done => Timer.Done;

    public AnimationFrame Frame => Animation.GetFrame((float)Timer.ElapsedTimeClamped);

    public SpriteAnimated(LittleGame game, string animation) : this(game, game.Assets.GetAnimation(animation)) { }

    public override void Render(Batcher batcher) {
        AnimationFrame frame = Frame;
        //! FIXME (Alex): Surely there's a more elegant way to do this
        //! FIXME (Alex): Also untested
        bool flipX = frame.FlipX ? !FlipX : FlipX;
        bool flipY = frame.FlipY ? !FlipY : FlipY;
        batcher.Image(frame.Texture, frame.PositionOffset + Offset + (bounds.Size / 2), bounds.Center, FlipScale(flipX, flipY), 0, Color);
    }
}