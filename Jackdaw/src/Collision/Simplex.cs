using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

internal struct GJKSimplex {
    public readonly ColliderContextPair Ctx;

    public readonly Vector2[] Points = new Vector2[3];
    int addIndex = 0;

    public readonly Vector2 A => Points[0];
    public readonly Vector2 B => Points[1];
    public readonly Vector2 C => Points[2];

    public readonly bool Collided = false;

    public GJKSimplex(ColliderContextPair ctx) {
        Ctx = ctx;

        Vector2 direction = InitialDirection();
        if (!Add(direction)) { return; }

        direction = -direction;
        if (!Add(direction)) { return; }

        direction = Vector2.PerpendicularToward(A, B, Vector2.Zero);
        if (!Add(direction)) { return; }

        // Adjust points until they cover the origin
        while (true) {
            Vector2 perpCB = Vector2.PerpendicularToward(C, B, Vector2.Zero);
            Vector2 perpCA = Vector2.PerpendicularToward(C, A, Vector2.Zero);
            if (perpCB.OppositeDirection(A)) {
                SetA(B);
                SetB(C);

                direction = perpCB;
                if (!Add(direction)) { return; }
            }
            else if (perpCA.OppositeDirection(B)) {
                SetB(C);

                direction = perpCA;
                if (!Add(direction)) { return; }
            }
            else {
                Collided = true;
                return;
            }
        }
    }

    public bool Add(Vector2 direction) {
        Vector2 newPoint = Ctx.Support(direction);
        SetNext(newPoint);
        return PointCrossesOrigin(newPoint, direction);
    }

    void SetA(Vector2 value) => Points[0] = value;
    void SetB(Vector2 value) => Points[1] = value;
    void SetC(Vector2 value) => Points[2] = value;
    void SetNext(Vector2 value) {
        Points[addIndex] = value;
        if (addIndex < 2) { addIndex++; }
    }

    readonly Vector2 InitialDirection() => TransformedCenter(Ctx.B) - TransformedCenter(Ctx.A);
    static Vector2 TransformedCenter(ColliderContext collider) => Vector2.Transform(collider.Collider.Center, collider.Position.Matrix);

    static bool PointCrossesOrigin(Vector2 point, Vector2 direction) => point.SameDirectionInclusive(direction);
}