namespace LittleLib;

internal class ChildContainerModifyActionRemove<T>(ChildContainer<T> container, T child) : ChildContainerModifyAction<T>(container) {
    readonly T Child = child;

    public override void Execute() {
        Container.HandleRemove(Child);
        Container.Elements.Remove(Child);
    }
}