using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A grid with a position and tile size.
/// </summary>
public interface ISpatialGrid {
    /// <summary>
    /// The position of the entire grid.
    /// </summary>
    public Vector2 Position { get; set; }

    /// <summary>
    /// The width and height of an individual tile.
    /// </summary>
    public Vector2 TileSize { get; protected set; }

    /// <summary>
    /// The bounding box for the entire grid.
    /// </summary>
    public Rect Bounds { get; }
}