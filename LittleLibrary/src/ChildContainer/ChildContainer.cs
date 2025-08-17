namespace LittleLib;

/// <summary>
/// A container for storing, managing, and searching collections of children owned by a parent object.
/// </summary>
/// <typeparam name="Telement">The type of elements to store in the container.</typeparam>
/// <typeparam name="Towner">The type of owner.</typeparam>
/// <param name="owner">The owning object.</param>
public abstract class ChildContainer<Telement, Towner>(Towner owner) where Telement : class {
    protected readonly Towner Owner = owner;

    /// <summary>
    /// All the elements currently being stored.
    /// </summary>
    public readonly List<Telement> Elements = [];
    public Telement this[int index] {
        get => Elements[index];
        set => Elements[index] = value;
    }

    internal readonly List<ChildContainerModifyAction<Telement, Towner>> modifyActions = [];

    #region Overridable Functions
    /// <summary>
    /// Update any child object added to the container.
    /// </summary>
    /// <param name="child">The child being added.</param>
    public abstract void HandleAdd(Telement child);

    /// <summary>
    /// Update any child object removed from the container.
    /// </summary>
    /// <param name="child">The child being removed.</param>
    public abstract void HandleRemove(Telement child);

    /// <summary>
    /// If the child is able to be added to the container.
    /// </summary>
    /// <param name="child">The child to be added.</param>
    /// <returns>If the child can be added.</returns>
    public abstract bool CanAdd(Telement child);

    //! FIXME (Alex): recursion functions probably aren't very useable outside of the current use case, is that a problem?

    /// <summary>
    /// Get the identifier object used for comparing children.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    protected abstract ObjectIdentifier<Telement> Match(Telement element);

    /// <summary>
    /// Get the number of child elements that need to be recursed in a search.
    /// </summary>
    /// <returns>The number of recursable children.</returns>
    protected abstract int RecurseCount();

    /// <summary>
    /// Get a recursable child element.
    /// </summary>
    /// <param name="index">The child element index.</param>
    /// <returns>The recursable child element</returns>
    protected abstract ChildContainer<Telement, Towner> RecurseItem(int index);

    /// <summary>
    /// Check if the container is locked and should queue any modifications.
    /// </summary>
    /// <returns></returns>
    public abstract bool Locked();

    /// <summary>
    /// Get printable string for an element.
    /// </summary>
    /// <param name="child"></param>
    /// <returns>a string identifier for the element.</returns>
    public abstract string Printable(Telement child);

    #endregion

    #region Modification Functions
    /// <summary>
    /// Add a child to the bottom of the elements array.
    /// </summary>
    /// <param name="child">The child to add.</param>
    /// <returns>The container owner.</returns>
    public Towner Add(Telement child) => Action(new ChildContainerModifyActionAdd<Telement, Towner>(this, child));

    /// <summary>
    /// Add all children to the bottom of the elements array.
    /// </summary>
    /// <param name="children">The children to add.</param>
    /// <returns>The container owner.</returns>
    public Towner AddAll(params Telement[] children) { foreach (Telement child in children) { Add(child); } return Owner; }

    /// <summary>
    /// Remove a child from the elements array.
    /// </summary>
    /// <param name="child">The child to remove</param>
    /// <returns>The container owner.</returns>
    public Towner Remove(Telement child) => Action(new ChildContainerModifyActionRemove<Telement, Towner>(this, child));

    /// <summary>
    /// Remove all the children from the elements array.
    /// </summary>
    /// <param name="children">The children to remove.</param>
    /// <returns>The container owner.</returns>
    public Towner RemoveAll(params Telement[] children) { foreach (Telement child in children) { Remove(child); } return Owner; }

    /// <summary>
    /// Add a child to the top of the elements array.
    /// </summary>
    /// <param name="child">The child to add.</param>
    /// <returns>The container owner.</returns>
    public Towner AddTop(Telement child) => Action(new ChildContainerModifyActionAddTop<Telement, Towner>(this, child));

    /// <summary>
    /// Add a child above another in the elements array.
    /// </summary>
    /// <param name="child">The child to add.</param>
    /// <param name="relative">The child currently in the container.</param>
    /// <returns>The container owner.</returns>
    public Towner AddAbove(Telement child, Telement relative) => AddRelative(child, relative, 0);

    /// <summary>
    /// Add a child below another in the elements array.
    /// </summary>
    /// <param name="child">The child to add.</param>
    /// <param name="relative">The child currently in the container.</param>
    /// <returns>The container owner.</returns>
    public Towner AddBelow(Telement child, Telement relative) => AddRelative(child, relative, 1);

