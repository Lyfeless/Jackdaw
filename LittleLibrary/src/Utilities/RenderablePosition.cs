using System.Numerics;
using Foster.Framework;

namespace LittleLib;

/// <summary>
/// Position control that stores both an exact location and a location rounded to the nearest int,
/// to avoid any floating point imprecision when rendering
/// </summary>
public struct RenderablePosition {
    public Vector2 Precise { get; private set; }
    public Point2 Rounded { get; private set; }

    public RenderablePosition(Vector2 position) {
        Set(position);
    }

    public RenderablePosition() : this(Vector2.Zero) { }

    /// <summary>
    /// Move current position by amount.
    /// </summary>
    /// <param name="amount">The amount to move the position relative to its current position.</param>
    public void Change(Vector2 amount) {
        Set(Precise + amount);
    }

    /// <summary>
    /// Move current position by amount.
    /// </summary>
    /// <param name="x">The amount to move on the x axis.</param>
    /// <param name="y">The amount to move on the y axis.</param>
    public void Change(float x, float y) => Change(new(x, y));
    /// <summary>
    /// Move current position along the x axis by amount.
    /// </summary>
    /// <param name="x">The amount to move on the x axis.</param>
    public void ChangeX(float x) => Change(new(x, 0));
    /// <summary>
    /// Move current position along the y axis by amount.
    /// </summary>
    /// <param name="y">The amount to move on the y axis.</param>
    public void ChangeY(float y) => Change(new(0, y));

    /// <summary>
    /// Move current position by angle and magnitude.
    /// </summary>
    /// <param name="angle">Angle to move at, in radians.</param>
    /// <param name="amount">Distance to move.</param>
    public void ChangeByAngle(float angle, float amount) => Change(Calc.AngleToVector(angle, amount));

    /// <summary>
    /// Set position.
    /// </summary>
    /// <param name="value">Target position.</param>
    public void Set(Vector2 value) {
        Precise = value;
        Rounded = new((int)Math.Round(value.X), (int)Math.Round(value.Y));
    }

    /// <summary>
    /// Set position.
    /// </summary>
    /// <param name="x">Target x position.</param>
    /// <param name="y">Target y position.</param>
    public void Set(float x, float y) => Set(new(x, y));
    /// <summary>
    /// Set x position.
    /// </summary>
    /// <param name="value">Target x position.</param>
    public void SetX(float value) => Set(value, Precise.Y);
    /// <summary>
    /// Set y position.
    /// </summary>
    /// <param name="value">Target y position.</param>
    public void SetY(float value) => Set(Precise.X, value);

    public static RenderablePosition operator +(RenderablePosition a, RenderablePosition b) => new(a.Precise + b.Precise);
    public static RenderablePosition operator +(RenderablePosition a, Vector2 b) => new(a.Precise + b);
    public static RenderablePosition operator +(Vector2 a, RenderablePosition b) => new(a + b.Precise);

    public static RenderablePosition operator -(RenderablePosition a) => new(-a.Precise);
    public static RenderablePosition operator -(RenderablePosition a, RenderablePosition b) => a + (-b);
    public static RenderablePosition operator -(RenderablePosition a, Vector2 b) => a + (-b);
    public static RenderablePosition operator -(Vector2 a, RenderablePosition b) => a + (-b);

    public static RenderablePosition operator *(RenderablePosition a, Vector2 b) => new(a.Precise * b);
    public static RenderablePosition operator *(Vector2 a, RenderablePosition b) => new(a * b.Precise);
    public static RenderablePosition operator *(RenderablePosition a, RenderablePosition b) => new(a.Precise * b.Precise);

    public static RenderablePosition operator /(RenderablePosition a, RenderablePosition b) => new(a.Precise / b.Precise);
    public static RenderablePosition operator /(RenderablePosition a, Vector2 b) => new(a.Precise / b);
    public static RenderablePosition operator /(Vector2 a, RenderablePosition b) => new(a / b.Precise);
}