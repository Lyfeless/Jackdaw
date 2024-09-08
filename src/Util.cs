
using System.Numerics;
using Foster.Framework;

namespace LittleLib;

/// <summary>
/// Utility functions that don't neatly fit into any other classes
/// </summary>
public static class Util {
    public static Rng random = new(DateTime.Now.Millisecond);

    public static bool MouseMoved() {
        return Input.Mouse.Position != Input.LastState.Mouse.Position;
    }

    public static Vector2 MouseDelta() {
        return Input.Mouse.Position - Input.LastState.Mouse.Position;
    }
}