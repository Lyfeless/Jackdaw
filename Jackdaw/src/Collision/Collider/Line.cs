using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A collider represanting a single line segment, defined with two points.
/// </summary>
/// <param name="point1">The line start position.</param>
/// <param name="point2">The line end position.</param>
public class LineCollider(Vector2 point1, Vector2 point2) : Collider {
    Vector2 point1 = point1;
    Vector2 point2 = point2;

    /// <summary>
    /// The line start position.
    /// </summary>
    public Vector2 Point1 {
        get => point1;
        set {
            point1 = value;
            bounds = GetBounds(point1, point2);
        }
    }

    /// <summary>
    /// The line end position.
    /// </summary>
    public Vector2 Point2 {
        get => point2;
        set {
            point2 = value;
            bounds = GetBounds(point1, point2);
        }
    }

    Rect bounds = GetBounds(point1, point2);
    public override Rect Bounds => bounds;
    public override Vector2 Center => CalcExtra.LineMidpoint(Point1, Point2);

    public override bool Multi => false;
    public override Collider[] GetSubColliders(Rect bounds) => [this];

    public override Vector2 Support(Vector2 direction) => Vector2.Dot(point2 - point1, direction) < 0 ? point1 : point2;

    static Rect GetBounds(Vector2 point1, Vector2 point2) => Rect.Between(point1, point2);
}