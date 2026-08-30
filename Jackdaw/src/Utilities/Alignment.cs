using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Information for aligning a 1-dimensional element.
/// </summary>
/// <param name="percent">
///     The alignment's percent offset along the element's size. <br/>
///     0 aligns to the low end of the size, and 1 aligns to the high end.
/// </param>
public readonly struct AlignmentRange(float percent = 0) {
    /// <summary>
    /// Returns an alignment that aligns to the lowest point of an element's size.
    /// </summary>
    /// <returns>An alignment with the given state.</returns>
    public static AlignmentRange Low() => new();

    /// <summary>
    /// Returns an alignment that aligns to the highest point of an element's size.
    /// </summary>
    /// <returns>An alignment with the given state.</returns>
    public static AlignmentRange High() => new(1);

    /// <summary>
    /// Returns an alignment that aligns to the center of the element's size.
    /// </summary>
    /// <returns>An alignment with the given state.</returns>
    public static AlignmentRange Center() => new(0.5f);

    /// <summary>
    ///     The alignment's percent offset along the element's size. <br/>
    ///     0 aligns to the low end of the size, and 1 aligns to the high end.
    /// </summary>
    public readonly float Percent = percent;

    /// <summary>
    /// Get the offset value to align the given element size.
    /// </summary>
    /// <param name="size">The size of the element.</param>
    /// <returns>The amount to offset the element to fit the set alignment.</returns>
    public readonly float Get(float size) => -size * Percent;

    /// <summary>
    /// Get the offset value to align the given element position range.
    /// </summary>
    /// <param name="low">The lowest point of the element's position.</param>
    /// <param name="high">The highest point of the element's position.</param>
    /// <returns>The amount to offset the element to fit the set alignment.</returns>
    public readonly float Get(float low, float high) => low - ((high - low) * Percent);
}

/// <summary>
/// Information for aligning a 2-dimensional element.
/// </summary>
/// <param name="alignX">
///     The alignment for the x axis, with low as the element's left and high as its right.
/// </param>
/// <param name="alignY">
///     The alignment for the y axis, with low as the element's top and high as its bottom.
/// </param>
public readonly struct AlignmentBound(AlignmentRange alignX, AlignmentRange alignY) {
    /// <summary>
    /// Returns an alignment that aligns to an element's top left corner.
    /// </summary>
    /// <returns></returns>
    public static AlignmentBound TopLeft() => new(Vector2.Zero);

    /// <summary>
    /// Returns an alignment that aligns to an element's top right corner.
    /// </summary>
    /// <returns></returns>
    public static AlignmentBound TopRight() => new(Vector2.UnitX);

    /// <summary>
    /// Returns an alignment that aligns to an element's bottom left corner.
    /// </summary>
    /// <returns></returns>
    public static AlignmentBound BottomLeft() => new(Vector2.UnitY);

    /// <summary>
    /// Returns an alignment that aligns to an element's bottom right corner.
    /// </summary>
    /// <returns></returns>
    public static AlignmentBound BottomRight() => new(Vector2.One);

    /// <summary>
    /// Returns an alignment that aligns to the center of an element's top edge.
    /// </summary>
    /// <returns></returns>
    public static AlignmentBound Top() => new(0.5f, 0);

    /// <summary>
    /// Returns an alignment that aligns to the center of an element's bottom edge.
    /// </summary>
    /// <returns></returns>
    public static AlignmentBound Bottom() => new(0.5f, 1);

    /// <summary>
    /// Returns an alignment that aligns to the center of an element's left edge.
    /// </summary>
    /// <returns></returns>
    public static AlignmentBound Left() => new(0, 0.5f);

    /// <summary>
    /// Returns an alignment that aligns to the center of an element's right edge.
    /// </summary>
    /// <returns></returns>
    public static AlignmentBound Right() => new(1, 0.5f);

    /// <summary>
    /// Returns an alignment that aligns to the center of an element.
    /// </summary>
    /// <returns></returns>
    public static AlignmentBound Center() => new(0.5f, 0.5f);

    /// <summary>
    /// Information for aligning a 2-dimensional element.
    /// </summary>
    /// <param name="percent">
    ///     The alignment's percent offset along the element's width and height. <br/>
    ///     0 aligns to the top left, and 1 aligns to the bottom right.
    /// </param>
    public AlignmentBound(Vector2 percent) : this(new AlignmentRange(percent.X), new AlignmentRange(percent.Y)) { }

    /// <summary>
    /// Information for aligning a 2-dimensional element.
    /// </summary>
    /// <param name="percentX">
    ///     The alignment's percent offset along the element's width. <br/>
    ///     0 aligns to the left, and 1 aligns to the right.
    /// </param>
    /// <param name="percentY">
    ///     The alignment's percent offset along the element's height. <br/>
    ///     0 aligns to the top, and 1 aligns to the bottom.
    /// </param>
    public AlignmentBound(float percentX, float percentY) : this(new Vector2(percentX, percentY)) { }

    readonly AlignmentRange AlignX = alignX;
    readonly AlignmentRange AlignY = alignY;

    /// <summary>
    ///     The alignment's percent offset along the element's width. <br/>
    ///     0 aligns to the left, and 1 aligns to the right.
    /// </summary>
    public readonly float PercentX => AlignX.Percent;

    /// <summary>
    ///     The alignment's percent offset along the element's height. <br/>
    ///     0 aligns to the top, and 1 aligns to the bottom.
    /// </summary>
    public readonly float PercentY => AlignY.Percent;

    /// <summary>
    /// Get the offset value to align the given element position size.
    /// </summary>
    /// <param name="size">The element's size.</param>
    /// <returns>The amount to offset the element to fit the set alignment.</returns>
    public readonly Vector2 Get(Vector2 size) => new(AlignX.Get(size.X), AlignY.Get(size.Y));

    /// <summary>
    /// Get the offset value to align the given element position bounds.
    /// </summary>
    /// <param name="bounds">The element's bounding rectangle.</param>
    /// <returns>The amount to offset the element to fit the set alignment.</returns>
    public readonly Vector2 Get(Rect bounds) => new(AlignX.Get(bounds.Left, bounds.Right), AlignX.Get(bounds.Top, bounds.Bottom));
}