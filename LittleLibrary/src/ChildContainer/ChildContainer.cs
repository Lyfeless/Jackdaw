namespace LittleLib;

public abstract class ChildContainer<Telement, Towner>(Towner owner) where Telement : class {
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
    public abstract ObjectIdentifier<Telement> Match(Telement element);
    public abstract int RecurseCount();
    public abstract ChildContainer<Telement, Towner> RecurseItem(int index);
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

    #region Search Functions
    public Telement? Find(Func<ObjectIdentifier<Telement>, bool> func) {
        foreach (Telement element in Elements) {
            if (func(Match(element))) { return element; }
        }

        return null;
    }

    public T? Find<T>(Func<ObjectIdentifier<Telement>, bool> func) where T : class, Telement {
        foreach (Telement element in Elements) {
            if (element is T typeElement && func(Match(typeElement))) { return typeElement; }
        }

        return null;
    }

    public T? Find<T>() where T : class, Telement {
        foreach (Telement element in Elements) {
            if (element is T typeElement) { return typeElement; }
        }

        return null;
    }

    public Telement[] FindAll(Func<ObjectIdentifier<Telement>, bool> func) {
        List<Telement> foundElements = [];
        foreach (Telement element in Elements) {
            if (func(Match(element))) { foundElements.Add(element); }
        }

        return [.. foundElements];
    }

    public T[] FindAll<T>(Func<ObjectIdentifier<Telement>, bool> func) where T : class, Telement {
        List<T> foundElements = [];
        foreach (Telement element in Elements) {
            if (element is T typeElement && func(Match(typeElement))) { foundElements.Add(typeElement); }
        }

        return [.. foundElements];
    }

    public T[] FindAll<T>() where T : class, Telement {
        List<T> foundElements = [];
        foreach (Telement element in Elements) {
            if (element is T typeElement) { foundElements.Add(typeElement); }
        }

        return [.. foundElements];
    }

    public Telement? FindRecursive(Func<ObjectIdentifier<Telement>, bool> func) {
        Telement? element = Find(func);
        if (element != null) { return element; }

        int count = RecurseCount();
        for (int i = 0; i < count; ++i) {
            element = RecurseItem(i).FindRecursive(func);
            if (element != null) { return element; }
        }

        return null;
    }

    public T? FindRecursive<T>(Func<ObjectIdentifier<Telement>, bool> func) where T : class, Telement {
        T? element = Find<T>(func);
        if (element != null) { return element; }

        int count = RecurseCount();
        for (int i = 0; i < count; ++i) {
            element = RecurseItem(i).FindRecursive<T>(func);
            if (element != null) { return element; }
        }

        return null;
    }

    public T? FindRecursive<T>() where T : class, Telement {
        T? element = Find<T>();
        if (element != null) { return element; }

        int count = RecurseCount();
        for (int i = 0; i < count; ++i) {
            element = RecurseItem(i).FindRecursive<T>();
            if (element != null) { return element; }
        }

        return null;
    }

    public Telement[] FindAllRecursive(Func<ObjectIdentifier<Telement>, bool> func) {
        Telement[] elements = FindAll(func);

        int count = RecurseCount();
        for (int i = 0; i < count; ++i) {
            Telement[] newElements = RecurseItem(i).FindAllRecursive(func);
            if (newElements.Length > 0) { elements = [.. elements, .. newElements]; }
        }

        return elements;
    }

    public T[] FindAllRecursive<T>(Func<ObjectIdentifier<Telement>, bool> func) where T : class, Telement {
        T[] elements = FindAll<T>(func);

        int count = RecurseCount();
        for (int i = 0; i < count; ++i) {
            T[] newElements = RecurseItem(i).FindAllRecursive<T>(func);
            if (newElements.Length > 0) { elements = [.. elements, .. newElements]; }
        }

        return elements;
    }

    public T[] FindAllRecursive<T>() where T : class, Telement {
        T[] elements = FindAll<T>();

        int count = RecurseCount();
        for (int i = 0; i < count; ++i) {
            T[] newElements = RecurseItem(i).FindAllRecursive<T>();
            if (newElements.Length > 0) { elements = [.. elements, .. newElements]; }
        }

        return elements;
    }
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