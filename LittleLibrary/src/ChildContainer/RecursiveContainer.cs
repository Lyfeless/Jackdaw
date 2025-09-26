namespace LittleLib;

/// <summary>
/// A container for storing, managing, and searching collections of children owned by a parent object.
/// </summary>
/// <typeparam name="Telement">The type of elements to store in the container.</typeparam>
/// <typeparam name="Towner">The type of owner.</typeparam>
/// <param name="owner">The owning object.</param>
public abstract class RecursiveSearchableChildContainer<Telement, Towner>(Towner owner) : SearchableChildContainer<Telement, Towner>(owner) where Telement : class {
    #region Overridable Functions
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
    protected abstract RecursiveSearchableChildContainer<Telement, Towner> RecurseItem(int index);
    #endregion

    #region Search Functions
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
}