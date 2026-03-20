using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

internal struct EPAPushout {
    const float SEPERATION_AMOUNT = 0.01f;

    readonly struct Edge {
        public readonly Vector2 From;
        public readonly Vector2 To;
        public readonly Vector2 Normal;
        public readonly float Distance;

        public Edge(Vector2 from, Vector2 to, Vector2 normal, float distance) {
            From = from;
            To = to;
            Normal = normal;
            Distance = distance;
        }

        public Edge(Vector2 from, Vector2 to, bool clockwise) {
            From = from;
            To = to;
            Vector2 diff = to - from;
            Normal = (clockwise ? diff.TurnLeft() : diff.TurnRight()).Normalized();
            Distance = Vector2.Dot(Normal, from);
        }
    }

    const int ITERATION_LIMIT = 32;
    const float TOLERANCE = 0.0001f;

    public readonly ColliderContextPair Ctx;

    public readonly Vector2 Pushout = Vector2.Zero;
    public readonly bool Collided = false;

    public EPAPushout(ColliderContextPair ctx) : this(ctx, new GJKSimplex(ctx)) { }

    public EPAPushout(ColliderContextPair ctx, GJKSimplex simplex) {
        Ctx = ctx;

        if (!simplex.Collided) { return; }

        Collided = true;

        bool isClockwise = IsClockwiseWinding(simplex.A, simplex.B, simplex.C);

        List<Edge> edges = [
            new(simplex.A, simplex.B, isClockwise),
            new(simplex.B, simplex.C, isClockwise),
            new(simplex.C, simplex.A, isClockwise)
        ];

        for (int i = 0; i < ITERATION_LIMIT; ++i) {
            int closestIndex = GetClosestIndex(edges);
            Edge closestEdge = edges[closestIndex];

            Vector2 direction = closestEdge.Normal;
            Vector2 newSupport = ctx.Support(direction);
            float supportDistance = Vector2.Dot(newSupport, direction);

            Pushout = -supportDistance * direction;

            // Return immediately if pushout is as close as possible
            supportDistance = MathF.Abs(supportDistance - closestEdge.Distance);
            if (supportDistance <= TOLERANCE) {
                Pushout = ApplySeperation(Pushout);
                return;
            }

            edges[closestIndex] = new(closestEdge.From, newSupport, isClockwise);
            edges.Add(new(newSupport, closestEdge.To, isClockwise));
        }
        Pushout = ApplySeperation(Pushout);
    }

    static bool IsClockwiseWinding(Vector2 a, Vector2 b, Vector2 c) {
        float wind0 = (b.X - a.X) * (b.Y + a.Y);
        float wind1 = (c.X - b.X) * (c.Y + b.Y);
        float wind2 = (a.X - c.X) * (a.Y + c.Y);
        return wind0 + wind1 + wind2 <= 0;
    }

    static int GetClosestIndex(in List<Edge> edges) {
        int closestEdge = 0;
        for (int i = 1; i < edges.Count; ++i) {
            if (edges[i].Distance < edges[closestEdge].Distance) { closestEdge = i; }
        }
        return closestEdge;
    }

    static Vector2 ApplySeperation(Vector2 pushout)
        => pushout + (pushout.Normalized() * SEPERATION_AMOUNT);
}