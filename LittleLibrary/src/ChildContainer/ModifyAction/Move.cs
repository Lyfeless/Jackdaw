namespace LittleLib;

internal class ChildContainerModifyActionMove<T>(ChildContainer<T> container, T child, int amount) : ChildContainerModifyAction<T>(container) {
    readonly T Child = child;
    readonly int Amount = amount;

    public override void Execute() {
        if (!Container.Elements.Contains(Child)) {
            Console.WriteLine($"ActorContainer: Object {Child} is not child, can't move");
            return;
        }

        int startIndex = Container.Elements.IndexOf(Child);
        int endIndex = Math.Clamp(startIndex + Amount, 0, Container.Elements.Count - 1);
        if (startIndex == endIndex) { return; }
        Container.Elements.RemoveAt(startIndex);
        Container.Elements.Insert(endIndex, Child);
    }
}