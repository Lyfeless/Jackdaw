using System.Numerics;
using Foster.Framework;

namespace LittleLib;

/// <summary>
/// A container for storing render actions on an actor.
/// </summary>
/// <param name="owner">The owning actor.</param>
public class RenderActionContainer(Actor owner) : SearchableChildContainer<ActorRenderAction, Actor>(owner) {
    bool Active = false;

    readonly Stack<Batcher> batcherStack = [];

    /// <summary>
    /// The most recent batcher passed.
    /// The last batcher pushed will be sent to all components and children.
    /// </summary>
    public Batcher CurrentBatcher => batcherStack.Peek();

    internal Matrix3x2 GetDisplayMatrix() {
        Matrix3x2 matrix = Matrix3x2.Identity;
        foreach (ActorRenderAction action in Elements) {
            matrix = action.PositionOffset * matrix;
        }
        return matrix;
    }

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

    /// <summary>
    /// Add a batcher to the stack.
    /// The last batcher pushed will be sent to all components and children.
    /// </summary>
    /// <param name="batcher"></param>
    public void PushBatcher(Batcher batcher) {
        batcherStack.Push(batcher);
    }

    /// <summary>
    /// Remove the most recent batcher from the stack.
    /// The last batcher pushed will be sent to all components and children.
    /// </summary>
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

    protected override ObjectIdentifier<ActorRenderAction> Match(ActorRenderAction element) => element.Match;
}