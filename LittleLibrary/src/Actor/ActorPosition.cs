using System.Numerics;
using Foster.Framework;

namespace LittleLib;

/// <summary>
/// Position data for an Actor's local, global, and display positions.
/// </summary>
public struct ActorPosition {
    /// <summary>
    /// The actor storing the position.
    /// </summary>
    public Actor Actor { get; private set; }

    bool isDirty = true;

    Transform transform;

    /// <summary>
    /// The full transform representing the actor's position relative to its parent.
    /// </summary>
    public Transform LocalTransform {
        get {
            if (isDirty) { CachePosition(); }
            return transform;
        }
        set {
            transform = value;
            MakeDirty();
        }
    }

    /// <summary>
    /// The position matrix representing the actor's position relative to its parent.
    /// </summary>
    public Matrix3x2 LocalMatrix => transform.Matrix;

    /// <summary>
    /// An inverted matrix of the actor's local position, useful for undoing tranformations.
    /// </summary>
    public Matrix3x2 LocalMatrixInverse => transform.MatrixInverse;

    /// <summary>
    /// The actor's local translation, relative to its parent.
    /// </summary>
    public Vector2 LocalPosition {
        get {
            if (isDirty) { CachePosition(); }
            return transform.Position;
        }
        set {
            transform.Position = value;
            MakeDirty();
        }
    }

    /// <summary>
    /// The actor's local rotation, relative to its parent.
    /// </summary>
    public float LocalRotation {
        get {
            if (isDirty) { CachePosition(); }
            return transform.Rotation;
        }
        set {
            transform.Rotation = value;
            MakeDirty();
        }
    }

    /// <summary>
    /// The actor's local scale, relative to its parent.
    /// </summary>
    public Vector2 LocalScale {
        get {
            if (isDirty) { CachePosition(); }
            return transform.Scale;
        }
        set {
            transform.Scale = value;
            MakeDirty();
        }
    }

    InvertableMatrix globalMatrix = new();

    /// <summary>
    /// Ther actor's position in global space, accounting for all parent transforms.
    /// </summary>
    public Matrix3x2 GlobalMatrix {
        get {
            if (isDirty) { CachePosition(); }
            return globalMatrix.Matrix;
        }
    }

    /// <summary>
    /// An inverted matrix of the actor's global position, useful for undoing tranformations.
    /// </summary>
    public Matrix3x2 GlobalMatrixInverse {
        get {
            if (isDirty) { CachePosition(); }
            return globalMatrix.MatrixInverse;
        }
    }

    Vector2 globalPosition = Vector2.Zero;

    /// <summary>
    /// The actor's global translation, accounting for all parent transforms.
    /// </summary>
    public Vector2 GlobalPosition {
        get {
            if (isDirty) { CachePosition(); }
            return globalPosition;
        }
    }

    float globalRotation = 0;

    /// <summary>
    /// The actor's global rotation, accounting for all parent transforms.
    /// </summary>
    public float GlobalRotation {
        get {
            if (isDirty) { CachePosition(); }
            return globalRotation;
        }
    }

    InvertableMatrix localDisplayMatrix = new();

    /// <summary>
    /// The actor's local display position, representing the actor's local position combined with any local render action offsets.
    /// </summary>
    public readonly Matrix3x2 LocalDisplayMatrix => localDisplayMatrix.Matrix;

    /// <summary>
    /// An inverted matrix of the actor's local display position, useful for undoing tranformations.
    /// </summary>
    public readonly Matrix3x2 LocalDisplayMatrixInverse => localDisplayMatrix.MatrixInverse;

    InvertableMatrix globalDisplayMatrix = new();

    /// <summary>
    /// The actor's local display position, representing the actor's global position combined with any local or parent render action offsets.
    /// </summary>
    public readonly Matrix3x2 GlobalDisplayMatrix => globalDisplayMatrix.Matrix;

    /// <summary>
    /// An inverted matrix of the actor's global display position, useful for undoing tranformations.
    /// </summary>
    public readonly Matrix3x2 GlobalDisplayMatrixInverse => globalDisplayMatrix.MatrixInverse;

