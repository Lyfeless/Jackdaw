using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

internal struct GJKSimplex {
    readonly ColliderContextPair Ctx;

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

        Vector2 AInv = -A;
        Vector2 AB = B + AInv;
        direction = Calc.TripleProduct(AB, AInv, AB);
        if (!Add(direction)) { return; }

        // Adjust points until they cover the origin
        while (true) {
            Vector2 CInv = -C;
            Vector2 CB = B + CInv;
            Vector2 CA = A + CInv;
            Vector2 perpCB = Calc.TripleProduct(CA, CB, CB);
            Vector2 perpCA = Calc.TripleProduct(CB, CA, CA);
            if (Vector2.Dot(perpCB, CInv) > 0) {
                SetA(B);
                SetB(C);

                direction = perpCB;
                if (!Add(direction)) { return; }
            }
            else if (Vector2.Dot(perpCA, CInv) > 0) {
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
    static Vector2 TransformedCenter(ColliderContext collider) => Vector2.Transform(collider.Collider.Center, collider.Position);

    static bool PointCrossesOrigin(Vector2 point, Vector2 direction) => Vector2.Dot(direction, point) >= 0;
}