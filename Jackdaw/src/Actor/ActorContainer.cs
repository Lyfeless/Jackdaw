using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A container for storing child actors on an actor.
/// </summary>
/// <param name="actor">The owning actor.</param>
public class ActorContainer(Actor actor) : RecursiveSearchableChildContainer<Actor, Actor>(actor) {
    public override bool CanAdd(Actor child) {
        if (!Owner.IsValid) {
            Log.Warning($"ActorContainer: Cannot add object {child}, Actor is invalid");
            return false;
        }

        Actor compare = Owner;
        while (compare.Parent.IsValid) {
            if (compare.Parent == child) { return false; }
            compare = compare.Parent;
        }
        return true;
    }

    public override string Printable(Actor child) {
        return child.Match.ToString();
    }

    public override void HandleAdd(Actor child) => child.ParentAdded(Owner);

    public override void HandleRemove(Actor child) => child.ParentRemoved(Owner);

    protected override ObjectIdentifier<Actor> Match(Actor element) => element.Match;

    protected override int RecurseCount() {
        if (modifyActions.Count == 0) { return Owner.Children.Elements.Count; }
        int addCount = 0;
        foreach (ChildContainerModifyAction<Actor, Actor> action in modifyActions) {
            if (action is ChildContainerModifyActionAdd<Actor, Actor>) { addCount++; }
        }
        return Owner.Children.Elements.Count + addCount;
    }

    protected override RecursiveSearchableChildContainer<Actor, Actor> RecurseItem(int index) {
        if (index >= Owner.Children.Elements.Count) {
            index -= Owner.Children.Elements.Count;
            foreach (ChildContainerModifyAction<Actor, Actor> action in modifyActions) {
                if (action is ChildContainerModifyActionAdd<Actor, Actor> addAction) {
                    if (index == 0) { return addAction.Child.Children; }
                    index--;
                }
            }
        }

        return Owner.Children.Elements[index].Children;
    }
}