using Foster.Framework;

namespace LittleLib;

internal class ChildContainerModifyActionAddRelative<Telement, Towner>(ChildContainer<Telement, Towner> container, Telement child, Telement relative, int offset) : ChildContainerModifyActionAdd<Telement, Towner>(container, child) where Telement : class {
    readonly Telement Relative = relative;
    readonly int Offset = offset;

    public override void Execute() {
        if (!Container.CanAdd(Child) || Container.Elements.Contains(Child)) {
            Log.Warning($"ChildContainer: Failed to add child {Container.Printable(Child)}");
            return;
        }
        if (!Container.Elements.Contains(Relative)) {
            Log.Warning($"ChildContainer: Relative object {Container.Printable(Relative)} is not a child, can't add {Child}");
            return;
        }
        int index = Container.Elements.IndexOf(Relative) + Offset;
        Container.Elements.Insert(index, Child);
        Container.HandleAdd(Child);
    }
}