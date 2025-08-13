using System.Numerics;
using Foster.Framework;

namespace LittleLib;

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
    /// <param name="bounds">The bounds to check collisions against.</param>
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
    /// The gjk support function for the shape.
    /// </summary>
    /// <param name="position">The collider's global position</param>
    /// <param name="direction">the ray direction vector.</param>
    /// <returns>The nearest support point in the given direction.</returns>
    public abstract Vector2 Support(Vector2 position, Vector2 direction);
}