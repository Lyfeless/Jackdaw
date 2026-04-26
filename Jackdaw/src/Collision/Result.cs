using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Information from a manual collision query with another collider.
/// </summary>
public readonly struct CollisionResult {
    /// <summary>
    /// Information for collisions with a single collision component.
    /// </summary>
    public readonly struct ComponentResult {
        /// <summary>
        /// The collided component.
        /// </summary>
        public readonly CollisionComponent Component;

        /// <summary>
        /// The individual intersected colliders. <br/>
        /// This will generally be a single element unless the collider uses sub-colliders.
        /// </summary>
        public readonly ColliderResult[] Colliders = [];

        internal readonly Data data = new();

        /// <summary>
        /// The amount to move the other collider to seperate, if applicable.
        /// </summary>
        public readonly Vector2 Pushout => data.Pushout;

        /// <summary>
        /// The sweep information for the collision, if applicable.
        /// </summary>
        public readonly SweepResult Sweep => data.Sweep;

        internal ComponentResult(CollisionComponent component, ColliderResult[] colliders) {
            Component = component;
            Colliders = colliders;
            if (colliders.Length > 0) {
                // Results should already be pre-sorted so the first result can be taken
                data = colliders[0].data;
            }
        }
    }

    /// <summary>
    /// Information for collisions with a single collider.
    /// </summary>
    public readonly struct ColliderResult {
        /// <summary>
        /// The intersected collider.
        /// </summary>
        public readonly Collider Collider;

        internal readonly Data data = new();

        /// <summary>
        /// The amount to move the other collider to seperate, if applicable.
        /// </summary>
        public readonly Vector2 Pushout => data.Pushout;

        /// <summary>
        /// The sweep information for the collision, if applicable.
        /// </summary>
        public readonly SweepResult Sweep => data.Sweep;

        internal ColliderResult(ColliderContext ctx) {
            Collider = ctx.Collider;
        }

        internal ColliderResult(ColliderContext ctx, EPAPushout pushout) : this(ctx) {
            data = new(pushout);
        }

        internal ColliderResult(ColliderContext ctx, JDASweep sweep) : this(ctx) {
            data = new(sweep);
        }
    }

    internal readonly struct Data {
        public readonly Vector2 Pushout = Vector2.Zero;
        public readonly SweepResult Sweep = new();

        internal Data(EPAPushout pushout) : this(pushout.Pushout) { }
        internal Data(Vector2 pushout) {
            Pushout = pushout;
        }

        internal Data(JDASweep sweep) : this(new SweepResult(sweep)) { }
        internal Data(SweepResult sweep) {
            Sweep = sweep;
            if (Sweep.Fraction < 0) { Pushout = Sweep.AdjustedVelocity; }
        }
    }

    /// <summary>
    /// Information for resolving a swept collision.
    /// </summary>
    public readonly struct SweepResult {
        /// <summary>
        /// If the sweep collided.
        /// </summary>
        public readonly bool Collided = false;

        /// <summary>
        /// The fraction of the sweep velocity that was able to be completed before a collision.
        /// </summary>
        public readonly float Fraction = 1;

        /// <summary>
        /// The fraction of the sweep velocity that was able to be completed before a collision, clamped to not be less than 0
        /// </summary>
        public readonly float FractionClamped = 1;

        /// <summary>
        /// The sweep's target velocity.
        /// </summary>
        public readonly Vector2 OriginalVelocity = Vector2.Zero;

        /// <summary>
        /// The furthest distance the sweep was able to move before a collision.
        /// </summary>
        public readonly Vector2 AdjustedVelocity = Vector2.Zero;

        /// <summary>
        /// The furthest distance the sweep was able to move before a collision, clamped to not move backwards.
        /// </summary>
        public readonly Vector2 AdjustedVelocityClamped = Vector2.Zero;

        /// <summary>
        /// The surface normal of the collided face.
        /// </summary>
        public readonly Vector2 Normal = Vector2.UnitY;

        internal SweepResult(bool collided, float fraction, Vector2 normal, Vector2 velocity) {
            Collided = collided;
            OriginalVelocity = velocity;
            Normal = normal;

            Fraction = fraction;
            AdjustedVelocity = OriginalVelocity * Fraction;

            FractionClamped = Calc.Clamp(Fraction);
            AdjustedVelocityClamped = OriginalVelocity * FractionClamped;
        }

        internal SweepResult(JDASweep sweep)
            // Assuming collider A because manual sweep checks will order the colliders correctly
            //  Need to use a more robust setup if this is used for an eventual broadphase
            : this(sweep.Collided, sweep.Fraction, sweep.Normal, sweep.Ctx.A.Velocity) { }
    }

    /// <summary>
    /// If the collision collided with anything.
    /// </summary>
    public readonly bool Collided = false;

    readonly Data data = new();

    /// <summary>
    /// The amount to move the other collider to seperate, if applicable.
    /// </summary>
    public readonly Vector2 Pushout => data.Pushout;

    /// <summary>
    /// The sweep information for the collision, if applicable.
    /// </summary>
    public readonly SweepResult Sweep => data.Sweep;

    /// <summary>
    /// All intersected collision components.
    /// </summary>
    public readonly ComponentResult[] CollidedComponents = [];

    internal CollisionResult(ComponentResult[] collidedComponents) {
        if (collidedComponents.Length > 0) {
            Collided = true;
            // Results should already be pre-sorted so the first result can be taken
            data = collidedComponents[0].data;
        }
        CollidedComponents = collidedComponents;
    }

    // Fallback for sweep collision that hits nothing
    internal CollisionResult(SweepResult sweep) {
        Collided = sweep.Collided;
        data = new(sweep);
    }
}