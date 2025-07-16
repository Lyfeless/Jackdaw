namespace LittleLib;

internal class ChildContainerModifyActionClear<T>(ChildContainer<T> container) : ChildContainerModifyAction<T>(container) {
    public override void Execute() {
        Container.Elements.Clear();
    }
}