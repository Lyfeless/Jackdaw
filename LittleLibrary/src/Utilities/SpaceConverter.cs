using System.Numerics;
using System.Runtime.InteropServices;

namespace LittleLib;

/// <summary>
/// A general-purpose utility for converting coordinates between several common spaces.
/// </summary>
/// <param name="game">The game instance.</param>
public class SpaceConverter(LittleGame game) {
    readonly LittleGame Game = game;

    //! FIXME (Alex): Doc comments
    //! FIXME (Alex): Literally none of this is tested right now

    /*
        Global -> Root space for all objects, always originates from 0,0 at top left of window. Doubles as Window Space
        Local -> Position Relative to internal location of actor
        DisplayLocal -> Position Relative to rendered position of actor
        Tile -> Position in spatial grid with each tile being one unit
    */

    public Vector2 MouseToGlobal()
        => Game.Input.Mouse.Position;
    public Vector2 MouseToLocal(Actor localActor)
        => GlobalToLocal(Game.Input.Mouse.Position, localActor);
    public Vector2 MouseToDisplayLocal(Actor displayActor)
        => GlobalToDisplayLocal(Game.Input.Mouse.Position, displayActor);
    public Vector2 MouseToTile<Tin, Tout>(Actor localActor, ISpatialGrid<Tin, Tout> grid)
        => GlobalToTile(Game.Input.Mouse.Position, localActor, grid);
    public Vector2 MouseToDisplayTile<Tin, Tout>(Actor localActor, ISpatialGrid<Tin, Tout> grid)
        => GlobalToDisplayTile(Game.Input.Mouse.Position, localActor, grid);

