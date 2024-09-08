using Foster.Framework;

namespace LittleLib;

/// <summary>
/// A layer responsible for loading and rendering stored entities
/// </summary>
public class EntityLayer : Layer {
    static readonly Dictionary<string, Func<EntitySaveData, Entity>> entityTypes = [];

    List<EntityReference> entities = [];

    public EntityLayer(LayerSaveData data, Level level, bool skipRegister) : base(data, level) {
        for (int i = 0; i < data.Entities.Length; ++i) {
            if (entityTypes.TryGetValue(data.Entities[i].NameID, out Func<EntitySaveData, Entity>? func)) {
                Entity entity = func(data.Entities[i]);

                if (!skipRegister) { EntityManager.Register(entity.Reference); }
                entity.SetLayer(this);
                AddEntity(entity);
            }
        }
    }

    public static void AddType(string id, Func<EntitySaveData, Entity> func) {
        entityTypes.Add(id, func);
    }

    public void AddEntity(Entity entity) {
        entities.Add(entity.Reference);
    }

    /// <summary>
    /// Remove entity from layer.
    /// WARNING: This doesn't delete the entity, just stops it from rendering on the layer.
    /// Deleting the entity should be done through the EntityManager, auto-clearing the entities from any layer
    /// </summary>
    /// <param name="entity"></param>
    public void RemoveEntity(Entity entity) {
        entities.RemoveAll(e => e.Entity == entity);
    }

    public void Activate() {
        foreach (EntityReference entity in entities) {
            entity.Entity?.OnActivate();
        }
    }

    public void Deactivate() {
        foreach (EntityReference entity in entities) {
            entity.Entity?.OnDeactivate();
        }
    }

    public override void Render(Batcher batcher) {
        // Remove empty elements and sort the list
        entities = [.. entities.Where(e => e.Entity != null).OrderBy(e => e.Entity?.GetSortValue())];

        // Render sorted entities
        foreach (EntityReference entityRef in entities) {
            // After the sort there should be no more invalid entities, but I don't like when the compiler complains
            if (entityRef.Entity == null) { continue; }

            // Cull entity that's not inside the camera's view
            Rect cameraBounds = Camera.ActiveCamera.Bounds;
            if (
                entityRef.Entity.Position.Precise.X + entityRef.Entity.Renderbox.Right < cameraBounds.Left ||
                entityRef.Entity.Position.Precise.X + entityRef.Entity.Renderbox.X > cameraBounds.Right ||
                entityRef.Entity.Position.Precise.Y + entityRef.Entity.Renderbox.Bottom < cameraBounds.Top ||
                entityRef.Entity.Position.Precise.Y + entityRef.Entity.Renderbox.Y > cameraBounds.Bottom
            ) { continue; }


            batcher.PushMatrix(entityRef.Entity.Position.Rounded);
            entityRef.Entity.Render(batcher);
            batcher.PopMatrix();
        }
    }
}