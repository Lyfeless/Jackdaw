namespace LittleLib;

public abstract class ChildContainer<T>() {

    public List<T> Elements = [];
    public T this[int index] {
        get => Elements[index];
        set => Elements[index] = value;
    }

    public bool QueueEvents = false;
    readonly List<ChildContainerModifyAction<T>> modifyActions = [];

    #region Overridable Functions
    public abstract void HandleAdd(T child);
    public abstract void HandleRemove(T child);
    public abstract bool CanAdd(T child);
    public abstract string Printable(T child);

    #endregion

    #region Modification Functions
    public void Add(T child) => Action(new ChildContainerModifyActionAdd<T>(this, child));

    public void Remove(T child) => Action(new ChildContainerModifyActionRemove<T>(this, child));

    public void AddTop(T child) => Action(new ChildContainerModifyActionAddTop<T>(this, child));

    public void AddAbove(T child, T relative) => AddRelative(child, relative, 0);
    public void AddBelow(T child, T relative) => AddRelative(child, relative, 1);

    void AddRelative(T child, T relative, int offset) => Action(new ChildContainerModifyActionAddRelative<T>(this, child, relative, offset));

    public void MoveDown(T child) => Move(child, 1);

    public void MoveUp(T child) => Move(child, -1);

    public void MoveTop(T child) => Move(child, -Elements.Count);

    public void MoveBottom(T child) => Move(child, Elements.Count);

    void Move(T child, int amount) => Action(new ChildContainerModifyActionMove<T>(this, child, amount));

    public void Clear() => Action(new ChildContainerModifyActionClear<T>(this));

    #endregion

    #region Internal Utilities
    void Action(ChildContainerModifyAction<T> action) {
        if (QueueEvents) {
            modifyActions.Add(action);
        }
        else {
            action.Execute();
        }
    }

    public void ApplyChanges() {
        if (modifyActions.Count == 0) { return; }
        foreach (ChildContainerModifyAction<T> action in modifyActions) {
            action.Execute();
        }
        modifyActions.Clear();
    }

    public void HandleClear() {
        foreach (T child in Elements) {
            HandleRemove(child);
        }
    }
    #endregion
}