using System.Numerics;
using Foster.Framework;

namespace LittleLib;

/// <summary>
/// Static class responsible for storing and updating all active entities.
/// </summary>
public static class EntityManager {
    public static readonly List<EntityReference> Entities = [];

    static readonly HashSet<EntityReference> clearList = [];

    /// <summary>
    /// Remove all registered entities, mostly useful for changing scenes
    /// </summary>
    public static void Clear() {
        Entities.Clear();
    }

    /// <summary>
    /// Register an unregistered entity to the update queue.
    /// NOTE: This should only be used in very specific edge cases, Pre-registered entites should be used whenever possible
    /// </summary>
    /// <param name="entity">Entity to be referenced and </param>
    public static void Register(Entity entity) {
        Register(entity.Reference);
    }

    /// <summary>
    /// Register an entity for updates.
    /// NOTE: Automatically called by the entity on creation.
    /// This doesn't need to be called manually unless entity is removed and needs to be re-registered.
    /// </summary>
    /// <param name="entity">Reference object to the entity</param>
    public static void Register(EntityReference entity) {
        if (Entities.Contains(entity)) { return; }
        Entities.Add(entity);
    }

    /// <summary>
    /// Removed an entity from update management.
    /// If not manually referenced anywhere, this will automatically remove the entity from any layer rendering as well.
    /// This may take a tick to go into effect.
    /// </summary>
    /// <param name="entity"></param>
    public static void Remove(Entity entity) {
        Remove(entity.Reference);
    }

    public static void Remove(EntityReference entity) {
        clearList.Add(entity);
    }

    #region Updating

    /// <summary>
    /// Update all entities
    /// </summary>
    public static void Update() {
        // Run update on all registered entities
        for (int i = Entities.Count - 1; i >= 0; --i) {
            // Skip update if entity is set to be cleared in this tick
            if (clearList.Contains(Entities[i])) { continue; }
            // Update entity if timer is running
            if ((!Entities[i].Entity?.IsFrozen) ?? false) { Entities[i].Entity?.Update(); }
            // Remove entity if it was deleted elsewhere
            if (Entities[i].Entity == null) {
                clearList.Add(Entities[i]);
                continue;
            }

            // Collisions from the previous tick need to be cleared before we can go into the next collisions check
            Entities[i].Entity?.ClearCollisions();
        }

        // Clear old entities
        //      Decrement backwards to allow for index removal without messing up loop
        foreach (EntityReference entity in clearList) {
            Entities.Remove(entity);
            entity.ClearEntity();
        }
        clearList.Clear();

        //! FIXME (Alex): Benchmark this in the future, I assume we're going to run into performance issues if we continue checking every entity against every other entity
        // Weird nested for loop helps ensure each entity pair is only checked once, with the second iterator only checking elements after the first iterator
        for (int i = 0; i < Entities.Count - 1; ++i) {
            // Get first entity + null guard
            EntityReference entity1 = Entities[i];
            if (entity1.Entity == null) { continue; }

            for (int ii = i + 1; ii < Entities.Count; ++ii) {
                // Get second entity + null guard
                EntityReference entity2 = Entities[ii];
                if (entity2.Entity == null) { continue; }

                // Entity1 can collide with entity2?
                bool entity1TagMatch = entity2.Entity.Hitboxes.HasAnyTag(entity1.Entity.Hitboxes.CollisionTags);
                // Entity2 can collide with entity1?
                bool entity2TagMatch = entity1.Entity.Hitboxes.HasAnyTag(entity2.Entity.Hitboxes.CollisionTags);
                if (!entity1TagMatch && !entity2TagMatch) { continue; }

                // Skip if no hitboxes overlap
                if (!CheckOverlap(
                    entity1.Entity.Position.Precise,
                    entity1.Entity.Hitboxes.Bounds,
                    entity2.Entity.Position.Precise,
                    entity2.Entity.Hitboxes.Bounds
                )) { continue; }

                int checkCount = 0;

                // Check each hitbox against each other
                foreach (Hitbox entity1Hitbox in entity1.Entity.Hitboxes.Hitboxes) {
                    foreach (Hitbox entity2Hitbox in entity2.Entity.Hitboxes.Hitboxes) {
                        // Skip if tags don't match
                        bool hitbox1TagMatch = entity2Hitbox.HasAnyTag(entity1Hitbox.CollisionTags);
                        bool hitbox2TagMatch = entity1Hitbox.HasAnyTag(entity2Hitbox.CollisionTags);

                        if (!hitbox1TagMatch && !hitbox2TagMatch) { continue; }

                        checkCount++;

                        if (CheckOverlap(
                            entity1.Entity.Position.Precise,
                            entity1Hitbox.Bounds,
                            entity2.Entity.Position.Precise,
                            entity2Hitbox.Bounds)
                        ) {
                            if (hitbox1TagMatch) { entity1.Entity.AddCollision(entity2, entity2Hitbox, entity1Hitbox); }
                            if (hitbox2TagMatch) { entity2.Entity.AddCollision(entity1, entity1Hitbox, entity2Hitbox); }
                        }
                    }
                }
            }
        }
    }

