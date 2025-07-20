using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class SpriteAnimated(LittleGame game, AnimationData animation) : Sprite {
    readonly AnimationData Animation = animation;
    readonly Timer Timer = new(
            game: game,
            duration: animation.Duration,
            startDelay: animation.StartDelay,
            looping: animation.Looping
        );

    readonly RectInt bounds = (RectInt)new BoundsBuilder([.. animation.Frames.Select(e => new Rect(animation.PositionOffset + e.PositionOffset, e.Texture.Size))]).Rect;
    public override Point2 Size => bounds.Size;
    public override RectInt Bounds => bounds.Translate(Offset);

    public SpriteAnimated(LittleGame game, string animation) : this(game, game.Assets.GetAnimation(animation)) { }

    public override void Render(Batcher batcher) {
        AnimationFrame frame = Animation.GetFrame((float)Timer.ElapsedTimeClamped);
        batcher.Image(frame.Texture, frame.PositionOffset + Offset + (bounds.Size / 2), Bounds.Center, FlipScale(), 0, Color);
    }
}