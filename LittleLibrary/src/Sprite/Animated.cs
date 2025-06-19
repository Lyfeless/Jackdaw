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

    public override void Render(Batcher batcher) {
        AnimationFrame frame = Animation.GetFrame((float)Timer.ElapsedTimeClamped);
        batcher.Image(frame.Texture, frame.PositionOffset, Color.White);
    }
}