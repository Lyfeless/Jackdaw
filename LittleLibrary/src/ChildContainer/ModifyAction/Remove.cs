namespace LittleLib;

internal class ChildContainerModifyActionRemove<Telement, Towner>(ChildContainer<Telement, Towner> container, Telement child) : ChildContainerModifyAction<Telement, Towner>(container) {
    readonly Telement Child = child;

    public override void Execute() {
        Container.HandleRemove(Child);
        Container.Elements.Remove(Child);
    }
}