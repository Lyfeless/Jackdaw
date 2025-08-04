namespace LittleLib;

public class ActorContainer(Actor actor) : ChildContainer<Actor>() {
    Actor Actor = actor;

    public override bool Locked() => Actor.Game == null || Actor.Game.LockContainers;

    public override bool CanAdd(Actor child) {
        if (!Actor.IsValid) {
            Console.WriteLine($"ActorContainer: Cannot add object {child}, Actor is invalid");
            return false;
        }

        Actor compare = Actor;
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
        child.Parent = Actor;
        if (Actor.InTree) {
            child.EnterTree();
        }
    }

    public override void HandleRemove(Actor child) {
        child.Parent = Actor.Invalid;
        if (Actor.InTree) {
            child.ExitTree();
        }
    }
}