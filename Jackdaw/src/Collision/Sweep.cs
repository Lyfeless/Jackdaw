using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

internal struct JDASweep {
    const int ITERATION_LIMIT = 32;
    public const float TOLERANCE = 0.0001f;

    public readonly ColliderContextPair Ctx;

    public float Fraction = 1;
    public Vector2 Normal = Vector2.UnitY;
    public bool Collided = false;

    public readonly bool IsFractionNegative => Fraction < 0;

    public JDASweep(ColliderContextPair ctx) {
        Ctx = ctx;

        // Get velocity difference
        Vector2 velocityDifference = ctx.B.Velocity - ctx.A.Velocity;

        // Cancel if objects are moving the same direction or stationary
        if (velocityDifference == Vector2.Zero) { return; }

        float velocityLength = velocityDifference.LengthSquared();

        // Point A
        Vector2 direction = -velocityDifference;
        Vector2 pointA = ctx.Support(direction);

        // Point B
        direction = Calc.TripleProduct(direction, -pointA, direction);
        Vector2 pointB = ctx.Support(direction);

        float crossA = Calc.Cross(velocityDifference, pointA);
        float crossB = Calc.Cross(velocityDifference, pointB);
        if (MathF.Sign(crossA) == MathF.Sign(crossB)) {
            // First line lies exactly on velocity vector
            if (MathF.Abs(crossA) < float.Epsilon && MathF.Abs(crossB) < float.Epsilon) {
                if (SweepInfiniteCollisionResolve(pointA, pointB, velocityLength, out Vector2 intersection)) {
                    Fraction = VelocityFraction(intersection, velocityDifference);
                    Normal = -velocityDifference.Normalized();
                    Collided = true;
                }
            }

            return;
        }

        for (int i = 0; i < ITERATION_LIMIT; ++i) {
            // Get new point C
            direction = PerpDirection(pointA, pointB, -velocityDifference);
            // Exit immediately if the direction has no magnitude (lies exactly on the line AB)
            if (direction.LengthSquared() < float.Epsilon) {
                SweepLineIntersection(pointA, pointB, velocityDifference, velocityLength);
                return;
            }
            Vector2 pointC = ctx.Support(direction);

            // New support point is the same as one of the current points
            if (Vector2.DistanceSquared(pointC, pointA) < TOLERANCE || Vector2.DistanceSquared(pointC, pointB) < TOLERANCE) {
                SweepLineIntersection(pointA, pointB, velocityDifference, velocityLength);
                return;
            }

            float crossC = Calc.Cross(velocityDifference, pointC);

            // pointC lies exactly on the line
            if (MathF.Abs(crossC) < float.Epsilon) {
                Fraction = VelocityFraction(pointC, velocityDifference);
                Normal = PerpDirection(pointA, pointB, -velocityDifference);
                Collided = true;
            }

            if (MathF.Sign(crossC) == MathF.Sign(crossA)) {
                pointA = pointC;
                crossA = crossC;
            }
            else {
                pointB = pointC;
                crossB = crossC;
            }
        }

        SweepLineIntersection(pointA, pointB, velocityDifference, velocityLength);
    }

    public static float VelocityFraction(Vector2 value, Vector2 max)
        // Fraction should be the same on either axis so just use the one that will give more precision
        => Math.Abs(max.X) > Math.Abs(max.Y) ? FloatFraction(value.X, max.X) : FloatFraction(value.Y, max.Y);

    static float FloatFraction(float value, float max) => (max == 0) ? 0 : (value / max);

    // Returns squared length for optimization
    // Expects direction to be normalized
    static float SweepIntersectionLength(Vector2 intersection, Vector2 direction) {
        float distance = intersection.LengthSquared();
        return Vector2.Dot(intersection.Normalized(), direction) > 0 ? distance : -distance;
    }

    // Expects velocityLength to be a squared distance
    static bool SweepInfiniteCollisionResolve(Vector2 pointA, Vector2 pointB, float velocityLength, out Vector2 intersection) {
        float distA = pointA.LengthSquared();
        float distB = pointB.LengthSquared();
        float dist;
        if (distA < distB) {
            dist = distA;
            intersection = pointA;
        }
        else {
            dist = distB;
            intersection = pointB;
        }

        return dist < velocityLength;
    }

    // Expects velocityLength to be a squared distance
    void SweepLineIntersection(Vector2 pointA, Vector2 pointB, Vector2 velocity, float velocityLength) {
        if (!new Line(Vector2.Zero, velocity).IntersectionLineSegment(new(pointA, pointB), out Vector2 intersection)) { return; }

        Normal = PerpDirection(pointA, pointB, velocity).Normalized();

        // NaN is returned if the lines are the same
        if (intersection == Vector2.NaN) {
            if (!SweepInfiniteCollisionResolve(pointA, pointB, velocityLength, out intersection)) {
                Normal = -velocity;
                return;
            }
        }
        else {
            if (SweepIntersectionLength(intersection, velocity) > velocityLength) { return; }
        }

        Fraction = VelocityFraction(intersection, velocity);
        Collided = true;
    }

    static Vector2 PerpDirection(Vector2 pointA, Vector2 pointB, Vector2 facingDirection) {
        Vector2 line = pointB - pointA;
        Vector2 direction = Calc.TripleProduct(line, pointA, line);
        if (Vector2.Dot(direction, facingDirection) < 0) { direction = -direction; }
        return direction;
    }
}