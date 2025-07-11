using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class SpriteAnimated(LittleGame game, AnimationData animation, Color color) : Sprite {
    readonly AnimationData Animation = animation;
    readonly Timer Timer = new(
            game: game,
            duration: animation.Duration,
            startDelay: animation.StartDelay,
            looping: animation.Looping
        );
    readonly Color Color = color;

    readonly Rect bounds = new BoundsBuilder([.. animation.Frames.Select(e => new Rect(animation.PositionOffset + e.PositionOffset, e.Texture.Size))]).Rect;
    public override Vector2 Size => bounds.Size;
    public override Rect Bounds => bounds;

    public SpriteAnimated(LittleGame game, AnimationData animation) : this(game, animation, Color.White) { }
    public SpriteAnimated(LittleGame game, string animation) : this(game, game.Assets.GetAnimation(animation)) { }
    public SpriteAnimated(LittleGame game, string animation, Color color) : this(game, game.Assets.GetAnimation(animation), color) { }

    public override void Render(Batcher batcher) {
        AnimationFrame frame = Animation.GetFrame((float)Timer.ElapsedTimeClamped);
        batcher.Image(frame.Texture, frame.PositionOffset, Color);
    }
}