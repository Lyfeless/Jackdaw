namespace LittleLib;

internal class ChildContainerModifyActionAddTop<Telement, Towner>(ChildContainer<Telement, Towner> container, Telement child) : ChildContainerModifyAction<Telement, Towner>(container) where Telement : class {
    readonly Telement Child = child;

    public override void Execute() {
        if (!Container.CanAdd(Child) || Container.Elements.Contains(Child)) {
            Console.WriteLine($"ActorContainer: Failed to add child {Container.Printable(Child)}");
            return;
        }
        Container.Elements.Insert(0, Child);
        Container.HandleAdd(Child);
    }
}