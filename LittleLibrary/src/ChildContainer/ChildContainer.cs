namespace LittleLib;

public abstract class ChildContainer<Telement, Towner>(Towner owner) {
    protected readonly Towner Owner = owner;

    public List<Telement> Elements = [];
    public Telement this[int index] {
        get => Elements[index];
        set => Elements[index] = value;
    }

    readonly List<ChildContainerModifyAction<Telement, Towner>> modifyActions = [];

    #region Overridable Functions
    public abstract void HandleAdd(Telement child);
    public abstract void HandleRemove(Telement child);
    public abstract bool CanAdd(Telement child);
    public abstract bool Locked();
    public abstract string Printable(Telement child);

    #endregion

    #region Modification Functions
    public Towner Add(Telement child) => Action(new ChildContainerModifyActionAdd<Telement, Towner>(this, child));
    public Towner AddAll(params Telement[] children) { foreach (Telement child in children) { Add(child); } return Owner; }

    public Towner Remove(Telement child) => Action(new ChildContainerModifyActionRemove<Telement, Towner>(this, child));
    public Towner RemoveAll(params Telement[] children) { foreach (Telement child in children) { Remove(child); } return Owner; }

    public Towner AddTop(Telement child) => Action(new ChildContainerModifyActionAddTop<Telement, Towner>(this, child));

    public Towner AddAbove(Telement child, Telement relative) => AddRelative(child, relative, 0);
    public Towner AddBelow(Telement child, Telement relative) => AddRelative(child, relative, 1);

    Towner AddRelative(Telement child, Telement relative, int offset) => Action(new ChildContainerModifyActionAddRelative<Telement, Towner>(this, child, relative, offset));

    public Towner MoveDown(Telement child) => Move(child, 1);
    public Towner MoveDown(Telement child, int amount) => Move(child, amount);

    public Towner MoveUp(Telement child) => Move(child, -1);
    public Towner MoveUp(Telement child, int amount) => Move(child, -amount);

    public Towner MoveTop(Telement child) => Move(child, -Elements.Count);

    public Towner MoveBottom(Telement child) => Move(child, Elements.Count);

    Towner Move(Telement child, int amount) => Action(new ChildContainerModifyActionMove<Telement, Towner>(this, child, amount));

    public Towner Clear() => Action(new ChildContainerModifyActionClear<Telement, Towner>(this));

    #endregion

    #region Internal Utilities
    Towner Action(ChildContainerModifyAction<Telement, Towner> action) {
        if (Locked()) {
            modifyActions.Add(action);
        }
        else {
            action.Execute();
        }

        return Owner;
    }

    public void ApplyChanges() {
        if (modifyActions.Count == 0) { return; }
        foreach (ChildContainerModifyAction<Telement, Towner> action in modifyActions) {
            action.Execute();
        }
        modifyActions.Clear();
    }

    public void HandleClear() {
        foreach (Telement child in Elements) {
            HandleRemove(child);
        }
    }
    #endregion
}