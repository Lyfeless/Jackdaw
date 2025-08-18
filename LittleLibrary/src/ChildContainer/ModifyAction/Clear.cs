namespace LittleLib;

internal class ChildContainerModifyActionClear<Telement, Towner>(ChildContainer<Telement, Towner> container) : ChildContainerModifyAction<Telement, Towner>(container) where Telement : class {
    public override void Execute() {
        Container.HandleClear();
        Container.Elements.Clear();
    }
}