    /// <summary>
    /// Create a new position for an actor.
    /// </summary>
    /// <param name="actor">The actor storing the position.</param>
    /// <param name="position">The actor's local transform.</param>
    /// <param name="rotation">The actor's local rotation.</param>
    /// <param name="scale">The actor's local scale.</param>
    public ActorPosition(Actor actor, Vector2 position, float rotation, Vector2 scale) {
        Actor = actor;
        transform = new(position, scale, rotation);
    }

    /// <summary>
    /// Create a new position for an actor.
    /// </summary>
    /// <param name="actor">The actor storing the position.</param>
    /// <param name="transform">The actor's local transform.</param>
    public ActorPosition(Actor actor, Transform transform) {
        Actor = actor;
        this.transform = transform;
    }

    /// <summary>
    /// Create a new position for an actor.
    /// </summary>
    /// <param name="actor">The actor storing the position.</param>
    /// <param name="position">The actor's local transform.</param>
    public ActorPosition(Actor actor, Vector2 position) : this(actor, position, 0, Vector2.One) { }

    /// <summary>
    /// Create a new position for an actor.
    /// </summary>
    /// <param name="actor">The actor storing the position.</param>
    /// <param name="position">The actor's local transform.</param>
    /// <param name="rotation">The actor's local rotation.</param>
    public ActorPosition(Actor actor, Vector2 position, float rotation) : this(actor, position, rotation, Vector2.One) { }

    /// <summary>
    /// Create a new position for an actor.
    /// </summary>
    /// <param name="actor">The actor storing the position.</param>
    /// <param name="position">The actor's local transform.</param>
    /// <param name="scale">The actor's local scale.</param>
    public ActorPosition(Actor actor, Vector2 position, Vector2 scale) : this(actor, position, 0, scale) { }

    /// <summary>
    /// Create a new position for an actor.
    /// </summary>
    /// <param name="actor">The actor storing the position.</param>
    public ActorPosition(Actor actor) : this(actor, Vector2.Zero, 0, Vector2.One) { }

    internal void MakeDirty() {
        isDirty = true;
        foreach (Actor child in Actor.Children.Elements) {
            child.Position.MakeDirty();
        }
    }

    void CachePosition() {
        isDirty = false;

        if (Actor.ParentValid) {
            globalMatrix.Matrix = transform.Matrix * Actor.Parent.Position.GlobalMatrix;
            globalPosition = Vector2.Transform(LocalPosition, Actor.Parent.Position.GlobalMatrix);
            globalRotation = LocalRotation + Actor.Parent.Position.GlobalRotation;
        }
        else {
            globalMatrix.Matrix = transform.Matrix;
            globalPosition = transform.Position;
            globalRotation = transform.Rotation;
        }
    }

    internal void CacheDisplay() {
        //! FIXME (Alex): Round the transforms on these

        Matrix3x2 localDisplay = Actor.RenderActions.GetDisplayMatrix() * LocalMatrix;
        if (LocalDisplayMatrix != localDisplay) {
            localDisplayMatrix.Matrix = localDisplay;
        }

        if (Actor.ParentValid) {
            Matrix3x2 globalDisplay = localDisplay * Actor.Parent.Position.GlobalDisplayMatrix;
            if (GlobalDisplayMatrix != globalDisplay) {
                globalDisplayMatrix.Matrix = globalDisplay;
            }
        }
        else {
            globalDisplayMatrix.Matrix = LocalDisplayMatrix;
        }
    }

    /// <summary>
    /// Convert a position from local space to global.
    /// <br/> <br/>
    /// Local Space - Positions relative to the actor's location.
    /// <br/>
    /// Global Space - Positions relative to the world origin.
    /// </summary>
    /// <param name="matrix">The local position to transform.</param>
    /// <returns>The local position in global space.</returns>
    public Matrix3x2 LocalToGlobal(Matrix3x2 matrix) => matrix * GlobalMatrix;

