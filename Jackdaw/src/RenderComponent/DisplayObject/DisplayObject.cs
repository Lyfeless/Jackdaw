using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// An object that can be rendered to the screen. <br/>
/// Use a <see cref="RenderComponent" /> to render.
/// </summary>
public abstract class DisplayObject {
    internal Point2 offset = Point2.Zero;
    internal AlignmentBound alignment = new();

    /// <summary>
    /// The object's rendering bounds.
    /// </summary>
    public RectInt Bounds = new(0, 0);

    /// <summary>
    /// The top-left corner of the object's bounds.
    /// </summary>
    public Vector2 Position => Bounds.Position;

    /// <summary>
    /// The size of the object.
    /// </summary>
    public Vector2 Size => Bounds.Size;

    /// <summary>
    /// The color to render the object with. Defaults to white.
    /// </summary>
    public Color Color = Color.White;

    /// <summary>
    /// The position offset of the display element, independant of the object's alignment.
    /// </summary>
    public Point2 Offset {
        get => offset;
        set {
            offset = value;
            CacheBounds();
        }
    }

    /// <summary>
    /// How the element should be aligned relative to its position.
    /// Defaults to aligning the top left corner to the object's position.
    /// </summary>
    public AlignmentBound Alignment {
        get => alignment;
        set {
            alignment = value;
            CacheBounds();
        }
    }

    /// <summary>
    /// Check if the object is currently inside the window's view bounds.
    /// </summary>
    /// <param name="bounds">The view bounds the object should render in.</param>
    /// <param name="actor">The actor the object is rendered relative to.</param>
    /// <param name="offset">The renderer component's relative offset.</param>
    /// <returns>If the object is onscreen.</returns>
    public bool IsOnScreen(RectInt bounds, Actor actor, Point2 offset) {
        Rect globalBounds = GetGlobalRenderBounds(actor, offset);
        return bounds.Overlaps(globalBounds);
    }

    /// <summary>
    /// Get the global bounds of the object in window coordinates.
    /// </summary>
    /// <param name="actor">The actor the object is rendered relative to.</param>
    /// <param name="offset">The renderer component's relative offset.</param>
    /// <returns>The display object's bounds as an AABB.</returns>
    public Rect GetGlobalRenderBounds(Actor actor, Point2 offset)
        => Bounds
            .Translate(offset)
            .TransformAABB(actor.Transform.GlobalDisplayMatrix);

    /// <summary>
    /// Render the object to the given batcher.
    /// </summary>
    /// <param name="batcher">The batcher to render to.</param>
    public abstract void Render(Batcher batcher);

    /// <summary>
    /// Get the size and position of the overriden element's bounding box. Used to calculate the display object's overal bounds. <br/>
    /// Run <see cref="CacheBounds"/> whenever this value could change.
    /// </summary>
    /// <returns></returns>
    public abstract Rect GetObjectBounds();

    /// <summary>
    /// Stores the display object's bounding box using the bounds provided by <see cref="GetObjectBounds"/>
    /// and the object's offsets. <br/>
    /// Automatically calculated when changing offset values, but not called by default on creation.
    /// Make sure to run once all necessary values are initialized for the display object.
    /// </summary>
    protected void CacheBounds() {
        Rect source = GetObjectBounds();
        Bounds = source.Translate(offset + alignment.Get(source.Size)).Int();
    }
}