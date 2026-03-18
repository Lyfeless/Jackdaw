using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

public readonly struct CollisionResult {
    public readonly struct ComponentResult {
        public readonly CollisionComponent Component;
        public readonly ColliderResult[] Colliders = [];

        internal readonly Data data = new();
        public readonly Vector2 Pushout => data.Pushout;
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

    public readonly struct ColliderResult {
        public readonly Collider Collider;

        internal readonly Data data = new();
        public readonly Vector2 Pushout => data.Pushout;
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
        const float SEPERATION_AMOUNT = 0.0001f;

        public readonly Vector2 Pushout = Vector2.Zero;
        public readonly SweepResult Sweep = new();

        internal Data(EPAPushout pushout) : this(pushout.Collided, pushout.Pushout) { }
        internal Data(bool collided, Vector2 pushout) {
            Pushout = pushout;
            // Apply a small amount of additional pushout to stop objects getting stuck
            if (collided) {
                Pushout += Pushout.Normalized() * SEPERATION_AMOUNT;
            }
        }

        internal Data(JDASweep sweep) : this(new SweepResult(sweep)) { }
        internal Data(SweepResult sweep) {
            Sweep = sweep;
            if (Sweep.Fraction < 0) { Pushout = Sweep.AdjustedVelocity; }
        }
    }

    public readonly struct SweepResult {
        const float SEPERATION_AMOUNT = 0.0001f;

        public readonly bool Collided = false;
        public readonly float Fraction = 1;
        public readonly float FractionClamped = 1;
        public readonly Vector2 OriginalVelocity = Vector2.Zero;
        public readonly Vector2 AdjustedVelocity = Vector2.Zero;
        public readonly Vector2 AdjustedVelocityClamped = Vector2.Zero;
        public readonly Vector2 Normal = Vector2.UnitY;

        internal SweepResult(bool collided, float fraction, Vector2 normal, Vector2 velocity) {
            Collided = collided;
            OriginalVelocity = velocity;
            Normal = normal;

            Fraction = fraction;
            AdjustedVelocity = OriginalVelocity * Fraction;

            // Apply a small amount of additional pushout to stop objects getting stuck
            if (Collided) {
                AdjustedVelocity -= AdjustedVelocity.Normalized() * SEPERATION_AMOUNT;
                Fraction = JDASweep.VelocityFraction(AdjustedVelocity, OriginalVelocity);
            }

            FractionClamped = Calc.Clamp(Fraction);
            AdjustedVelocityClamped = OriginalVelocity * FractionClamped;
        }

        internal SweepResult(JDASweep sweep)
            // Assuming collider A because manual sweep checks will order the colliders correctly
            //  Need to use a more robust setup if this is used for an eventual broadphase
            : this(sweep.Collided, sweep.Fraction, sweep.Normal, sweep.Ctx.A.Velocity) { }
    }


    public readonly bool Collided = false;

    readonly Data data = new();
    public readonly Vector2 Pushout => data.Pushout;
    public readonly SweepResult Sweep => data.Sweep;

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