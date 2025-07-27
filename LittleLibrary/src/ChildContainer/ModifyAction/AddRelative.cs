namespace LittleLib;

internal class ChildContainerModifyActionAddRelative<T>(ChildContainer<T> container, T child, T relative, int offset) : ChildContainerModifyAction<T>(container) {
    readonly T Child = child;
    readonly T Relative = relative;
    readonly int Offset = offset;

    public override void Execute() {
        if (!Container.CanAdd(Child) || Container.Elements.Contains(Child)) {
            Console.WriteLine($"ActorContainer: Failed to add child {Container.Printable(Child)}");
            return;
        }
        if (!Container.Elements.Contains(Relative)) {
            Console.WriteLine($"ActorContainer: Relative object {Container.Printable(Relative)} is not a child, can't add {Child}");
            return;
        }
        int index = Container.Elements.IndexOf(Relative) + Offset;
        Container.HandleAdd(Child);
        Container.Elements.Insert(index, Child);
    }
}