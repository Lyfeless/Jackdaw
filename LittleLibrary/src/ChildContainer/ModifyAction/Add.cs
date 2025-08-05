namespace LittleLib;

internal class ChildContainerModifyActionAdd<Telement, Towner>(ChildContainer<Telement, Towner> container, Telement child) : ChildContainerModifyAction<Telement, Towner>(container) {
    readonly Telement Child = child;

    public override void Execute() {
        if (!Container.CanAdd(Child) || Container.Elements.Contains(Child)) {
            Console.WriteLine($"ActorContainer: Failed to add child {Container.Printable(Child)}");
            return;
        }
        Container.HandleAdd(Child);
        Container.Elements.Add(Child);
    }
}