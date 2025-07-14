using System.Numerics;
using Foster.Framework;

namespace LittleLib;

//! FIXME (Alex): Switch to interface so these can be structs?
public interface ICollider {
    public bool Multi { get; }

    // Return all useful subcolliders for a given bound, local to collider
    public ICollider[] GetSubColliders(Rect bounds);

    public Rect Bounds { get; }
    public Vector2 Center { get; }

    public Vector2 Support(Vector2 position, Vector2 direction);
}