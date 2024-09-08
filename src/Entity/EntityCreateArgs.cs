using System.Numerics;
using Foster.Framework;

namespace LittleLib;

/// <summary>
/// Container for generic spawn information for newly created entities.
/// </summary>
/// <remarks>
/// This structure is mostly a cleanliness feature, allowing new entities to be made without needing to manually create save file info or manually modifying every constructor if new generic data is needed.
/// </remarks>
public struct EntityCreateArgs() {
    public Vector2 Position = Vector2.Zero;
    public string InstanceID = Guid.NewGuid().ToString();
    public bool SkipRegister = false;
    public string? TimeTracker = null;
    public Point2 Size = Point2.Zero;
}