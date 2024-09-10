using System.Numerics;
using Foster.Framework;

namespace LittleLib;

/// <summary>
/// Inheritable class for storing any entity type.
/// </summary>
public abstract class Entity {
    public string InstanceID { get; }

    public readonly EntityReference Reference;
    protected EntityLayer layer;

    public RenderablePosition Position { get; protected set; }
    public Vector2 Velocity { get; protected set; }
    public HitboxContainer Hitboxes { get; protected set; } = new();
    public Rect Renderbox { get; protected set; }

    public readonly Point2 Size;

    protected List<EntityCollisionInfo> Collisions = [];
    public bool HitboxCollided(Hitbox hitbox) => Collisions.Any(c => c.LocalHitbox == hitbox);

    public string? TimeTracker { get; private set; }
    public void SetTimeTracker(string? tracker) => TimeTracker = tracker;
    public bool IsFrozen => TimeTracker != null && TimeManager.GetPaused(TimeTracker);

    //! FIXME (Alex): Not a fan of the redunant code here between these two constructors, but c# really doesn't like readonly assignments being put in subfunctions

    /// <summary>
    /// Entity loading logic when loaded from a save file.
    /// </summary>
    /// <param name="data">Savedata loaded from the level file.</param>
    public Entity(EntitySaveData data) {
        if (data.WorldX != null && data.WorldY != null) {
            Position = new(new(data.WorldX ?? 0, data.WorldY ?? 0));
        }
        else {
            Position = new(new(data.Position[0], data.Position[1]));
        }

        InstanceID = data.InstanceID;

        Size = new(data.Width, data.Height);

        TimeTracker = EntityLayer.GetDefaultTracker(GetType());

        Reference = new(this);
    }

    /// <summary>
    /// Entity loading logic when created new.
    /// </summary>
    /// <param name="layer">Layer the entity is created on.</param>
    /// <param name="args">Entity creation data.</param>
    public Entity(EntityLayer layer, EntityCreateArgs args) {
        Position = new(args.Position);

        this.layer = layer;

        InstanceID = args.InstanceID;

        Reference = new(this);

        TimeTracker = args.TimeTracker ?? EntityLayer.GetDefaultTracker(GetType());

        Size = args.Size;

        if (!args.SkipRegister) { EntityManager.Register(Reference); }
    }

    public void OnActivate() {
        Activate();
        EntityManager.Register(this);
    }
    public virtual void Activate() { }

    public void OnDeactivate() {
        Deactivate();
        EntityManager.Remove(this);
    }
    public virtual void Deactivate() { }

    /// <summary>
    /// Update logic, overridden for per-entity behavior.
    /// </summary>
    public abstract void Update();

    /// <summary>
    /// Entity rendering, overridden for per-entity display differences.
    /// Expects matricies applied to batcher to be in world space adjusted by camera offsets.
    /// </summary>
    public abstract void Render(Batcher batcher);

    public void SetLayer(EntityLayer layer) {
        this.layer = layer;
    }

    /// <summary>
    /// Move Entity by current internal velocity, while adjusting position and velocity based on tile collisions.
    /// </summary>
    /// <param name="stopVelocity">If true, stops the velocity on an axis when hitting a wall</param>
    /// <returns>Collision info for the x and y</returns>
    public (TileCollisionInfo, TileCollisionInfo) MoveAndCollide(bool stopVelocityX = true, bool stopVelocityY = true) {
        (TileCollisionInfo colX, TileCollisionInfo colY) = MoveAndCollide(Velocity);
        Velocity = new(
            (stopVelocityX && colX.Collision) ? 0 : Velocity.X,
            (stopVelocityY && colY.Collision) ? 0 : Velocity.Y
        );

        return (colX, colY);
    }

    public (TileCollisionInfo, TileCollisionInfo) MoveAndCollide(Vector2 amount) {
        TileCollisionInfo collisionInfoX;
        TileCollisionInfo collisionInfoY;

        Vector2 newPosition = Position.Precise;

        if (!amount.X.Equals(0)) {
            // Get collision information for target position
            newPosition.X += amount.X;
            collisionInfoX = LevelManager.ActiveLevel.CheckEntityCollision(newPosition, Hitboxes);

            if (collisionInfoX.Collision) {
                foreach (TileHitboxCollisionInfo hitboxCollision in collisionInfoX.HitboxCollisions) {
                    newPosition.X = hitboxCollision.Hitbox.ResolveX(hitboxCollision, newPosition.X, Position.Precise.X, amount.X);
                }
            }
        }
        else {
            collisionInfoX = TileCollisionInfo.Empty;
        }

        if (!amount.Y.Equals(0)) {
            // Get collision information for target position
            newPosition.Y += amount.Y;
            collisionInfoY = LevelManager.ActiveLevel.CheckEntityCollision(newPosition, Hitboxes);

            if (collisionInfoY.Collision) {
                foreach (TileHitboxCollisionInfo hitboxCollision in collisionInfoY.HitboxCollisions) {
                    newPosition.Y = hitboxCollision.Hitbox.ResolveY(hitboxCollision, newPosition.Y, Position.Precise.Y, amount.Y);
                }
            }
        }
        else {
            collisionInfoY = TileCollisionInfo.Empty;
        }

        Position.Set(newPosition);
        return (collisionInfoX, collisionInfoY);
    }

    public void ClearCollisions() {
        Collisions = [];
    }

    public void AddCollision(EntityReference entity, Hitbox hitbox, Hitbox localHitbox) {
        Collisions.Add(new(entity, hitbox, localHitbox));
    }

    /// <summary>
    /// Calculate value that the entity should be y sorted against. Defaults to the bottom of the entity's hitbox.
    /// </summary>
    /// <returns>The y value the renderer should use to sort the entity.</returns>
    public virtual float GetSortValue() {
        Rect bounds = Hitboxes.Bounds;
        return Position.Rounded.Y - bounds.Y + bounds.Height;
    }

    /// <summary>
    /// Calculate position that entities or camera should target this entity. Defaults to the center of the entity's hitbox
    /// </summary>
    /// <returns>The Vector2 position of the target point.</returns>
    public virtual Vector2 GetTargetPosition() {
        return Position.Rounded + Hitboxes.Bounds.Center;
    }

    public Vector2 RelativeCoordsToWorldCoords(Vector2 relativeCoords) {
        return relativeCoords + Position.Precise;
    }

    public Vector2 WorldCoordsToRelativeCoords(Vector2 relativeCoords) {
        return relativeCoords - Position.Precise;
    }
}