    Towner AddRelative(Telement child, Telement relative, int offset) => Action(new ChildContainerModifyActionAddRelative<Telement, Towner>(this, child, relative, offset));

    /// <summary>
    /// Move a child 1 element closer to the bottom of the elements array.
    /// </summary>
    /// <param name="child">The child to move.</param>
    /// <returns>The container owner.</returns>
    public Towner MoveDown(Telement child) => Move(child, 1);

    /// <summary>
    /// Move a child closer to the bottom of the elements array.
    /// </summary>
    /// <param name="child">The child to move.</param>
    /// <param name="amount">The amount to move the child.</param>
    /// <returns>The container owner.</returns>
    public Towner MoveDown(Telement child, int amount) => Move(child, amount);

    /// <summary>
    /// Move a child 1 element closer to the top of the elements array.
    /// </summary>
    /// <param name="child">The child to move.</param>
    /// <returns>The container owner.</returns>
    public Towner MoveUp(Telement child) => Move(child, -1);

    /// <summary>
    /// Move a child closer to the top of the elements array.
    /// </summary>
    /// <param name="child">The child to move.</param>
    /// <param name="amount">The amount to move the child.</param>
    /// <returns>The container owner.</returns>
    public Towner MoveUp(Telement child, int amount) => Move(child, -amount);

    /// <summary>
    /// Move a child to the top of the elements array.
    /// </summary>
    /// <param name="child">The child to move.</param>
    /// <returns>The container owner.</returns>
    public Towner MoveTop(Telement child) => Move(child, -Elements.Count);

    /// <summary>
    /// Move a child to the bottom of the elements array.
    /// </summary>
    /// <param name="child">The child to move.</param>
    /// <returns>The container owner.</returns>
    public Towner MoveBottom(Telement child) => Move(child, Elements.Count);

    Towner Move(Telement child, int amount) => Action(new ChildContainerModifyActionMove<Telement, Towner>(this, child, amount));

    /// <summary>
    /// Empty the elements array.
    /// </summary>
    /// <returns>The container owner.</returns>
    public Towner Clear() => Action(new ChildContainerModifyActionClear<Telement, Towner>(this));

    #endregion

    #region Search Functions
    /// <summary>
    /// Find a child matching the condition.
    /// </summary>
    /// <param name="func">The condition function to check.</param>
    /// <returns>A matching element, null if no element is found.</returns>
    public Telement? Find(Func<ObjectIdentifier<Telement>, bool> func) {
        foreach (Telement element in Elements) {
            if (func(Match(element))) { return element; }
        }

        foreach (ChildContainerModifyAction<Telement, Towner> action in modifyActions) {
            if (action is ChildContainerModifyActionAdd<Telement, Towner> addAction) {
                if (func(Match(addAction.Child))) { return addAction.Child; }
            }
        }

        return null;
    }

    /// <summary>
    /// Find a child of the given type that matches the condition.
    /// </summary>
    /// <typeparam name="T">The type of child to search for.</typeparam>
    /// <param name="func">The condition function to check.</param>
    /// <returns>A matching element, null if no element is found.</returns>
    public T? Find<T>(Func<ObjectIdentifier<Telement>, bool> func) where T : class, Telement {
        foreach (Telement element in Elements) {
            if (element is T typeElement && func(Match(typeElement))) { return typeElement; }
        }

        foreach (ChildContainerModifyAction<Telement, Towner> action in modifyActions) {
            if (action is ChildContainerModifyActionAdd<Telement, Towner> addAction) {
                if (addAction.Child is T typeElement && func(Match(typeElement))) { return typeElement; }
            }
        }

        return null;
    }

    /// <summary>
    /// Find a child of the given type
    /// </summary>
    /// <typeparam name="T">The type of child to search for.</typeparam>
    /// <returns>A matching element, null if no element is found.</returns>
    public T? Find<T>() where T : class, Telement {
        foreach (Telement element in Elements) {
            if (element is T typeElement) { return typeElement; }
        }

        foreach (ChildContainerModifyAction<Telement, Towner> action in modifyActions) {
            if (action is ChildContainerModifyActionAdd<Telement, Towner> addAction) {
                if (addAction.Child is T typeElement) { return typeElement; }
            }
        }

        return null;
    }

    /// <summary>
    /// Find all children matching the condition.
    /// </summary>
    /// <param name="func">The condition function to check.</param>
    /// <returns>An array of all found elements.</returns>
    public Telement[] FindAll(Func<ObjectIdentifier<Telement>, bool> func) {
        List<Telement> foundElements = [];
        foreach (Telement element in Elements) {
            if (func(Match(element))) { foundElements.Add(element); }
        }

        foreach (ChildContainerModifyAction<Telement, Towner> action in modifyActions) {
            if (action is ChildContainerModifyActionAdd<Telement, Towner> addAction) {
                if (func(Match(addAction.Child))) { foundElements.Add(addAction.Child); }
            }
        }

        return [.. foundElements];
    }

