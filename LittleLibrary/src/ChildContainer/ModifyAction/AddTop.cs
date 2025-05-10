namespace LittleLib;

internal class ChildContainerModifyActionAddTop<T>(ChildContainer<T> container, T child) : ChildContainerModifyAction<T>(container) {
    readonly T Child = child;

    public override void Execute() {
        if (!Container.CanAdd(Child) || Container.Elements.Contains(Child)) {
            Console.WriteLine($"ActorContainer: Failed to add child {Container.Printable(Child)}");
            return;
        }
        Container.HandleAdd(Child);
        Container.Elements.Insert(0, Child);
    }
}