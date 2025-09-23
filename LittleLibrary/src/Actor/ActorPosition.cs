using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public struct ActorPosition {
    public Actor Actor;

    bool isDirty = true;

    Transform transform;

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

    public Matrix3x2 LocalMatrix => transform.Matrix;
    public Matrix3x2 LocalMatrixInverse => transform.MatrixInverse;

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

    Matrix3x2 globalMatrix = Matrix3x2.Identity;
    public Matrix3x2 GlobalMatrix {
        get {
            if (isDirty) { CachePosition(); }
            return globalMatrix;
        }
    }

    Matrix3x2 globalMatrixInverse = Matrix3x2.Identity;
    public Matrix3x2 GlobalMatrixInverse {
        get {
            if (isDirty) { CachePosition(); }
            return globalMatrixInverse;
        }
    }

    Vector2 globalPosition = Vector2.Zero;
    public Vector2 GlobalPosition {
        get {
            if (isDirty) { CachePosition(); }
            return globalPosition;
        }
    }

    float globalRotation = 0;
    public float GlobalRotation {
        get {
            if (isDirty) { CachePosition(); }
            return globalRotation;
        }
    }

    public Matrix3x2 LocalDisplayMatrix { get; internal set; } = Matrix3x2.Identity;
    public Matrix3x2 LocalDisplayMatrixInverse { get; internal set; } = Matrix3x2.Identity;
    public Matrix3x2 GlobalDisplayMatrix { get; internal set; } = Matrix3x2.Identity;
    public Matrix3x2 GlobalDisplayMatrixInverse { get; internal set; } = Matrix3x2.Identity;

    public ActorPosition(Actor actor, Vector2 position, float rotation, Vector2 scale) {
        Actor = actor;
        transform = new(position, scale, rotation);
    }
    public ActorPosition(Actor actor, Transform transform) {
        Actor = actor;
        this.transform = transform;
    }

    public ActorPosition(Actor actor, Vector2 position) : this(actor, position, 0, Vector2.One) { }
    public ActorPosition(Actor actor, Vector2 position, float rotation) : this(actor, position, rotation, Vector2.One) { }
    public ActorPosition(Actor actor, Vector2 position, Vector2 scale) : this(actor, position, 0, scale) { }
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
            globalMatrix = transform.Matrix * Actor.Parent.Position.GlobalMatrix;
            globalMatrixInverse = InvertMatrix(globalMatrix);
            globalPosition = Vector2.Transform(LocalPosition, Actor.Parent.Position.GlobalMatrix);
            globalRotation = LocalRotation + Actor.Parent.Position.GlobalRotation;
        }
        else {
            globalMatrix = transform.Matrix;
            globalMatrixInverse = transform.MatrixInverse;
            globalPosition = transform.Position;
            globalRotation = transform.Rotation;
        }
    }

    // void CacheDisplay() {
    //     displayDirty = false;
    //     if (isDirty) { CachePosition(); }

    //     localDisplayMatrix = Actor.RenderActions.GetDisplayMatrix() * transform.Matrix;
    //     localDisplayMatrixInverse = InvertMatrix(localDisplayMatrix);

    //     if (Actor.ParentValid) {
    //         globalDisplayMatrix = localDisplayMatrix * Actor.Parent.Position.GlobalDisplayMatrix;
    //         globalDisplayMatrixInverse = InvertMatrix(globalDisplayMatrix);
    //     }
    //     else {
    //         globalDisplayMatrix = localDisplayMatrix;
    //         globalDisplayMatrixInverse = localDisplayMatrixInverse;
    //     }
    // }

    internal void CacheDisplay() {
        //! FIXME (Alex): Round the transforms on these

        Matrix3x2 localDisplay = Actor.RenderActions.GetDisplayMatrix() * LocalMatrix;
        if (LocalDisplayMatrix != localDisplay) {
            LocalDisplayMatrix = localDisplay;
            LocalDisplayMatrixInverse = InvertMatrix(localDisplay);
        }

        if (Actor.ParentValid) {
            Matrix3x2 globalDisplay = localDisplay * Actor.Parent.Position.GlobalDisplayMatrix;
            if (GlobalDisplayMatrix != globalDisplay) {
                GlobalDisplayMatrix = globalDisplay;
                GlobalDisplayMatrixInverse = InvertMatrix(globalDisplay);
            }
        }
        else {
            GlobalDisplayMatrix = LocalDisplayMatrix;
            GlobalDisplayMatrixInverse = LocalDisplayMatrixInverse;
        }
    }

    public Matrix3x2 LocalToGlobal(Matrix3x2 matrix) => matrix * GlobalMatrix;
    public Vector2 LocalToGlobal(Vector2 position) => Vector2.Transform(position, GlobalMatrix);

    public Matrix3x2 GlobalToLocal(Matrix3x2 matrix) => matrix * GlobalMatrixInverse;
    public Vector2 GlobalToLocal(Vector2 position) => Vector2.Transform(position, GlobalMatrixInverse);

    public Matrix3x2 FromOtherLocal(Matrix3x2 matrix, Actor other) => FromOtherLocal(matrix, other.Position);
    public Matrix3x2 FromOtherLocal(Matrix3x2 matrix, ActorPosition other) => GlobalToLocal(other.GlobalToLocal(matrix));
    public Vector2 FromOtherLocal(Vector2 position, Actor other) => FromOtherLocal(position, other.Position);
    public Vector2 FromOtherLocal(Vector2 position, ActorPosition other) => GlobalToLocal(other.GlobalToLocal(position));
    public Matrix3x2 FromOtherLocal(ActorPosition other) => GlobalToLocal(other.GlobalMatrix);

    public Matrix3x2 LocalToDisplay(Matrix3x2 matrix) => GlobalToDisplay(LocalToGlobal(matrix));
    public Vector2 LocalToDisplay(Vector2 position) => GlobalToDisplay(LocalToGlobal(position));
    public Matrix3x2 GlobalToDisplay(Matrix3x2 matrix) => GlobalDisplayMatrixInverse * matrix;
    public Vector2 GlobalToDisplay(Vector2 position) => Vector2.Transform(position, GlobalDisplayMatrixInverse);

    public Matrix3x2 DisplayToLocal(Matrix3x2 matrix) => GlobalToLocal(DisplayToGlobal(matrix));
    public Vector2 DisplayToLocal(Vector2 position) => GlobalToLocal(DisplayToGlobal(position));
    public Matrix3x2 DisplayToGlobal(Matrix3x2 matrix) => GlobalDisplayMatrix * matrix;
    public Vector2 DisplayToGlobal(Vector2 position) => Vector2.Transform(position, GlobalDisplayMatrix);

    static Matrix3x2 InvertMatrix(Matrix3x2 matrix) {
        Matrix3x2.Invert(matrix, out Matrix3x2 inv);
        return inv;
    }
}