    /// <summary>
    /// Convert a position from local space to global.
    /// <br/> <br/>
    /// Local Space - Positions relative to the actor's location.
    /// <br/>
    /// Global Space - Positions relative to the world origin.
    /// </summary>
    /// <param name="position">The local position to transform.</param>
    /// <returns>The local position in global space.</returns>
    public Vector2 LocalToGlobal(Vector2 position) => Vector2.Transform(position, GlobalMatrix);

    /// <summary>
    /// Convert a position from global space to local.
    /// <br/> <br/>
    /// Global Space - Positions relative to the world origin.
    /// <br/>
    /// Local Space - Positions relative to the actor's location.
    /// </summary>
    /// <param name="matrix">The global position to transform.</param>
    /// <returns>The global position in local space.</returns>
    public Matrix3x2 GlobalToLocal(Matrix3x2 matrix) => matrix * GlobalMatrixInverse;

    /// <summary>
    /// Convert a position from global space to local.
    /// <br/> <br/>
    /// Global Space - Positions relative to the world origin.
    /// <br/>
    /// Local Space - Positions relative to the actor's location.
    /// </summary>
    /// <param name="position">The global position to transform.</param>
    /// <returns>The global position in local space.</returns>
    public Vector2 GlobalToLocal(Vector2 position) => Vector2.Transform(position, GlobalMatrixInverse);

    /// <summary>
    /// Convert a position to local space from another actor's local.
    /// <br/> <br/>
    /// Local Space - Positions relative to the actor's location.
    /// </summary>
    /// <param name="matrix">The local position to transform.</param>
    /// <param name="other">The actor the position is relative to.</param>
    /// <returns>The position in the new local space.</returns>
    public Matrix3x2 FromOtherLocal(Matrix3x2 matrix, Actor other) => FromOtherLocal(matrix, other.Position);

    /// <summary>
    /// Convert a position to local space from another actor's local.
    /// <br/> <br/>
    /// Local Space - Positions relative to the actor's location.
    /// </summary>
    /// <param name="matrix">The local position to transform.</param>
    /// <param name="other">The actor position the position is relative to.</param>
    /// <returns>The position in the new local space.</returns>
    public Matrix3x2 FromOtherLocal(Matrix3x2 matrix, ActorPosition other) => GlobalToLocal(other.GlobalToLocal(matrix));

    /// <summary>
    /// Convert a position to local space from another actor's local.
    /// <br/> <br/>
    /// Local Space - Positions relative to the actor's location.
    /// </summary>
    /// <param name="position">The local position to transform.</param>
    /// <param name="other">The actor the position is relative to.</param>
    /// <returns>The position in the new local space.</returns>
    public Vector2 FromOtherLocal(Vector2 position, Actor other) => FromOtherLocal(position, other.Position);

    /// <summary>
    /// Convert a position to local space from another actor's local.
    /// <br/> <br/>
    /// Local Space - Positions relative to the actor's location.
    /// </summary>
    /// <param name="position">The local position to transform.</param>
    /// <param name="other">The actor position the position is relative to.</param>
    /// <returns>The position in the new local space.</returns>
    public Vector2 FromOtherLocal(Vector2 position, ActorPosition other) => GlobalToLocal(other.GlobalToLocal(position));

    /// <summary>
    /// Convert a another actor's position to local space.
    /// <br/> <br/>
    /// Local Space - Positions relative to the actor's location.
    /// </summary>
    /// <param name="other">The actor to get the position from.</param>
    /// <returns>The position in the new local space.</returns>
    public Matrix3x2 FromOtherLocal(Actor other) => GlobalToLocal(other.Position.GlobalMatrix);

    /// <summary>
    /// Convert a another actor's position to local space.
    /// <br/> <br/>
    /// Local Space - Positions relative to the actor's location.
    /// </summary>
    /// <param name="other">The actor position to get the position from.</param>
    /// <returns>The position in the new local space.</returns>
    public Matrix3x2 FromOtherLocal(ActorPosition other) => GlobalToLocal(other.GlobalMatrix);

