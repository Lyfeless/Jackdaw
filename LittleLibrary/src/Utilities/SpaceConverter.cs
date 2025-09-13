using System.Numerics;
using Foster.Framework;

namespace LittleLib;

/// <summary>
/// A general-purpose utility for converting coordinates between several common spaces.
/// </summary>
/// <param name="game">The game instance.</param>
public class SpaceConverter(LittleGame game) {
    readonly LittleGame Game = game;

    //! FIXME (Alex): Doc comments
    //! FIXME (Alex): replace DisplayLocal with displayLocal position based on renderoffsets

    // public Vector2 MouseToLocal(Actor relativeActor)
    //     => WindowToLocal(Game.Input.Mouse.Position, relativeActor);
    // public Vector2 MouseToGlobal()
    //     => WindowToGlobal(Game.Input.Mouse.Position);
    // public Vector2 MouseToDisplayLocal()
    //     => WindowToDisplayLocal(Game.Input.Mouse.Position);

    // public Vector2 WindowToLocal(Vector2 windowCoord, Actor relativeActor)
    //     => DisplayLocalToLocal(WindowToDisplayLocal(windowCoord), relativeActor);
    // public Vector2 WindowToGlobal(Vector2 windowCoord)
    //     => DisplayLocalToGlobal(WindowToDisplayLocal(windowCoord));
    // public Vector2 WindowToDisplayLocal(Vector2 windowCoord)
    //     => Game.WindowToDisplayLocalspace(windowCoord);

    // public Vector2 DisplayLocalToLocal(Vector2 DisplayLocalCoord, Actor relativeActor)
    //     => GlobalToLocal(DisplayLocalToGlobal(DisplayLocalCoord), relativeActor);
    // public Vector2 DisplayLocalToGlobal(Vector2 DisplayLocalCoord)
    //     => DisplayLocalCoord + Game.DisplayLocalspace.TopLeft;
    // public Vector2 DisplayLocalToWindow(Vector2 DisplayLocalCoord)
    //     => Game.DisplayLocalspaceToWindow(DisplayLocalCoord);

    public Vector2 GlobalToLocal(Vector2 globalCoord, Actor relativeActor)
        => relativeActor.Position.GlobalToLocal(globalCoord);
    public Matrix3x2 GlobalToLocal(Matrix3x2 globalCoord, Actor relativeActor)
        => relativeActor.Position.GlobalToLocal(globalCoord);
    // public Vector2 GlobalToDisplayLocal(Vector2 globalCoord)
    //     => globalCoord - Game.DisplayLocalspace.TopLeft;
    // public Vector2 GlobalToWindow(Vector2 globalCoord)
    //     => DisplayLocalToWindow(GlobalToDisplayLocal(globalCoord));

    public Matrix3x2 LocalToLocal(Actor originLocal, Actor targetLocal)
        => targetLocal.Position.FromOtherLocal(originLocal.Position);
    public Vector2 LocalToLocal(Vector2 localCoord, Actor originLocal, Actor targetLocal)
        => targetLocal.Position.FromOtherLocal(localCoord, originLocal.Position);
    public Matrix3x2 LocalToLocal(Matrix3x2 localMatrix, Actor originLocal, Actor targetLocal)
        => targetLocal.Position.FromOtherLocal(localMatrix, originLocal.Position);
    public Vector2 LocalToGlobal(Vector2 localCoord, Actor relativeActor)
        => relativeActor.Position.LocalToGlobal(localCoord);
    public Matrix3x2 LocalToGlobal(Matrix3x2 localCoord, Actor relativeActor)
        => relativeActor.Position.LocalToGlobal(localCoord);
    // public Vector2 LocalToDisplayLocal(Vector2 localCoord, Actor relativeActor)
    //     => GlobalToDisplayLocal(LocalToGlobal(localCoord, relativeActor));
    // public Vector2 LocalToWindow(Vector2 localCoord, Actor relativeActor)
    //     => DisplayLocalToWindow(LocalToDisplayLocal(localCoord, relativeActor));
}