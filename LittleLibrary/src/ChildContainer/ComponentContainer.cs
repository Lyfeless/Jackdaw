namespace LittleLib;

/// <summary>
/// A container for storing components on an actor.
/// </summary>
/// <param name="actor">The owning actor.</param>
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

        if (!child.AddedToActor) {
            child.AddedFirst();
            child.AddedToActor = true;
        }
        child.Added();

        if (Owner.InTree) {
            if (Owner.Parent.IsValid && !child.AddedToTree) {
                child.EnterTreeFirst();
                child.AddedToTree = true;
            }
            child.EnterTree();
        }
    }

    public override void HandleRemove(Component child) {
        child.Removed();
        if (Owner.InTree) {
            child.ExitTree();
        }
        child.Actor = Actor.Invalid;
    }

    protected override ObjectIdentifier<Component> Match(Component element) {
        return element.Match;
    }

    protected override int RecurseCount() {
        return Owner.Children.Elements.Count;
    }

    protected override ChildContainer<Component, Actor> RecurseItem(int index) {
        return Owner.Children.Elements[index].Components;
    }
}