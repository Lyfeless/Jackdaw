using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A collider built for handling collisions between objects.
/// </summary>
public abstract class Collider {
    /// <summary>
    /// The collisions tags the collider has.
    /// Leave empty to act as any tag.
    /// </summary>
    public TagContainer Tags = new();

    /// <summary>
    /// The collision tags the collider will collider with.
    /// Leave empty to collide with all tags.
    /// </summary>
    public TagContainer Mask = new();

    /// <summary>
    /// If the collider is made up of multiple smaller colliders.
    /// </summary>
    public abstract bool Multi { get; }

    /// <summary>
    /// Return all useful subcolliders for a given local bound.
    /// </summary>
    /// <param name="bounds">The bounds to check collisions against, local to the collider.</param>
    /// <returns>The relevant subcolliders for the bounds.</returns>
    public abstract Collider[] GetSubColliders(Rect bounds);

    /// <summary>
    /// The total bounds the collider covers.
    /// </summary>
    public abstract Rect Bounds { get; }

    /// <summary>
    /// The center of the collider.
    /// </summary>
    public abstract Vector2 Center { get; }

    /// <summary>
    /// The GJK support function for the shape.
    /// </summary>
    /// <param name="direction">The ray direction vector.</param>
    /// <param name="position">The collider's global transformation.</param>
    /// <returns>The nearest support point in the given direction.</returns>
    public abstract Vector2 Support(Vector2 direction, InvertableMatrix position);

    /// <summary>
    /// Transform the direction into the collider's local space.
    /// </summary>
    /// <param name="direction">The direction to transform.</param>
    /// <param name="position">The collider's global position.</param>
    /// <returns>The direction transformed to be local to the collider.</returns>
    protected static Vector2 GetLocalDirection(Vector2 direction, InvertableMatrix position)
        => Vector2.TransformNormal(direction, position.MatrixInverse);

    /// <summary>
    /// Transform the point from the collider's local space back into world space.
    /// </summary>
    /// <param name="point">The point to transform.</param>
    /// <param name="position">The collider's global position.</param>
    /// <returns>The point in global space.</returns>
    protected static Vector2 GetGlobalPoint(Vector2 point, InvertableMatrix position)
        => Vector2.Transform(point, position.Matrix);
}