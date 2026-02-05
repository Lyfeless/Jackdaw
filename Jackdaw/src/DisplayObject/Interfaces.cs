namespace Jackdaw;

/// <summary>
/// Display configuration for rendering an outline object with lines.
/// </summary>
public interface IDisplayObjectOutline {
    /// <summary>
    /// The width of the rendered lines.
    /// </summary>
    public float LineWeight { get; set; }
}

/// <summary>
/// Display configuration for rendering an object with dashed lines.
/// </summary>
public interface IDisplayObjectDashedOutline : IDisplayObjectOutline {
    /// <summary>
    /// The length of every segment and gap pair along the line.
    /// </summary>
    public float DashLength { get; set; }

    /// <summary>
    /// The percent through the <see cref="DashLength" /> the line should start at.
    /// </summary>
    public float OffsetPercent { get; set; }
}