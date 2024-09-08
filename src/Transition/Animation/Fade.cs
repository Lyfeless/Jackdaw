using Foster.Framework;

namespace LittleLib;

public class FadeTransition(Color color, float startOpacity, float endOpacity, float duration) : TransitionAnimation(duration) {
    Color Color = color;
    float StartOpacity = startOpacity;
    float EndOpacity = endOpacity;

    public override void Render(Batcher batcher) {
        batcher.Rect(new(Camera.Viewport.X, Camera.Viewport.Y), new(Color.R, Color.G, Color.B, ((EndOpacity - StartOpacity) * Timer.Percent) + StartOpacity));
    }
}