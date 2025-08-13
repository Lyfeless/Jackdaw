namespace LittleLib;

internal class ChildContainerModifyActionClear<Telement, Towner>(ChildContainer<Telement, Towner> container) : ChildContainerModifyAction<Telement, Towner>(container) where Telement : class {
    public override void Execute() {
        //! FIXME (Alex): Handling needs to happen first but more effort needs to be made to ensure the containers are properly locked first
        Container.Elements.Clear();
        Container.HandleClear();
    }
}