    public Matrix3x2 GlobalToLocal(Matrix3x2 position, Actor localActor)
        => localActor.Position.GlobalToLocal(position);
    public Vector2 GlobalToLocal(Vector2 position, Actor localActor)
        => localActor.Position.GlobalToLocal(position);
    public Matrix3x2 GlobalToDisplayLocal(Matrix3x2 position, Actor displayActor)
        => displayActor.Position.GlobalToDisplay(position);
    public Vector2 GlobalToDisplayLocal(Vector2 position, Actor displayActor)
        => displayActor.Position.GlobalToDisplay(position);
    public Matrix3x2 GlobalToTile<Tin, Tout>(Matrix3x2 position, Actor localActor, ISpatialGrid<Tin, Tout> grid)
        => grid.LocalToTileCoord(GlobalToLocal(position, localActor));
    public Vector2 GlobalToTile<Tin, Tout>(Vector2 position, Actor localActor, ISpatialGrid<Tin, Tout> grid)
        => grid.LocalToTileCoord(localActor.Position.GlobalToLocal(position));
    public Matrix3x2 GlobalToDisplayTile<Tin, Tout>(Matrix3x2 position, Actor displayActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToTile(GlobalToDisplayLocal(position, displayActor), grid);
    public Vector2 GlobalToDisplayTile<Tin, Tout>(Vector2 position, Actor displayActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToTile(GlobalToDisplayLocal(position, displayActor), grid);

    public Matrix3x2 LocalToGlobal(Matrix3x2 position, Actor localActor)
        => localActor.Position.LocalToGlobal(position);
    public Vector2 LocalToGlobal(Vector2 position, Actor localActor)
        => localActor.Position.LocalToGlobal(position);
    public Matrix3x2 LocalToDisplayLocal(Matrix3x2 position, Actor localActor, Actor displayActor)
        => displayActor.Position.GlobalToDisplay(LocalToGlobal(position, localActor));
    public Vector2 LocalToDisplayLocal(Vector2 position, Actor localActor, Actor displayActor)
        => displayActor.Position.GlobalToDisplay(LocalToGlobal(position, localActor));
    public Matrix3x2 LocalToDisplayLocal(Matrix3x2 position, Actor displayActor)
        => displayActor.Position.LocalToDisplay(position);
    public Vector2 LocalToDisplayLocal(Vector2 position, Actor displayActor)
        => displayActor.Position.LocalToDisplay(position);
    public Matrix3x2 LocalToLocal(Matrix3x2 position, Actor originLocalActor, Actor targetLocalActor)
        => targetLocalActor.Position.FromOtherLocal(position, originLocalActor);
    public Vector2 LocalToLocal(Vector2 position, Actor originLocalActor, Actor targetLocalActor)
        => targetLocalActor.Position.FromOtherLocal(position, originLocalActor);
    public Matrix3x2 LocalToTile<Tin, Tout>(Matrix3x2 position, ISpatialGrid<Tin, Tout> grid)
        => grid.LocalToTileCoord(position);
    public Vector2 LocalToTile<Tin, Tout>(Vector2 position, ISpatialGrid<Tin, Tout> grid)
        => grid.LocalToTileCoord(position);
    public Matrix3x2 LocalToTile<Tin, Tout>(Matrix3x2 position, Actor originLocalActor, Actor targetLocalActor, ISpatialGrid<Tin, Tout> grid)
        => grid.LocalToTileCoord(LocalToLocal(position, originLocalActor, targetLocalActor));
    public Vector2 LocalToTile<Tin, Tout>(Vector2 position, Actor originLocalActor, Actor targetLocalActor, ISpatialGrid<Tin, Tout> grid)
        => grid.LocalToTileCoord(LocalToLocal(position, originLocalActor, targetLocalActor));
    public Matrix3x2 LocalToDisplayTile<Tin, Tout>(Matrix3x2 position, Actor localActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToTile(LocalToDisplayLocal(position, localActor), grid);
    public Vector2 LocalToDisplayTile<Tin, Tout>(Vector2 position, Actor localActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToTile(LocalToDisplayLocal(position, localActor), grid);
    public Matrix3x2 LocalToDisplayTile<Tin, Tout>(Matrix3x2 position, Actor localActor, Actor displayActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToTile(LocalToDisplayLocal(position, localActor, displayActor), grid);
    public Vector2 LocalToDisplayTile<Tin, Tout>(Vector2 position, Actor localActor, Actor displayActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToTile(LocalToDisplayLocal(position, localActor, displayActor), grid);

    public Matrix3x2 DisplayLocalToGlobal(Matrix3x2 position, Actor localActor)
        => localActor.Position.DisplayToGlobal(position);
    public Vector2 DisplayLocalToGlobal(Vector2 position, Actor localActor)
        => localActor.Position.DisplayToGlobal(position);
    public Matrix3x2 DisplayLocalToLocal(Matrix3x2 position, Actor localActor)
        => localActor.Position.DisplayToLocal(position);
    public Vector2 DisplayLocalToLocal(Vector2 position, Actor localActor)
        => localActor.Position.DisplayToLocal(position);
    public Matrix3x2 DisplayLocalToLocal(Matrix3x2 position, Actor localActor, Actor displayActor)
        => GlobalToLocal(DisplayLocalToGlobal(position, displayActor), localActor);
    public Vector2 DisplayLocalToLocal(Vector2 position, Actor localActor, Actor displayActor)
        => GlobalToLocal(DisplayLocalToGlobal(position, displayActor), localActor);
    public Matrix3x2 DisplayLocalToDisplayLocal(Matrix3x2 position, Actor originLocalActor, Actor targetLocalActor)
        => targetLocalActor.Position.GlobalToDisplay(originLocalActor.Position.DisplayToGlobal(position));
    public Vector2 DisplayLocalToDisplayLocal(Vector2 position, Actor originLocalActor, Actor targetLocalActor)
        => targetLocalActor.Position.GlobalToDisplay(originLocalActor.Position.DisplayToGlobal(position));
    public Matrix3x2 DisplayLocalToTile<Tin, Tout>(Matrix3x2 position, Actor localActor, ISpatialGrid<Tin, Tout> grid)
        => grid.LocalToTileCoord(localActor.Position.DisplayToLocal(position));
    public Vector2 DisplayLocalToTile<Tin, Tout>(Vector2 position, Actor localActor, ISpatialGrid<Tin, Tout> grid)
        => grid.LocalToTileCoord(localActor.Position.DisplayToLocal(position));
    public Matrix3x2 DisplayLocalToTile<Tin, Tout>(Matrix3x2 position, Actor originLocalActor, Actor targetLocalActor, ISpatialGrid<Tin, Tout> grid)
        => DisplayLocalToTile(LocalToLocal(position, originLocalActor, targetLocalActor), targetLocalActor, grid);
    public Vector2 DisplayLocalToTile<Tin, Tout>(Vector2 position, Actor originLocalActor, Actor targetLocalActor, ISpatialGrid<Tin, Tout> grid)
        => DisplayLocalToTile(LocalToLocal(position, originLocalActor, targetLocalActor), targetLocalActor, grid);
    public Matrix3x2 DisplayLocalToDisplayTile<Tin, Tout>(Matrix3x2 position, ISpatialGrid<Tin, Tout> grid)
        => LocalToTile(position, grid);
    public Vector2 DisplayLocalToDisplayTile<Tin, Tout>(Vector2 position, ISpatialGrid<Tin, Tout> grid)
        => LocalToTile(position, grid);
    public Matrix3x2 DisplayLocalToDisplayTile<Tin, Tout>(Matrix3x2 position, Actor originDisplayActor, Actor targetDisplayActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToTile(DisplayLocalToDisplayLocal(position, originDisplayActor, targetDisplayActor), grid);
    public Vector2 DisplayLocalToDisplayTile<Tin, Tout>(Vector2 position, Actor originDisplayActor, Actor targetDisplayActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToTile(DisplayLocalToDisplayLocal(position, originDisplayActor, targetDisplayActor), grid);

    public Matrix3x2 TileToLocal<Tin, Tout>(Matrix3x2 position, ISpatialGrid<Tin, Tout> grid)
        => grid.TileCoordToLocal(position);
    public Vector2 TileToLocal<Tin, Tout>(Vector2 position, ISpatialGrid<Tin, Tout> grid)
        => grid.TileCoordToLocal(position);
    public Matrix3x2 TileToLocal<Tin, Tout>(Matrix3x2 position, ISpatialGrid<Tin, Tout> grid, Actor originLocalActor, Actor targetLocalActor)
        => LocalToLocal(TileToLocal(position, grid), originLocalActor, targetLocalActor);
    public Vector2 TileToLocal<Tin, Tout>(Vector2 position, ISpatialGrid<Tin, Tout> grid, Actor originLocalActor, Actor targetLocalActor)
        => LocalToLocal(TileToLocal(position, grid), originLocalActor, targetLocalActor);
    public Matrix3x2 TileToGlobal<Tin, Tout>(Matrix3x2 position, Actor localActor, ISpatialGrid<Tin, Tout> grid)
        => localActor.Position.LocalToGlobal(grid.TileCoordToLocal(position));
    public Vector2 TileToGlobal<Tin, Tout>(Vector2 position, Actor localActor, ISpatialGrid<Tin, Tout> grid)
        => localActor.Position.LocalToGlobal(grid.TileCoordToLocal(position));
    public Matrix3x2 TileToTile<Tin, Tout>(Matrix3x2 position, Actor localActor, ISpatialGrid<Tin, Tout> originGrid, ISpatialGrid<Tin, Tout> targetGrid)
        => targetGrid.LocalToTileCoord(originGrid.TileCoordToLocal(position));
    public Vector2 TileToTile<Tin, Tout>(Vector2 position, ISpatialGrid<Tin, Tout> originGrid, ISpatialGrid<Tin, Tout> targetGrid)
        => targetGrid.LocalToTileCoord(originGrid.TileCoordToLocal(position));
    public Matrix3x2 TileToTile<Tin, Tout>(Matrix3x2 position, Actor originLocalActor, Actor targetLocalActor, ISpatialGrid<Tin, Tout> originGrid, ISpatialGrid<Tin, Tout> targetGrid)
        => GlobalToTile(TileToGlobal(position, originLocalActor, originGrid), targetLocalActor, targetGrid);
    public Vector2 TileToTile<Tin, Tout>(Vector2 position, Actor originLocalActor, Actor targetLocalActor, ISpatialGrid<Tin, Tout> originGrid, ISpatialGrid<Tin, Tout> targetGrid)
        => GlobalToTile(TileToGlobal(position, originLocalActor, originGrid), targetLocalActor, targetGrid);
    public Matrix3x2 TileToDisplayLocal<Tin, Tout>(Matrix3x2 position, Actor localActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToDisplayLocal(grid.TileCoordToLocal(position), localActor);
    public Vector2 TileToDisplayLocal<Tin, Tout>(Vector2 position, Actor localActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToDisplayLocal(grid.TileCoordToLocal(position), localActor);
    public Matrix3x2 TileToDisplayLocal<Tin, Tout>(Matrix3x2 position, Actor originLocalActor, Actor targetLocalActor, ISpatialGrid<Tin, Tout> grid)
        => GlobalToDisplayLocal(TileToGlobal(position, originLocalActor, grid), targetLocalActor);
    public Vector2 TileToDisplayLocal<Tin, Tout>(Vector2 position, Actor originLocalActor, Actor targetLocalActor, ISpatialGrid<Tin, Tout> grid)
        => GlobalToDisplayLocal(TileToGlobal(position, originLocalActor, grid), targetLocalActor);
    public Matrix3x2 TileToDisplayTile<Tin, Tout>(Matrix3x2 position, Actor localActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToTile(TileToDisplayLocal(position, localActor, grid), grid);
    public Vector2 TileToDisplayTile<Tin, Tout>(Vector2 position, Actor localActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToTile(TileToDisplayLocal(position, localActor, grid), grid);
    public Matrix3x2 TileToDisplayTile<Tin, Tout>(Matrix3x2 position, Actor localActor, Actor displayActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToTile(TileToDisplayLocal(position, localActor, displayActor, grid), grid);
    public Vector2 TileToDisplayTile<Tin, Tout>(Vector2 position, Actor localActor, Actor displayActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToTile(TileToDisplayLocal(position, localActor, displayActor, grid), grid);

    public Matrix3x2 DisplayTileToGlobal<Tin, Tout>(Matrix3x2 position, Actor displayActor, ISpatialGrid<Tin, Tout> grid)
        => DisplayLocalToGlobal(DisplayTileToDisplayLocal(position, grid), displayActor);
    public Vector2 DisplayTileToGlobal<Tin, Tout>(Vector2 position, Actor displayActor, ISpatialGrid<Tin, Tout> grid)
        => DisplayLocalToGlobal(DisplayTileToDisplayLocal(position, grid), displayActor);
    public Matrix3x2 DisplayTileToLocal<Tin, Tout>(Matrix3x2 position, Actor displayActor, ISpatialGrid<Tin, Tout> grid)
        => GlobalToLocal(DisplayTileToGlobal(position, displayActor, grid), displayActor);
    public Vector2 DisplayTileToLocal<Tin, Tout>(Vector2 position, Actor displayActor, ISpatialGrid<Tin, Tout> grid)
        => GlobalToLocal(DisplayTileToGlobal(position, displayActor, grid), displayActor);
    public Matrix3x2 DisplayTileToLocal<Tin, Tout>(Matrix3x2 position, Actor displayActor, Actor localActor, ISpatialGrid<Tin, Tout> grid)
        => GlobalToLocal(DisplayTileToGlobal(position, displayActor, grid), localActor);
    public Vector2 DisplayTileToLocal<Tin, Tout>(Vector2 position, Actor displayActor, Actor localActor, ISpatialGrid<Tin, Tout> grid)
        => GlobalToLocal(DisplayTileToGlobal(position, displayActor, grid), localActor);
    public Matrix3x2 DisplayTileToDisplayLocal<Tin, Tout>(Matrix3x2 position, ISpatialGrid<Tin, Tout> grid)
        => TileToLocal(position, grid);
    public Vector2 DisplayTileToDisplayLocal<Tin, Tout>(Vector2 position, ISpatialGrid<Tin, Tout> grid)
        => TileToLocal(position, grid);
    public Matrix3x2 DisplayTileToTile<Tin, Tout>(Matrix3x2 position, Actor displayActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToTile(DisplayTileToLocal(position, displayActor, grid), grid);
    public Vector2 DisplayTileToTile<Tin, Tout>(Vector2 position, Actor displayActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToTile(DisplayTileToLocal(position, displayActor, grid), grid);
    public Matrix3x2 DisplayTileToTile<Tin, Tout>(Matrix3x2 position, Actor displayActor, Actor localActor, ISpatialGrid<Tin, Tout> displayGrid, ISpatialGrid<Tin, Tout> localGrid)
        => LocalToTile(DisplayTileToLocal(position, displayActor, localActor, displayGrid), localGrid);
    public Vector2 DisplayTileToTile<Tin, Tout>(Vector2 position, Actor displayActor, ISpatialGrid<Tin, Tout> displayGrid, Actor localActor, ISpatialGrid<Tin, Tout> localGrid)
        => LocalToTile(DisplayTileToLocal(position, displayActor, localActor, displayGrid), localGrid);
    public Matrix3x2 DisplayTileToDisplayTile<Tin, Tout>(Matrix3x2 position, Actor originDisplayActor, ISpatialGrid<Tin, Tout> originGrid, Actor targetDisplayActor, ISpatialGrid<Tin, Tout> targetGrid)
        => GlobalToDisplayTile(DisplayTileToGlobal(position, originDisplayActor, originGrid), targetDisplayActor, targetGrid);
    public Vector2 DisplayTileToDisplayTile<Tin, Tout>(Vector2 position, Actor originDisplayActor, ISpatialGrid<Tin, Tout> originGrid, Actor targetDisplayActor, ISpatialGrid<Tin, Tout> targetGrid)
        => GlobalToDisplayTile(DisplayTileToGlobal(position, originDisplayActor, originGrid), targetDisplayActor, targetGrid);
}