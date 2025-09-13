using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class ActorPosition {
    public Actor Actor;

    //! FIXME (Alex): Optimize caching to only update when the value is read

    Transform transform;

    public Transform LocalTransform {
        get => transform;
        set {
            transform = value;
            Cache();
        }
    }

    public Matrix3x2 LocalMatrix => transform.Matrix;
    public Matrix3x2 LocalMatrixInverse => transform.MatrixInverse;

    public Vector2 LocalPosition {
        get => transform.Position;
        set {
            transform.Position = value;
            Cache();
        }
    }

    public float LocalRotation {
        get => transform.Rotation;
        set {
            transform.Rotation = value;
            Cache();
        }
    }

    public Vector2 LocalScale {
        get => transform.Scale;
        set {
            transform.Scale = value;
            Cache();
        }
    }

    public Matrix3x2 GlobalMatrix { get; private set; } = Matrix3x2.Identity;
    public Matrix3x2 GlobalMatrixInverse { get; private set; } = Matrix3x2.Identity;
    public Vector2 GlobalPosition { get; private set; } = Vector2.Zero;
    public float GlobalRotation { get; private set; } = 0;

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

    //! FIXME (Alex): RUN THIS CACHE STEP WHEN A PARENT CHANGES!!! IMPORTANT IMPORTANT IMPORTANT
    internal void Cache() {
        if (Actor.ParentValid) {
            GlobalMatrix = transform.Matrix * Actor.Parent.Position.GlobalMatrix;
            GlobalPosition = Vector2.Transform(LocalPosition, Actor.Parent.Position.GlobalMatrix);
            GlobalRotation = LocalRotation + Actor.Parent.Position.GlobalRotation;
        }
        else {
            GlobalMatrix = transform.Matrix;
            GlobalPosition = transform.Position;
            GlobalRotation = transform.Rotation;
        }

        //! FIXME (Alex): Can this be simplified?
        Matrix3x2.Invert(GlobalMatrix, out Matrix3x2 globalMatrixInv);
        GlobalMatrixInverse = globalMatrixInv;

        foreach (Actor child in Actor.Children.Elements) {
            child.Position.Cache();
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

    //! FIXME (Alex): Operator overloads and type conversions
}