using Foster.Framework;

namespace LittleLib;

internal class ChildContainerModifyActionAddTop<Telement, Towner>(ChildContainer<Telement, Towner> container, Telement child) : ChildContainerModifyActionAdd<Telement, Towner>(container, child) where Telement : class {
    public override void Execute() {
        if (!Container.CanAdd(Child) || Container.Elements.Contains(Child)) {
            Log.Warning($"ChildContainer: Failed to add child {Container.Printable(Child)}");
            return;
        }
        Container.Elements.Insert(0, Child);
        Container.HandleAdd(Child);
    }
}