    public static EntityReference[] GetEntitiesAtPoint(Vector2 point) {
        List<EntityReference> hitEntities = [];
        foreach (EntityReference entity in Entities) {
            if (entity.Entity == null) { continue; }

            Rect entityBounds = entity.Entity.Hitboxes.Bounds;
            if (
                point.X < entityBounds.X + entity.Entity.Position.Precise.X ||
                point.X >= entityBounds.X + entity.Entity.Position.Precise.X ||
                point.Y < entityBounds.Y + entity.Entity.Position.Precise.Y ||
                point.Y >= entityBounds.Y + entity.Entity.Position.Precise.Y
            ) { continue; }

            foreach (Hitbox hitbox in entity.Entity.Hitboxes.Hitboxes) {
                if (
                    point.X < hitbox.Bounds.X + entity.Entity.Position.Precise.X ||
                    point.X >= hitbox.Bounds.X + entity.Entity.Position.Precise.X ||
                    point.Y < hitbox.Bounds.Y + entity.Entity.Position.Precise.Y ||
                    point.Y >= hitbox.Bounds.Y + entity.Entity.Position.Precise.Y
                ) {
                    hitEntities.Add(entity);
                    break;
                }
            }
        }

        return [.. hitEntities];
    }

    public static EntityReference[] GetEntitiesInRect(Rect rect) {
        List<EntityReference> hitEntities = [];
        foreach (EntityReference entity in Entities) {
            if (entity.Entity == null) { continue; }

            if (!CheckOverlap(entity.Entity.Position.Precise, entity.Entity.Hitboxes.Bounds, Vector2.Zero, rect)) { continue; }

            foreach (Hitbox hitbox in entity.Entity.Hitboxes.Hitboxes) {
                if (CheckOverlap(entity.Entity.Position.Precise, hitbox.Bounds, Vector2.Zero, rect)) {
                    hitEntities.Add(entity);
                    break;
                }
            }
        }

        return [.. hitEntities];
    }

    static bool CheckOverlap(Vector2 position1, Rect bounds1, Vector2 position2, Rect bounds2) {
        return (
            position1.X + bounds1.Right >= position2.X + bounds2.Left &&
            position1.X + bounds1.Left <= position2.X + bounds2.Right &&
            position1.Y + bounds1.Bottom >= position2.Y + bounds2.Top &&
            position1.Y + bounds1.Top <= position2.Y + bounds2.Bottom
        );
    }

    // public static bool 

    #endregion

    #region Lookup functions

    /// <summary>
    /// Find first registered entity that matches the given tags.
    /// </summary>
    /// <param name="tags">Tag objects combined into a single value e.g <c>EntityTags.A | EntityTags.B | EntityTags.C</c></param>
    /// <returns>The first entity found in the list, or null if no matching entities are present.</returns>
    public static EntityReference FindEntityByTag(long tags) {
        return Entities.Find(e => e.Entity != null && e.Entity.Hitboxes.HasTags(tags)) ?? EntityReference.Empty;
    }

    /// <summary>
    /// Find all registered entities that match the given tags.
    /// </summary>
    /// <param name="tags">Tag objects combined into a single value e.g <c>EntityTags.A | EntityTags.B | EntityTags.C</c></param>
    /// <returns>An array of entities.</returns>
    public static EntityReference[] FindEntitiesByTag(long tags) {
        return Entities.Where(e => e.Entity != null && e.Entity.Hitboxes.HasTags(tags)).ToArray();
    }

    /// <summary>
    /// Find first registered entity that matches the given type.
    /// </summary>
    /// <typeparam name="T">The type to search for.</typeparam>
    /// <returns>The first entity found in the list, or null if no matching entities are present.</returns>
    public static EntityReference FindEntityByType<T>() where T : Entity {
        return Entities.Find(e => e.Entity?.GetType() == typeof(T)) ?? EntityReference.Empty;
    }

    /// <summary>
    /// Find all registered entities that match the given type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>An array of entities.</returns>
    public static EntityReference[] FindEntitiesByType<T>() where T : Entity {
        return Entities.Where(e => e.Entity?.GetType() == typeof(T)).ToArray();
    }

    /// <summary>
    /// Find registered entity by unique instance ID
    /// </summary>
    /// <param name="InstanceID">Unique instance identifier</param>
    /// <returns>The entity with the matching instance ID, or null if no matching entity isn't present.</returns>
    public static EntityReference FindEntityByInstanceID(string InstanceID) {
        return Entities.Find(e => e.Entity?.InstanceID == InstanceID) ?? EntityReference.Empty;
    }

    #endregion
}