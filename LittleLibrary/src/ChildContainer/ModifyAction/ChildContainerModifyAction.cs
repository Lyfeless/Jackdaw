namespace LittleLib;

internal abstract class ChildContainerModifyAction<T>(ChildContainer<T> container) {
    protected ChildContainer<T> Container = container;

    public abstract void Execute();
}