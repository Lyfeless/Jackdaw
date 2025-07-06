using System.Numerics;

namespace LittleLib;

//! FIXME (Alex): Switch to Calc extension with .net 10 comes out of preview
public static class CalcExtra
{
    public static Vector2 TripleProduct(Vector2 a, Vector2 b, Vector2 c)
    {
        Vector3 a3 = new(a.X, a.Y, 0);
        Vector3 b3 = new(b.X, b.Y, 0);
        Vector3 c3 = new(c.X, c.Y, 0);

        Vector3 first = Vector3.Cross(a3, b3);
        Vector3 second = Vector3.Cross(first, c3);

        return new(second.X, second.Y);
    }

    public static Vector2 LineMidpoint(Vector2 a, Vector2 b)
    {
        return ((a - b) / 2) + b;
    }

    public static bool LineSegmentIntersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 intersection)
    {
        float t = (((a1.X - b1.X) * (b1.Y - b2.Y)) - ((a1.Y - b1.Y) * (b1.X - b2.X))) / (((a1.X - a2.X) * (b1.Y - b2.Y)) - ((a1.Y - a2.Y) * (b1.X - b2.X)));
        float u = -((((a1.X - a2.X) * (a1.Y - b1.Y)) - ((a1.Y - a2.Y) * (a1.X - b1.X))) / (((a1.X - a2.X) * (b1.Y - b2.Y)) - ((a1.Y - a2.Y) * (b1.X - b2.X))));
        intersection = new(a1.X + (t * (a2.X - a1.X)), a1.Y + (t * (a2.Y - a1.Y)));
        return 0 <= t && t <= 1 && 0 <= u && u <= 1;
    }
}