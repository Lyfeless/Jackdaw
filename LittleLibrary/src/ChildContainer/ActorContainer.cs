namespace LittleLib;

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
        child.Parent = Actor.Invalid;
        if (Owner.InTree) {
            child.ExitTree();
        }
    }
}