    /// <summary>
    /// Convert a position from local space to display.
    /// <br/> <br/>
    /// Local Space - Positions relative to the actor's location.
    /// <br/>
    /// Display Space - Positions relative to the actor's location, with any enter actions applied from the actor or its parents.
    /// </summary>
    /// <param name="matrix">The local position to transform.</param>
    /// <returns>The local position in display space.</returns>
    public Matrix3x2 LocalToDisplay(Matrix3x2 matrix) => GlobalToDisplay(LocalToGlobal(matrix));

    /// <summary>
    /// Convert a position from local space to display.
    /// <br/> <br/>
    /// Local Space - Positions relative to the actor's location.
    /// <br/>
    /// Display Space - Positions relative to the actor's location, with any enter actions applied from the actor or its parents.
    /// </summary>
    /// <param name="position">The local position to transform.</param>
    /// <returns>The local position in display space.</returns>
    public Vector2 LocalToDisplay(Vector2 position) => GlobalToDisplay(LocalToGlobal(position));

    /// <summary>
    /// Convert a position from global space to display.
    /// <br/> <br/>
    /// Global Space - Positions relative to the world origin.
    /// <br/>
    /// Display Space - Positions relative to the actor's location, with any enter actions applied from the actor or its parents.
    /// </summary>
    /// <param name="matrix">The global position to transform.</param>
    /// <returns>The global position in display space.</returns>
    public Matrix3x2 GlobalToDisplay(Matrix3x2 matrix) => GlobalDisplayMatrixInverse * matrix;

    /// <summary>
    /// Convert a position from global space to display.
    /// <br/> <br/>
    /// Global Space - Positions relative to the world origin.
    /// <br/>
    /// Display Space - Positions relative to the actor's location, with any enter actions applied from the actor or its parents.
    /// </summary>
    /// <param name="position">The global position to transform.</param>
    /// <returns>The global position in display space.</returns>
    public Vector2 GlobalToDisplay(Vector2 position) => Vector2.Transform(position, GlobalDisplayMatrixInverse);

    /// <summary>
    /// Convert a position from display space to local.
    /// <br/> <br/>
    /// Display Space - Positions relative to the actor's location, with any enter actions applied from the actor or its parents.
    /// <br/>
    /// Local Space - Positions relative to the actor's location.
    /// </summary>
    /// <param name="matrix">The display position to transform.</param>
    /// <returns>The display position in local space.</returns>
    public Matrix3x2 DisplayToLocal(Matrix3x2 matrix) => GlobalToLocal(DisplayToGlobal(matrix));

    /// <summary>
    /// Convert a position from display space to local.
    /// <br/> <br/>
    /// Display Space - Positions relative to the actor's location, with any enter actions applied from the actor or its parents.
    /// <br/>
    /// Local Space - Positions relative to the actor's location.
    /// </summary>
    /// <param name="position">The display position to transform.</param>
    /// <returns>The display position in local space.</returns>
    public Vector2 DisplayToLocal(Vector2 position) => GlobalToLocal(DisplayToGlobal(position));

    /// <summary>
    /// Convert a position from display space to global.
    /// <br/> <br/>
    /// Display Space - Positions relative to the actor's location, with any enter actions applied from the actor or its parents.
    /// <br/>
    /// Global Space - Positions relative to the world origin.
    /// </summary>
    /// <param name="matrix">The display position to transform.</param>
    /// <returns>The display position in global space.</returns>
    public Matrix3x2 DisplayToGlobal(Matrix3x2 matrix) => GlobalDisplayMatrix * matrix;

    /// <summary>
    /// Convert a position from display space to global.
    /// <br/> <br/>
    /// Display Space - Positions relative to the actor's location, with any enter actions applied from the actor or its parents.
    /// <br/>
    /// Global Space - Positions relative to the world origin.
    /// </summary>
    /// <param name="position">The display position to transform.</param>
    /// <returns>The display position in global space.</returns>
    public Vector2 DisplayToGlobal(Vector2 position) => Vector2.Transform(position, GlobalDisplayMatrix);
}