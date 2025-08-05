namespace LittleLib;

internal abstract class ChildContainerModifyAction<Telement, Towner>(ChildContainer<Telement, Towner> container) {
    protected ChildContainer<Telement, Towner> Container = container;

    public abstract void Execute();
}