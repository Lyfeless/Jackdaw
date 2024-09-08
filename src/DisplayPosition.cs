using System.Numerics;
using Foster.Framework;

namespace LittleLib;

/// <summary>
/// Position control that stores both an exact location and a location rounded to the nearest int,
/// to avoid any floating point imprecision when rendering
/// </summary>
public class RenderablePosition {
    public Vector2 Precise { get; private set; }
    public Point2 Rounded { get; private set; }

    public RenderablePosition(Vector2 position) {
        Set(position);
    }

    /// <summary>
    /// Move current position by amount.
    /// </summary>
    /// <param name="amount">The amount to move the position relative to its current position.</param>
    public void Change(Vector2 amount) {
        Set(Precise + amount);
    }

    /// <summary>
    /// Move current position by angle and magnitude.
    /// </summary>
    /// <param name="angle">Angle to move at, in radians.</param>
    /// <param name="amount">Distance to move.</param>
    public void Change(float angle, float amount) {
        Change(Calc.AngleToVector(angle, amount));
    }

    /// <summary>
    /// Set position.
    /// </summary>
    /// <param name="value">Target position.</param>
    public void Set(Vector2 value) {
        Precise = value;
        Rounded = new((int)Math.Round(value.X), (int)Math.Round(value.Y));
    }
}