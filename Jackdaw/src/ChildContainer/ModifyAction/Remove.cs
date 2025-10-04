using Foster.Framework;

namespace Jackdaw;

internal class ChildContainerModifyActionRemove<Telement, Towner>(ChildContainer<Telement, Towner> container, Telement child) : ChildContainerModifyAction<Telement, Towner>(container) where Telement : class {
    readonly Telement Child = child;

    public override void Execute() {
        if (!Container.Elements.Contains(Child)) {
            Log.Warning($"ChildContainer: Object {Child} is not child, can't remove");
            return;
        }

        Container.HandleRemove(Child);
        Container.Elements.Remove(Child);
    }
}