    /// <summary>
    /// Find all children of the given type that match the condition.
    /// </summary>
    /// <typeparam name="T">The type of child to search for.</typeparam>
    /// <param name="func">The condition function to check.</param>
    /// <returns>An array of all found elements.</returns>
    public T[] FindAll<T>(Func<ObjectIdentifier<Telement>, bool> func) where T : class, Telement {
        List<T> foundElements = [];
        foreach (Telement element in Elements) {
            if (element is T typeElement && func(Match(typeElement))) { foundElements.Add(typeElement); }
        }

        foreach (ChildContainerModifyAction<Telement, Towner> action in modifyActions) {
            if (action is ChildContainerModifyActionAdd<Telement, Towner> addAction) {
                if (addAction.Child is T typeElement && func(Match(typeElement))) { foundElements.Add(typeElement); }
            }
        }

        return [.. foundElements];
    }

    /// <summary>
    /// Find all children of the given type.
    /// </summary>
    /// <typeparam name="T">The type of child to search for.</typeparam>
    /// <returns>An array of all found elements.</returns>
    public T[] FindAll<T>() where T : class, Telement {
        List<T> foundElements = [];
        foreach (Telement element in Elements) {
            if (element is T typeElement) { foundElements.Add(typeElement); }
        }

        foreach (ChildContainerModifyAction<Telement, Towner> action in modifyActions) {
            if (action is ChildContainerModifyActionAdd<Telement, Towner> addAction) {
                if (addAction.Child is T typeElement) { foundElements.Add(typeElement); }
            }
        }

        return [.. foundElements];
    }

    /// <summary>
    /// Recursively finds a child that matches the condition in the elements array and all child objects.
    /// </summary>
    /// <param name="func">The condition function to check.</param>
    /// <returns>A matching element, null if no element is found.</returns>
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

    /// <summary>
    /// Recursively finds a child of the given type that matches the condition in the elements array and all child objects.
    /// </summary>
    /// <typeparam name="T">The type of child to search for.</typeparam>
    /// <param name="func">The condition function to check.</param>
    /// <returns>A matching element, null if no element is found.</returns>
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

    /// <summary>
    /// Recursively finds a child of the given type in the elements array and all child objects.
    /// </summary>
    /// <typeparam name="T">The type of child to search for.</typeparam>
    /// <returns>A matching element, null if no element is found.</returns>
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

    /// <summary>
    /// Recursively finds all children that match the condition in the elements array and all child objects.
    /// </summary>
    /// <param name="func">The condition function to check.</param>
    /// <returns>An array of all found elements.</returns>
    public Telement[] FindAllRecursive(Func<ObjectIdentifier<Telement>, bool> func) {
        Telement[] elements = FindAll(func);

        int count = RecurseCount();
        for (int i = 0; i < count; ++i) {
            Telement[] newElements = RecurseItem(i).FindAllRecursive(func);
            if (newElements.Length > 0) { elements = [.. elements, .. newElements]; }
        }

        return elements;
    }

    /// <summary>
    /// Recursively finds all children of the given type that match the condition in the elements array and all child objects.
    /// </summary>
    /// <typeparam name="T">The type of child to search for.</typeparam>
    /// <param name="func">The condition function to check.</param>
    /// <returns>An array of all found elements.</returns>
    public T[] FindAllRecursive<T>(Func<ObjectIdentifier<Telement>, bool> func) where T : class, Telement {
        T[] elements = FindAll<T>(func);

        int count = RecurseCount();
        for (int i = 0; i < count; ++i) {
            T[] newElements = RecurseItem(i).FindAllRecursive<T>(func);
            if (newElements.Length > 0) { elements = [.. elements, .. newElements]; }
        }

        return elements;
    }

    /// <summary>
    /// Recursively finds all children of the given type in the elements array and all child objects.
    /// </summary>
    /// <typeparam name="T">The type of child to search for.</typeparam>
    /// <returns>An array of all found elements.</returns>
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
        for (int i = 0; i < modifyActions.Count; ++i) {
            modifyActions[i].Execute();
        }
        modifyActions.Clear();
    }

    public void HandleClear() {
        for (int i = 0; i < Elements.Count; ++i) {
            HandleRemove(Elements[i]);
        }
    }
    #endregion
}