using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A container for storing components on an actor.
/// </summary>
/// <param name="actor">The owning actor.</param>
public class ComponentContainer(Actor actor) : RecursiveSearchableChildContainer<Component, Actor>(actor) {
    public override bool Locked() => !Owner.IsValid || (Owner.InTree && (Owner.Game == null || Owner.Game.LockContainers));

    public override bool CanAdd(Component child) {
        if (!child.IsValid) {
            Log.Warning($"ComponentContainer: Cannot add object {child}, Component is invalid");
            return false;
        }

        if (!Owner.IsValid) {
            Log.Warning($"ComponentContainer: Cannot add object {child}, Actor is invalid");
            return false;
        }

        return child.Actor != null;
    }

    public override string Printable(Component child) {
        return child.Match.ToString();
    }

    public override void HandleAdd(Component child) {
        if (child.ActorValid) {
            child.Actor.Components.Remove(child);
        }

        child.Actor = Owner;

        child.OnAdded();

        if (Owner.InTree) {
            child.OnEnterTree();
        }
    }

    public override void HandleRemove(Component child) {
        child.OnRemoved();
        if (Owner.InTree) {
            child.OnExitTree();
        }
        child.Actor = Actor.Invalid;
    }

    protected override ObjectIdentifier<Component> Match(Component element) => element.Match;

    protected override int RecurseCount() {
        if (modifyActions.Count == 0) { return Owner.Children.Elements.Count; }
        int addCount = 0;
        foreach (ChildContainerModifyAction<Component, Actor> action in modifyActions) {
            if (action is ChildContainerModifyActionAdd<Component, Actor>) { addCount++; }
        }
        return Owner.Children.Elements.Count + addCount;
    }

    protected override RecursiveSearchableChildContainer<Component, Actor> RecurseItem(int index) {
        if (index >= Owner.Children.Elements.Count) {
            index -= Owner.Children.Elements.Count;
            foreach (ChildContainerModifyAction<Actor, Actor> action in Owner.Children.modifyActions) {
                if (action is ChildContainerModifyActionAdd<Actor, Actor> addAction) {
                    if (index == 0) { return addAction.Child.Components; }
                    index--;
                }
            }
        }

        return Owner.Children.Elements[index].Components;
    }
}