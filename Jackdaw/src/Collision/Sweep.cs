using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

internal struct JDASweep {
    const int ITERATION_LIMIT = 32;
    public const float TOLERANCE = 0.0001f;
    const float SEPERATION_AMOUNT = 0.01f;

    public readonly ColliderContextPair Ctx;

    public float Fraction = 1;
    public Vector2 Normal = Vector2.UnitY;
    public bool Collided = false;

    public readonly bool IsFractionNegative => Fraction < 0;

    public JDASweep(ColliderContextPair ctx) {
        Ctx = ctx;

        // Get velocity difference
        Vector2 velocityDifference = ctx.B.Velocity - ctx.A.Velocity;
        Vector2 velocityNormal = velocityDifference.Normalized();

        // Cancel if objects are moving the same direction or stationary
        if (velocityDifference == Vector2.Zero) { return; }

        // Point A
        Vector2 direction = -velocityDifference;
        Vector2 pointA = ctx.Support(direction);

        // Point B
        direction = ClosestUnclamped(pointA, velocityDifference);
        Vector2 pointB = ctx.Support(direction);

        float crossA = Calc.Cross(velocityNormal, pointA.Normalized());
        float crossB = Calc.Cross(velocityNormal, pointB.Normalized());

        if (!CrossesVelocity(crossA, crossB)) { return; }

        for (int i = 0; i < ITERATION_LIMIT; ++i) {
            // Points lie exactly on the line
            if (ApproxZero(crossA, crossB)) {
                Set(CloserToZero(pointA, pointB), -velocityNormal, velocityDifference);
                return;
            }

            // Get new point C
            direction = PerpDirection(pointA, pointB, -velocityDifference);

            Vector2 pointC = ctx.Support(direction);

            // New support point is the same as one of the current points
            if (ApproxEither(pointA, pointB, pointC)) {
                Set(pointA, pointB, direction.Normalized(), velocityDifference);
                return;
            }

            float crossC = Calc.Cross(velocityNormal, pointC.Normalized());

            if (MathF.Sign(crossC) == MathF.Sign(crossA)) {
                pointA = pointC;
                crossA = crossC;
            }
            else {
                pointB = pointC;
                crossB = crossC;
            }
        }

        Set(pointA, pointB, direction.Normalized(), velocityDifference);
    }

    public void Set(Vector2 a, Vector2 b, Vector2 normal, Vector2 velocity)
        => Set(Intersection(a, b, velocity), normal, velocity);

    public void Set(Vector2 intersection, Vector2 normal, Vector2 velocity) {
        float frac = VelocityFraction(intersection, velocity);

        // Cancel if sweep is trying to push backwards when objects didn't initally collide
        if (frac < 0 && !new GJKSimplex(Ctx).Collided) {
            return;
        }

        // Apply a small amount of additional pushout to stop objects getting stuck
        if (FractionCollided(frac)) {
            frac = VelocityFraction(ApplySeperation(intersection, normal), velocity);
        }

        Fraction = Math.Min(frac, 1);
        Collided = FractionCollided(frac);
        Normal = normal;
    }

    static Vector2 ApplySeperation(Vector2 point, Vector2 normal) {
        Vector2 pointNormal = point.Normalized();
        Vector2 seperation = pointNormal * (SEPERATION_AMOUNT / Vector2.Dot(normal, pointNormal));
        return point + seperation;
    }

    static bool FractionCollided(float fraction) => fraction <= 1;

    static bool CrossesVelocity(float crossA, float crossB) {
        int signA = Math.Sign(crossA);
        int signB = Math.Sign(crossB);
        return Math.Max(signA, signB) >= 0 && Math.Min(signA, signB) < 0;
    }

    public static float VelocityFraction(Vector2 value, Vector2 max)
        // Fraction should be the same on either axis so just use the one that will give more precision
        => Math.Abs(max.X) > Math.Abs(max.Y) ? FloatFraction(value.X, max.X) : FloatFraction(value.Y, max.Y);

    static float FloatFraction(float value, float max) => (max == 0) ? 0 : (value / max);

    static Vector2 Intersection(Vector2 a, Vector2 b, Vector2 velocity) {
        float denominator = ((a.X - b.X) * velocity.Y) - ((a.Y - b.Y) * velocity.X);
        if (denominator == 0) { return CloserToZero(a, b); }
        return new(((a.X * b.Y) - (a.Y * b.X)) * velocity.X / denominator, ((a.X * b.Y) - (a.Y * b.X)) * velocity.Y / denominator);
    }

    static bool ApproxEither(Vector2 a, Vector2 b, Vector2 newPoint)
        => ApproxEqual(a, newPoint) || ApproxEqual(b, newPoint);

    static bool ApproxEqual(Vector2 a, Vector2 b) => Vector2.DistanceSquared(a, b) < TOLERANCE;

    static bool ApproxZero(float cross) => MathF.Abs(cross) < TOLERANCE;

    static bool ApproxZero(float crossA, float crossB)
    => ApproxZero(crossA) && ApproxZero(crossB);

    static Vector2 CloserToZero(Vector2 a, Vector2 b)
        => a.LengthSquared() < b.LengthSquared() ? a : b;

    static Vector2 PerpDirection(Vector2 pointA, Vector2 pointB, Vector2 velocity) {
        Vector2 perp = (pointB - pointA).TurnLeft();
        if (Vector2.Dot(perp, velocity) < 0) { perp = -perp; }
        return perp;
    }

    static Vector2 ClosestUnclamped(Vector2 point, Vector2 velocity) {
        Line line = new(Vector2.Zero, velocity);
        return line.On(line.ClosestTUnclamped(point)) - point;
    }
}