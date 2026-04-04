using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

public static class Vector2Extensions {
    extension(Vector2 v) {
        public float Dot(Vector2 other) => Vector2.Dot(v, other);

        public bool SameDirection(Vector2 other) => v.Dot(other) > 0;

        public bool SameDirectionInclusive(Vector2 other) => v.Dot(other) >= 0;

        public bool OppositeDirection(Vector2 other) => !v.SameDirectionInclusive(other);

        public bool OppositeDirectionInclusive(Vector2 other) => !v.SameDirection(other);

        public static Vector2 PerpendicularAway(Vector2 a, Vector2 b, Vector2 target)
        => -PerpendicularToward(a, b, target);

        public static Vector2 PerpendicularToward(Vector2 a, Vector2 b, Vector2 target) {
            Vector2 perp = (a - b).TurnLeft();
            if (OppositeDirection(perp, target - a)) { return -perp; }
            return perp;
        }

        public Vector2 Rotate(float by) => Calc.AngleToVector(v.Angle() + by, v.Length());
    }
}