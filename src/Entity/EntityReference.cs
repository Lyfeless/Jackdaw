namespace LittleLib;

/// <summary>
/// Storage container for an entity.
/// </summary>
/// <param name="entity">Entity to store in the reference.</param>
public class EntityReference(Entity? entity) {
    public static readonly EntityReference Empty = new(null);

    /// <summary>
    /// Warning: Do not directly store this entity, doing so has the potential to break entity removal.
    /// </summary>
    public Entity? Entity { get; private set; } = entity;

    /// <summary>
    /// Deload the entity reference. This will remove it from all updating and rendering.
    /// NOTE: This is not the recommended way to do this. Use instead: <c>EntityManager.Remove(this);</c>
    /// </summary>
    public void ClearEntity() { Entity = null; }
}