namespace Jackdaw;

/// <summary>
/// A container for storing and managing collections of children owned by a parent object.
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

    /// <summary>
    /// The number of elements currently stored in the container.
    /// </summary>
    public int Count => Elements.Count;

    /// <summary>
    /// If the container should store any change actions in a queue instead of executing immediately. <br/>
    /// If enabled, requires <see cref="ApplyChanges" /> to be run to change elements array.
    /// </summary>
    public bool QueueActions {
        get => queueActions;
        set {
            queueActions = value;
            if (!value) { ApplyChangesUntilEmpty(); }
        }
    }

    /// <summary>
    /// If the container has any queued actions to run.
    /// </summary>
    public bool HasQueuedActions => modifyActions.Count > 0;

    bool queueActions = false;
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

    /// <summary>
    /// Run code as all elements currently stored in the container. <br/>
    /// Only elements currently in the container will run, all elements added through the action will be skipped.
    /// </summary>
    /// <param name="action">The action to run.</param>
    public void RunAll(Action<Telement> action) {
        bool locked = QueueActions;
        // lock actions to avoid conflicts if new elements are added
        QueueActions = true;
        foreach (Telement element in Elements) { action(element); }
        QueueActions = locked;
    }

    /// <summary>
    /// Apply all queued changes. Only necessary if <see cref="QueueActions" /> is enabled. <br/>
    /// Only applies current changes. If applying changes causes more actions to be queued, the function needs to be re-run. <br/>
    /// The container can be checked for new queued actions with <see cref="HasQueuedActions"/>.
    /// </summary>
    public void ApplyChanges() {
        if (!HasQueuedActions) { return; }

        List<ChildContainerModifyAction<Telement, Towner>> modifyActionsCopy = [.. modifyActions];
        modifyActions.Clear();

        for (int i = 0; i < modifyActionsCopy.Count; ++i) {
            modifyActionsCopy[i].Execute();
        }
    }

    /// <summary>
    /// Apply queued changes, accounting for new items queued while applying.
    /// Only necessary if <see cref="QueueActions" /> is enabled. <br/>
    /// </summary>
    public void ApplyChangesUntilEmpty() {
        while (HasQueuedActions) { ApplyChanges(); }
    }

    Towner Action(ChildContainerModifyAction<Telement, Towner> action) {
        if (QueueActions) {
            modifyActions.Add(action);
        }
        else {
            action.Execute();
        }

        return Owner;
    }

    internal void HandleClear() {
        for (int i = 0; i < Elements.Count; ++i) {
            HandleRemove(Elements[i]);
        }
    }
}