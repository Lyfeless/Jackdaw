namespace Jackdaw;

/// <summary>
/// A container for storing, managing, and searching collections of children owned by a parent object.
/// </summary>
/// <typeparam name="Telement">The type of elements to store in the container.</typeparam>
/// <typeparam name="Towner">The type of owner.</typeparam>
/// <param name="owner">The owning object.</param>
public abstract class SearchableChildContainer<Telement, Towner>(Towner owner) : ChildContainer<Telement, Towner>(owner) where Telement : class {
    #region Overridable Functions
    /// <summary>
    /// Get the identifier object used for comparing children.
    /// </summary>
    /// <param name="element">The element to match.</param>
    /// <returns>The element's identifier object.</returns>
    protected abstract ObjectIdentifier<Telement> Match(Telement element);
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
    /// Check if the container has a child that matches the condition.
    /// </summary>
    /// <param name="func">The condition function to check.</param>
    /// <returns>If a matching element was found.</returns>
    public bool Has(Func<ObjectIdentifier<Telement>, bool> func)
        => Find(func) != null;

    /// <summary>
    /// Check if the container has a child that matches the condition.
    /// </summary>
    /// <typeparam name="T">The type of child to search for.</typeparam>
    /// <param name="func">The condition function to check.</param>
    /// <returns>If a matching element was found.</returns>
    public bool Has<T>(Func<ObjectIdentifier<Telement>, bool> func) where T : class, Telement
        => Find<T>(func) != null;

    /// <summary>
    /// Check if the container has a child that matches the condition.
    /// </summary>
    /// <typeparam name="T">The type of child to search for.</typeparam>
    /// <returns>If a matching element was found.</returns>
    public bool Has<T>() where T : class, Telement
        => Find<T>() != null;

    /// <summary>
    /// Check if the container has a child that matches the condition.
    /// </summary>
    /// <param name="func">The condition function to check.</param>
    /// <param name="foundElement">The first matching element found.</param>
    /// <returns>If a matching element was found.</returns>
    public bool Has(Func<ObjectIdentifier<Telement>, bool> func, out Telement foundElement) {
        foundElement = Find(func)!;
        return foundElement != null;
    }

    /// <summary>
    /// Check if the container has a child that matches the condition.
    /// </summary>
    /// <typeparam name="T">The type of child to search for.</typeparam>
    /// <param name="func">The condition function to check.</param>
    /// <param name="foundElement">The first matching element found.</param>
    /// <returns>If a matching element was found.</returns>
    public bool Has<T>(Func<ObjectIdentifier<Telement>, bool> func, out T foundElement) where T : class, Telement {
        foundElement = Find<T>(func)!;
        return foundElement != null;
    }

    /// <summary>
    /// Check if the container has a child that matches the condition.
    /// </summary>
    /// <typeparam name="T">The type of child to search for.</typeparam>
    /// <param name="foundElement">The first matching element found.</param>
    /// <returns>If a matching element was found.</returns>
    public bool Has<T>(out T foundElement) where T : class, Telement {
        foundElement = Find<T>()!;
        return foundElement != null;
    }

    #endregion
}