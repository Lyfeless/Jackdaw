namespace LittleLib;

/// <summary>
/// A container for storing child actors on an actor.
/// </summary>
/// <param name="actor">The owning actor.</param>
public class ActorContainer(Actor actor) : SearchableChildContainer<Actor, Actor>(actor) {
    public override bool Locked() => Owner.Game == null || Owner.Game.LockContainers;

    public override bool CanAdd(Actor child) {
        if (!Owner.IsValid) {
            Console.WriteLine($"ActorContainer: Cannot add object {child}, Actor is invalid");
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

    public override void HandleAdd(Actor child) {
        if (child.ParentValid) {
            child.Parent.Children.Remove(child);
        }

        child.Parent = Owner;
        if (Owner.InTree) {
            child.EnterTree();
        }

        child.Parent.Position.Cache();
    }

    public override void HandleRemove(Actor child) {
        if (Owner.InTree) {
            child.ExitTree();
        }

        child.Parent = Actor.Invalid;

        child.Parent.Position.Cache();
    }

    protected override ObjectIdentifier<Actor> Match(Actor element) {
        return element.Match;
    }

    protected override int RecurseCount() {
        if (modifyActions.Count == 0) { return Owner.Children.Elements.Count; }
        int addCount = 0;
        foreach (ChildContainerModifyAction<Actor, Actor> action in modifyActions) {
            if (action is ChildContainerModifyActionAdd<Actor, Actor>) { addCount++; }
        }
        return Owner.Children.Elements.Count + addCount;
    }

    protected override SearchableChildContainer<Actor, Actor> RecurseItem(int index) {
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