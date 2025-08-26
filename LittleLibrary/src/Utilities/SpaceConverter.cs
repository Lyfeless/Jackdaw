using System.Numerics;
using Foster.Framework;

namespace LittleLib;

/// <summary>
/// A gneral-purpose utility for converting coordinates between several common spaces.
/// </summary>
/// <param name="game">The game instance.</param>
public class SpaceConverter(LittleGame game) {
    readonly LittleGame Game = game;

    //! FIXME (Alex): Doc comments
    //! FIXME (Alex): replace view with display position based on renderoffsets

    // public Point2 MouseToTile<Tin, Tout>(Actor relativeActor, ISpatialGrid<Tin, Tout> grid)
    //     => WindowToTile(Game.Input.Mouse.Position, relativeActor, grid);
    // public Vector2 MouseToLocal(Actor relativeActor)
    //     => WindowToLocal(Game.Input.Mouse.Position, relativeActor);
    // public Vector2 MouseToGlobal()
    //     => WindowToGlobal(Game.Input.Mouse.Position);
    // public Vector2 MouseToView()
    //     => WindowToView(Game.Input.Mouse.Position);

    // public Point2 WindowToTile<Tin, Tout>(Vector2 windowCoord, Actor relativeActor, ISpatialGrid<Tin, Tout> grid)
    //     => ViewToTile(WindowToView(windowCoord), relativeActor, grid);
    // public Vector2 WindowToLocal(Vector2 windowCoord, Actor relativeActor)
    //     => ViewToLocal(WindowToView(windowCoord), relativeActor);
    // public Vector2 WindowToGlobal(Vector2 windowCoord)
    //     => ViewToGlobal(WindowToView(windowCoord));
    // public Vector2 WindowToView(Vector2 windowCoord)
    //     => Game.WindowToViewspace(windowCoord);

    // public Point2 ViewToTile<Tin, Tout>(Vector2 viewCoord, Actor relativeActor, ISpatialGrid<Tin, Tout> grid)
    //     => LocalToTile(ViewToLocal(viewCoord, relativeActor), grid);
    // public Vector2 ViewToLocal(Vector2 viewCoord, Actor relativeActor)
    //     => GlobalToLocal(ViewToGlobal(viewCoord), relativeActor);
    // public Vector2 ViewToGlobal(Vector2 viewCoord)
    //     => viewCoord + Game.Viewspace.TopLeft;
    // public Vector2 ViewToWindow(Vector2 viewCoord)
    //     => Game.ViewspaceToWindow(viewCoord);

    public Point2 GlobalToTile<Tin, Tout>(Vector2 globalCoord, Actor relativeActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToTile(GlobalToLocal(globalCoord, relativeActor), grid);
    public Vector2 GlobalToLocal(Vector2 globalCoord, Actor relativeActor)
        => relativeActor.GlobalToLocal(globalCoord);
    // public Vector2 GlobalToView(Vector2 globalCoord)
    //     => globalCoord - Game.Viewspace.TopLeft;
    // public Vector2 GlobalToWindow(Vector2 globalCoord)
    //     => ViewToWindow(GlobalToView(globalCoord));

    public Vector2 LocalToLocal(Actor originLocal, Actor targetLocal)
        => GlobalToLocal(originLocal.GlobalPosition, targetLocal);
    public Vector2 LocalToLocal(Vector2 localCoord, Actor originLocal, Actor targetLocal)
        => GlobalToLocal(LocalToGlobal(localCoord, originLocal), targetLocal);
    public Point2 LocalToTile<Tin, Tout>(Vector2 localCoord, ISpatialGrid<Tin, Tout> grid)
        => grid.LocalToTileCoord(localCoord);
    public Vector2 LocalToGlobal(Vector2 localCoord, Actor relativeActor)
        => relativeActor.LocalToGlobal(localCoord);
    // public Vector2 LocalToView(Vector2 localCoord, Actor relativeActor)
    //     => GlobalToView(LocalToGlobal(localCoord, relativeActor));
    // public Vector2 LocalToWindow(Vector2 localCoord, Actor relativeActor)
    //     => ViewToWindow(LocalToView(localCoord, relativeActor));

    public Vector2 TileToLocal<Tin, Tout>(Point2 tileCoord, ISpatialGrid<Tin, Tout> grid)
        => grid.TileCoordToLocal(tileCoord);
    public Vector2 TileToGlobal<Tin, Tout>(Point2 tileCoord, ISpatialGrid<Tin, Tout> grid, Actor relativeActor)
        => LocalToGlobal(TileToLocal(tileCoord, grid), relativeActor);
    // public Vector2 TileToView<Tin, Tout>(Point2 tileCoord, ISpatialGrid<Tin, Tout> grid, Actor relativeActor)
    //     => GlobalToView(TileToGlobal(tileCoord, grid, relativeActor));
    // public Vector2 TileToWindow<Tin, Tout>(Point2 tileCoord, ISpatialGrid<Tin, Tout> grid, Actor relativeActor)
    //     => ViewToWindow(TileToView(tileCoord, grid, relativeActor));
}