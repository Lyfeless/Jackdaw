namespace LittleLib;

public class ComponentContainer(Actor actor) : ChildContainer<Component, Actor>(actor) {
    public override bool Locked() => Owner.Game == null || Owner.Game.LockContainers;

    public override bool CanAdd(Component child) {
        if (!Owner.IsValid) {
            Console.WriteLine($"ActorContainer: Cannot add object {child}, Actor is invalid");
            return false;
        }

        return child.Actor != null;
    }

    public override string Printable(Component child) {
        return child.Match.ToString();
    }

    public override void HandleAdd(Component child) {
        child.Actor = Owner;
        child.Added();
        if (!child.AddedToActor) {
            child.AddedFirst();
            child.AddedToActor = true;
        }
        if (Owner.InTree) {
            child.EnterTree();
            //! FIXME (Alex): Redundant with actor check, does that matter?
            //      I already don't remember what this means
            if (Owner.Parent.IsValid && !child.AddedToTree) {
                child.EnterTreeFirst();
                child.AddedToTree = true;
            }
        }
    }

    public override void HandleRemove(Component child) {
        child.Actor = Actor.Invalid;
        child.Removed();
        if (Owner.InTree) {
            child.ExitTree();
        }
    }
}