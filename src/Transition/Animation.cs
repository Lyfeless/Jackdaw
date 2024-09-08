using Foster.Framework;

namespace LittleLib;

public abstract class TransitionAnimation(float duration) {
    public readonly Timer Timer = new(duration);

    public void Start() {
        Timer.Restart();
    }

    public virtual void Update() { }
    public virtual void Render(Batcher batcher) { }
}