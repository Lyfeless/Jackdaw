using Foster.Framework;

namespace LittleLib;

internal class ChildContainerModifyActionMove<Telement, Towner>(ChildContainer<Telement, Towner> container, Telement child, int amount) : ChildContainerModifyAction<Telement, Towner>(container) where Telement : class {
    readonly Telement Child = child;
    readonly int Amount = amount;

    public override void Execute() {
        if (!Container.Elements.Contains(Child)) {
            Log.Warning($"ChildContainer: Object {Child} is not child, can't move");
            return;
        }

        int startIndex = Container.Elements.IndexOf(Child);
        int endIndex = Math.Clamp(startIndex + Amount, 0, Container.Elements.Count - 1);
        if (startIndex == endIndex) { return; }
        Container.Elements.RemoveAt(startIndex);
        Container.Elements.Insert(endIndex, Child);
    }
}