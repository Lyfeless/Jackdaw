using Foster.Framework;

namespace LittleLib;

public class RenderActionContainer(Actor owner) : ChildContainer<ActorRenderAction, Actor>(owner) {
    bool Active = false;

    readonly Stack<Batcher> batcherStack = [];
    public Batcher CurrentBatcher => batcherStack.Peek();

    internal void PreRender(Batcher batcher) {
        batcherStack.Clear();
        batcherStack.Push(batcher);

        Active = true;

        for (int i = 0; i < Elements.Count; ++i) {
            Elements[i].PreRender(this);
        }
    }

    internal void PostRender() {
        for (int i = Elements.Count - 1; i >= 0; --i) {
            Elements[i].PostRender(this);
        }

        Active = false;
    }

    public void PushBatcher(Batcher batcher) {
        batcherStack.Push(batcher);
    }

    public void PopBatcher() {
        if (batcherStack.Count > 1) {
            batcherStack.Pop();
        }
    }

    public override bool Locked() => Active;

    //! FIXME (Alex): Does this need anything more?
    public override bool CanAdd(ActorRenderAction child) => true;

    public override void HandleAdd(ActorRenderAction child) { }

    public override void HandleRemove(ActorRenderAction child) { }

    public override string Printable(ActorRenderAction child) {
        return child.GetType().ToString();
    }
}