namespace LittleLib;

public class ComponentContainer(Actor actor) : ChildContainer<Component>() {
    readonly Actor Actor = actor;

    public override bool CanAdd(Component child) {
        if (!Actor.IsValid) {
            Console.WriteLine($"ActorContainer: Cannot add object {child}, Actor is invalid");
            return false;
        }

        return child.Actor != null;
    }

    public override string Printable(Component child) {
        return child.Match.ToString();
    }

    public override void HandleAdd(Component child) {
        child.Actor = Actor;
        child.Added();
        if (!child.AddedToActor) {
            child.AddedFirst();
            child.AddedToActor = true;
        }
        if (Actor.InTree) {
            child.EnterTree();
            //! FIXME (Alex): Redundant with actor check, does that matter?
            //      I already don't remember what this means
            if (Actor.Parent.IsValid && !child.AddedToTree) {
                child.EnterTreeFirst();
                child.AddedToTree = true;
            }
        }
    }

    public override void HandleRemove(Component child) {
        child.Actor = Actor.Invalid;
        child.Removed();
        if (Actor.InTree) {
            child.ExitTree();
        }
    }
}