using System.Numerics;

namespace Jackdaw;

/// <summary>
/// Information for how a sprite should be flipped when displayed.
/// </summary>
/// <param name="x">If the sprite should flip on the x axis.</param>
/// <param name="y">If the sprite should flip on the y axis.</param>
public readonly struct SpriteFlip(bool x, bool y) {
    /// <summary>
    /// If the sprite should flip on the x axis.
    /// </summary>
    public readonly bool X = x;

    /// <summary>
    /// If the sprite should flip on the y axis.
    /// </summary>
    public readonly bool Y = y;

    /// <summary>
    /// Information for how a sprite should be flipped when displayed.
    /// </summary>
    /// <param name="value">If the sprite should be flipped on the x and y axis.</param>
    public SpriteFlip(bool value) : this(value, value) { }

    /// <summary>
    /// Information for how a sprite should be flipped when displayed.
    /// </summary>
    public SpriteFlip() : this(false, false) { }

    /// <summary>
    /// Get the scale vector required to flip the current sprite.
    /// </summary>
    /// <returns>The amount to scale the sprite to get the desired flip.</returns>
    public Vector2 GetScale() => GetScaleOf(X, Y);

    /// <summary>
    /// Get the scale vector required to flip a sprite with the given parameters.
    /// </summary>
    /// <param name="flipX">If the sprite should flip on the x axis.</param>
    /// <param name="flipY">If the sprite should flip on the y axis.</param>
    /// <returns>The amount to scale the sprite to get the desired flip.</returns>
    public static Vector2 GetScaleOf(bool flipX, bool flipY)
        => new(flipX ? -1 : 1, flipY ? -1 : 1);

    /// <summary>
    /// Returns a copy of the flip with a given x value.
    /// </summary>
    /// <param name="x">The x value for the flip.</param>
    /// <returns>A flip with the new state.</returns>
    public SpriteFlip WithX(bool x) => new(x, Y);

    /// <summary>
    /// Returns a copy of the flip with an inverted x value.
    /// </summary>
    /// <returns>A flip with the new state.</returns>
    public SpriteFlip WithInvertedX() => new(!X, Y);

    /// <summary>
    /// Returns a copy of the flip with a given y value.
    /// </summary>
    /// <param name="y">The y value for the flip.</param>
    /// <returns>A flip with the new state.</returns>
    public SpriteFlip WithY(bool y) => new(X, y);

    /// <summary>
    /// Returns a copy of the flip with an inverted y value.
    /// </summary>
    /// <returns>A flip with the new state.</returns>
    public SpriteFlip WithInvertedY() => new(X, !Y);

    /// <summary>
    /// Returns a copy of the flip with both the x and y axis inverted.
    /// </summary>
    /// <returns>A flip with the new state.</returns>
    public SpriteFlip WithBothInverted() => new(!X, !Y);
}