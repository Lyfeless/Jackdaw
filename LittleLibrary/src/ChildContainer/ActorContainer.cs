namespace LittleLib;

/// <summary>
/// A container for storing child actors on an actor.
/// </summary>
/// <param name="actor">The owning actor.</param>
public class ActorContainer(Actor actor) : ChildContainer<Actor, Actor>(actor) {
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
        child.Parent = Owner;
        if (Owner.InTree) {
            child.EnterTree();
        }
    }

    public override void HandleRemove(Actor child) {
        if (Owner.InTree) {
            child.ExitTree();
        }

        child.Parent = Actor.Invalid;
    }

    protected override ObjectIdentifier<Actor> Match(Actor element) {
        return element.Match;
    }

    protected override int RecurseCount() {
        return Owner.Children.Elements.Count;
    }

    protected override ChildContainer<Actor, Actor> RecurseItem(int index) {
        return Owner.Children.Elements[index].Children;
    }
}