using Foster.Framework;

namespace LittleLib;

public class RenderActionContainer(Actor owner) : ChildContainer<ActorRenderAction, Actor>(owner) {
    bool Active = false;

    readonly Stack<Batcher> batcherStack = [];
    public Batcher CurrentBatcher => batcherStack.Peek();

    internal void PreRender(Batcher batcher) {
        batcherStack.Clear();
        batcherStack.Push(batcher);

        for (int i = 0; i < Elements.Count; ++i) {
            if (!Elements[i].ApplyToComponents) { continue; }
            Elements[i].PreRender(this, CurrentBatcher);
        }
    }

    internal void PostRender() {
        for (int i = Elements.Count - 1; i >= 0; --i) {
            if (!Elements[i].ApplyToComponents) { continue; }
            Elements[i].PostRender(this, CurrentBatcher);
        }
    }

    internal void PreRenderComponents() {
        Active = true;

        while (batcherStack.Count > 2) {
            batcherStack.Pop();
        }

        for (int i = 0; i < Elements.Count; ++i) {
            if (!Elements[i].ApplyToComponents) { continue; }
            Elements[i].PreRenderPhase(this, CurrentBatcher);
        }
    }

    internal void PostRenderComponents() {
        for (int i = Elements.Count - 1; i >= 0; --i) {
            if (!Elements[i].ApplyToComponents) { continue; }
            Elements[i].PostRenderPhase(this, CurrentBatcher);
        }

        Active = false;
    }

    internal void PreRenderChildren() {
        Active = true;

        while (batcherStack.Count > 1) {
            batcherStack.Pop();
        }

        for (int i = 0; i < Elements.Count; ++i) {
            if (!Elements[i].ApplyToChildren) { continue; }
            Elements[i].PreRenderPhase(this, batcherStack.Peek());
        }
    }

    internal void PostRenderChildren() {
        for (int i = Elements.Count - 1; i >= 0; --i) {
            if (!Elements[i].ApplyToChildren) { continue; }
            Elements[i].PostRenderPhase(this, batcherStack.Peek());
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