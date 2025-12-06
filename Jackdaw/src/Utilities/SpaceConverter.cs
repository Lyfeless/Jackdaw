using System.Numerics;

namespace Jackdaw;

/// <summary>
/// A general-purpose utility for converting coordinates between several common spaces.
/// </summary>
/// <param name="game">The game instance.</param>
public class SpaceConverter(Game game) {
    readonly Game Game = game;

    #region Mouse
    /// <summary>
    /// Convert the mouse position to global space.
    /// <br/> <br/>
    /// Global Space - Positions relative to the world origin.
    /// </summary>
    /// <returns>The mouse position in global space.</returns>
    public Vector2 MouseToGlobal()
        => Game.Input.Mouse.Position;

    /// <summary>
    /// Convert the mouse position to local space.
    /// <br/> <br/>
    /// Local Space - Positions relative to the actor's location.
    /// </summary>
    /// <param name="localActor">The actor the position should be local to.</param>
    /// <returns>the mouse position in the actor's local space.</returns>
    public Vector2 MouseToLocal(Actor localActor)
        => GlobalToLocal(Game.Input.Mouse.Position, localActor);

    /// <summary>
    /// Convert the mouse position to local display space.
    /// <br/> <br/>
    /// Display Local Space - Positions relative to the actor's location, with any enter actions applied from the actor or its parents.
    /// </summary>
    /// <param name="displayActor">The actor the position should be local to.</param>
    /// <returns>The mouse position in the actor's local display space.</returns>
    public Vector2 MouseToDisplayLocal(Actor displayActor)
        => GlobalToDisplayLocal(Game.Input.Mouse.Position, displayActor);

    /// <summary>
    /// Convert the mouse position to tile space.
    /// <br/> <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// </summary>
    /// <param name="localActor">The actor the position should be local to.</param>
    /// <param name="grid">The actor's grid.</param>
    /// <returns>The mouse position as a tile coordinate in the actor's grid.</returns>
    public Vector2 MouseToTile<Tin, Tout>(Actor localActor, ISpatialGrid<Tin, Tout> grid)
        => GlobalToTile(Game.Input.Mouse.Position, localActor, grid);

    /// <summary>
    /// Convert the mouse position to tile space, with an actor's display applied.
    /// <br/> <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// </summary>
    /// <param name="localActor">The actor the position should be local to.</param>
    /// <param name="grid">The actor's grid.</param>
    /// <returns>The mouse position as a tile coordinate in the actor's grid, with display transforms applied.</returns>
    public Vector2 MouseToDisplayTile<Tin, Tout>(Actor localActor, ISpatialGrid<Tin, Tout> grid)
        => GlobalToDisplayTile(Game.Input.Mouse.Position, localActor, grid);
    #endregion

    #region Global
    /// <summary>
    /// Convert a global position to an actor's local space.
    /// <br/> <br/>
    /// Global Space - Positions relative to the world origin.
    /// <br/>
    /// Local Space - Positions relative to the actor's location.
    /// </summary>
    /// <param name="position">The global position to transform.</param>
    /// <param name="localActor">The actor the position should be local to.</param>
    /// <returns>The global position in local space.</returns>
    public Matrix3x2 GlobalToLocal(Matrix3x2 position, Actor localActor)
        => localActor.Transform.GlobalToLocal(position);

    /// <summary>
    /// Convert a global position to an actor's local space.
    /// <br/> <br/>
    /// Global Space - Positions relative to the world origin.
    /// <br/>
    /// Local Space - Positions relative to the actor's location.
    /// </summary>
    /// <param name="position">The global position to transform.</param>
    /// <param name="localActor">The actor the position should be local to.</param>
    /// <returns>The global position in local space.</returns>
    public Vector2 GlobalToLocal(Vector2 position, Actor localActor)
        => localActor.Transform.GlobalToLocal(position);

    /// <summary>
    /// Convert a global position to an actor's local display space.
    /// <br/> <br/>
    /// Global Space - Positions relative to the world origin.
    /// <br/>
    /// Display Local Space - Positions relative to the actor's location, with any enter actions applied from the actor or its parents.
    /// </summary>
    /// <param name="position">The global position to transform.</param>
    /// <param name="displayActor">The actor the position should be local to.</param>
    /// <returns>The global position in local space.</returns>
    public Matrix3x2 GlobalToDisplayLocal(Matrix3x2 position, Actor displayActor)
        => displayActor.Transform.GlobalToDisplay(position);

    /// <summary>
    /// Convert a global position to an actor's local display space.
    /// <br/> <br/>
    /// Global Space - Positions relative to the world origin.
    /// <br/>
    /// Display Local Space - Positions relative to the actor's location, with any enter actions applied from the actor or its parents.
    /// </summary>
    /// <param name="position">The global position to transform.</param>
    /// <param name="displayActor">The actor the position should be local to.</param>
    /// <returns>The global position in local space.</returns>
    public Vector2 GlobalToDisplayLocal(Vector2 position, Actor displayActor)
        => displayActor.Transform.GlobalToDisplay(position);

    /// <summary>
    /// Convert a global position to a grid's local space.
    /// <br/> <br/>
    /// Global Space - Positions relative to the world origin.
    /// <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// </summary>
    /// <param name="position">The global position to transform.</param>
    /// <param name="localActor">The actor the grid is local to.</param>
    /// <param name="grid">The spatial grid.</param>
    /// <returns>The global position in tile space.</returns>
    public Matrix3x2 GlobalToTile<Tin, Tout>(Matrix3x2 position, Actor localActor, ISpatialGrid<Tin, Tout> grid)
        => grid.LocalToTileCoord(GlobalToLocal(position, localActor));

    /// <summary>
    /// Convert a global position to a grid's local space.
    /// <br/> <br/>
    /// Global Space - Positions relative to the world origin.
    /// <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// </summary>
    /// <param name="position">The global position to transform.</param>
    /// <param name="localActor">The actor the grid is local to.</param>
    /// <param name="grid">The spatial grid.</param>
    /// <returns>The global position in tile space.</returns>
    public Vector2 GlobalToTile<Tin, Tout>(Vector2 position, Actor localActor, ISpatialGrid<Tin, Tout> grid)
        => grid.LocalToTileCoord(localActor.Transform.GlobalToLocal(position));

    /// <summary>
    /// Convert a global position to a grid's local display position.
    /// <br/> <br/>
    /// Global Space - Positions relative to the world origin.
    /// <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// </summary>
    /// <param name="position">The global position to transform.</param>
    /// <param name="displayActor">The actor the grid is local to.</param>
    /// <param name="grid">The spatial grid.</param>
    /// <returns>The global position in tile display space.</returns>
    public Matrix3x2 GlobalToDisplayTile<Tin, Tout>(Matrix3x2 position, Actor displayActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToTile(GlobalToDisplayLocal(position, displayActor), grid);

    /// <summary>
    /// Convert a global position to a grid's local display position.
    /// <br/> <br/>
    /// Global Space - Positions relative to the world origin.
    /// <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// </summary>
    /// <param name="position">The global position to transform.</param>
    /// <param name="displayActor">The actor the grid is local to.</param>
    /// <param name="grid">The spatial grid.</param>
    /// <returns>The global position in tile display space.</returns>
    public Vector2 GlobalToDisplayTile<Tin, Tout>(Vector2 position, Actor displayActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToTile(GlobalToDisplayLocal(position, displayActor), grid);
    #endregion

    #region Local
    /// <summary>
    /// Convert a local position to global space.
    /// <br/> <br/>
    /// Local Space - Positions relative to the actor's location.
    /// <br/>
    /// Global Space - Positions relative to the world origin.
    /// </summary>
    /// <param name="position">The local position to transform.</param>
    /// <param name="localActor">The actor the position is local to.</param>
    /// <returns>The local position in global space.</returns>
    public Matrix3x2 LocalToGlobal(Matrix3x2 position, Actor localActor)
        => localActor.Transform.LocalToGlobal(position);

    /// <summary>
    /// Convert a local position to global space.
    /// <br/> <br/>
    /// Local Space - Positions relative to the actor's location.
    /// <br/>
    /// Global Space - Positions relative to the world origin.
    /// </summary>
    /// <param name="position">The local position to transform.</param>
    /// <param name="localActor">The actor the position is local to.</param>
    /// <returns>The local position in global space.</returns>
    public Vector2 LocalToGlobal(Vector2 position, Actor localActor)
        => localActor.Transform.LocalToGlobal(position);

    /// <summary>
    /// Convert an actor's local position to another actor's local display space.
    /// <br/> <br/>
    /// Local Space - Positions relative to the actor's location.
    /// <br/>
    /// Display Local Space - Positions relative to the actor's location, with any enter actions applied from the actor or its parents.
    /// </summary>
    /// <param name="position">The local position to transform.</param>
    /// <param name="localActor">The actor the position is local to.</param>
    /// <param name="displayActor">The actor the position should be local to.</param>
    /// <returns>The local position converted to a different local display space.</returns>
    public Matrix3x2 LocalToDisplayLocal(Matrix3x2 position, Actor localActor, Actor displayActor)
        => displayActor.Transform.GlobalToDisplay(LocalToGlobal(position, localActor));

    /// <summary>
    /// Convert an actor's local position to another actor's local display space.
    /// <br/> <br/>
    /// Local Space - Positions relative to the actor's location.
    /// <br/>
    /// Display Local Space - Positions relative to the actor's location, with any enter actions applied from the actor or its parents.
    /// </summary>
    /// <param name="position">The local position to transform.</param>
    /// <param name="localActor">The actor the position is local to.</param>
    /// <param name="displayActor">The actor the position should be local to.</param>
    /// <returns>The local position converted to a different local display space.</returns>
    public Vector2 LocalToDisplayLocal(Vector2 position, Actor localActor, Actor displayActor)
        => displayActor.Transform.GlobalToDisplay(LocalToGlobal(position, localActor));

    /// <summary>
    /// Convert a local position into the same actor's local display space.
    /// <br/> <br/>
    /// Local Space - Positions relative to the actor's location.
    /// <br/>
    /// Display Local Space - Positions relative to the actor's location, with any enter actions applied from the actor or its parents.
    /// </summary>
    /// <param name="position">The local position to transform.</param>
    /// <param name="displayActor">The actor the position is relative to.</param>
    /// <returns>The local position converted to local display space.</returns>
    public Matrix3x2 LocalToDisplayLocal(Matrix3x2 position, Actor displayActor)
        => displayActor.Transform.LocalToDisplay(position);

    /// <summary>
    /// Convert a local position into the same actor's local display space.
    /// <br/> <br/>
    /// Local Space - Positions relative to the actor's location.
    /// <br/>
    /// Display Local Space - Positions relative to the actor's location, with any enter actions applied from the actor or its parents.
    /// </summary>
    /// <param name="position">The local position to transform.</param>
    /// <param name="displayActor">The actor the position is relative to.</param>
    /// <returns>The local position converted to local display space.</returns>
    public Vector2 LocalToDisplayLocal(Vector2 position, Actor displayActor)
        => displayActor.Transform.LocalToDisplay(position);

    /// <summary>
    /// Convert a local position into another actor's local space.
    /// <br/> <br/>
    /// Local Space - Positions relative to the actor's location.
    /// </summary>
    /// <param name="position">The local position to transform.</param>
    /// <param name="originLocalActor">The actor the position is relative to.</param>
    /// <param name="targetLocalActor">The actor the position should be relative to.</param>
    /// <returns>The local position converted to the other actor's local space.</returns>
    public Matrix3x2 LocalToLocal(Matrix3x2 position, Actor originLocalActor, Actor targetLocalActor)
        => targetLocalActor.Transform.FromOtherLocal(position, originLocalActor);

    /// <summary>
    /// Convert a local position into another actor's local space.
    /// <br/> <br/>
    /// Local Space - Positions relative to the actor's location.
    /// </summary>
    /// <param name="position">The local position to transform.</param>
    /// <param name="originLocalActor">The actor the position is relative to.</param>
    /// <param name="targetLocalActor">The actor the position should be relative to.</param>
    /// <returns>The local position converted to the other actor's local space.</returns>
    public Vector2 LocalToLocal(Vector2 position, Actor originLocalActor, Actor targetLocalActor)
        => targetLocalActor.Transform.FromOtherLocal(position, originLocalActor);

    /// <summary>
    /// Convert a local position into tile space on the same actor.
    /// <br/> <br/>
    /// Local Space - Positions relative to the actor's location.
    /// <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// </summary>
    /// <param name="position">The local position to transform.</param>
    /// <param name="grid">The spatial grid.</param>
    /// <returns>The local position in tile space.</returns>
    public Matrix3x2 LocalToTile<Tin, Tout>(Matrix3x2 position, ISpatialGrid<Tin, Tout> grid)
        => grid.LocalToTileCoord(position);

    /// <summary>
    /// Convert a local position into tile space on the same actor.
    /// <br/> <br/>
    /// Local Space - Positions relative to the actor's location.
    /// <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// </summary>
    /// <param name="position">The local position to transform.</param>
    /// <param name="grid">The spatial grid.</param>
    /// <returns>The local position in tile space.</returns>
    public Vector2 LocalToTile<Tin, Tout>(Vector2 position, ISpatialGrid<Tin, Tout> grid)
        => grid.LocalToTileCoord(position);

    /// <summary>
    /// Convert a local position into tile space local to a different actor.
    /// <br/> <br/>
    /// Local Space - Positions relative to the actor's location.
    /// <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// </summary>
    /// <param name="position">The local position to transform.</param>
    /// <param name="originLocalActor">The actor the position is relative to.</param>
    /// <param name="targetLocalActor">The actor the grid is local to.</param>
    /// <param name="grid">The spatial grid.</param>
    /// <returns>The local position in the other actor's tile space.</returns>
    public Matrix3x2 LocalToTile<Tin, Tout>(Matrix3x2 position, Actor originLocalActor, Actor targetLocalActor, ISpatialGrid<Tin, Tout> grid)
        => grid.LocalToTileCoord(LocalToLocal(position, originLocalActor, targetLocalActor));

    /// <summary>
    /// Convert a local position into tile space local to a different actor.
    /// <br/> <br/>
    /// Local Space - Positions relative to the actor's location.
    /// <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// </summary>
    /// <param name="position">The local position to transform.</param>
    /// <param name="originLocalActor">The actor the position is relative to.</param>
    /// <param name="targetLocalActor">The actor the grid is local to.</param>
    /// <param name="grid">The spatial grid.</param>
    /// <returns>The local position in the other actor's tile space.</returns>
    public Vector2 LocalToTile<Tin, Tout>(Vector2 position, Actor originLocalActor, Actor targetLocalActor, ISpatialGrid<Tin, Tout> grid)
        => grid.LocalToTileCoord(LocalToLocal(position, originLocalActor, targetLocalActor));

    /// <summary>
    /// Convert a local position into tile display space on the same actor.
    /// <br/> <br/>
    /// Local Space - Positions relative to the actor's location.
    /// <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// </summary>
    /// <param name="position">The local position to transform.</param>
    /// <param name="grid">The spatial grid.</param>
    /// <returns>The local position in tile display space.</returns>
    public Matrix3x2 LocalToDisplayTile<Tin, Tout>(Matrix3x2 position, Actor localActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToTile(LocalToDisplayLocal(position, localActor), grid);

    /// <summary>
    /// Convert a local position into tile display space on the same actor.
    /// <br/> <br/>
    /// Local Space - Positions relative to the actor's location.
    /// <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// </summary>
    /// <param name="position">The local position to transform.</param>
    /// <param name="grid">The spatial grid.</param>
    /// <returns>The local position in tile display space.</returns>
    public Vector2 LocalToDisplayTile<Tin, Tout>(Vector2 position, Actor localActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToTile(LocalToDisplayLocal(position, localActor), grid);

    /// <summary>
    /// Convert a local position into tile display space on a different actor.
    /// br/> <br/>
    /// Local Space - Positions relative to the actor's location.
    /// <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// </summary>
    /// <param name="position">The local position to transform.</param>
    /// <param name="localActor">The actor the position is relative to.</param>
    /// <param name="displayActor">The actor the grid is local to.</param>
    /// <param name="grid">The spatial grid.</param>
    /// <returns>The local position in the other actor's tile display space.</returns>
    public Matrix3x2 LocalToDisplayTile<Tin, Tout>(Matrix3x2 position, Actor localActor, Actor displayActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToTile(LocalToDisplayLocal(position, localActor, displayActor), grid);

    /// <summary>
    /// Convert a local position into tile display space on a different actor.
    /// br/> <br/>
    /// Local Space - Positions relative to the actor's location.
    /// <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// </summary>
    /// <param name="position">The local position to transform.</param>
    /// <param name="localActor">The actor the position is relative to.</param>
    /// <param name="displayActor">The actor the grid is local to.</param>
    /// <param name="grid">The spatial grid.</param>
    /// <returns>The local position in the other actor's tile display space.</returns>
    public Vector2 LocalToDisplayTile<Tin, Tout>(Vector2 position, Actor localActor, Actor displayActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToTile(LocalToDisplayLocal(position, localActor, displayActor), grid);
    #endregion

    #region DisplayLocal
    /// <summary>
    /// Convert a local display position to global space.
    /// <br/> <br/>
    /// Display Local Space - Positions relative to the actor's location, with any enter actions applied from the actor or its parents.
    /// <br/>
    /// Global Space - Positions relative to the world origin.
    /// </summary>
    /// <param name="position">The local display position to transform.</param>
    /// <param name="localActor">The actor the position is local to.</param>
    /// <returns>The local display position converted to global space.</returns>
    public Matrix3x2 DisplayLocalToGlobal(Matrix3x2 position, Actor localActor)
        => localActor.Transform.DisplayToGlobal(position);

    /// <summary>
    /// Convert a local display position to global space.
    /// <br/> <br/>
    /// Display Local Space - Positions relative to the actor's location, with any enter actions applied from the actor or its parents.
    /// <br/>
    /// Global Space - Positions relative to the world origin.
    /// </summary>
    /// <param name="position">The local display position to transform.</param>
    /// <param name="localActor">The actor the position is local to.</param>
    /// <returns>The local display position converted to global space.</returns>
    public Vector2 DisplayLocalToGlobal(Vector2 position, Actor localActor)
        => localActor.Transform.DisplayToGlobal(position);

    /// <summary>
    /// Convert a local display position to local space on the same actor.
    /// <br/> <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// <br/>
    /// Local Space - Positions relative to the actor's location.
    /// </summary>
    /// <param name="position">The local display position to transform.</param>
    /// <param name="localActor">The actor the position is local to.</param>
    /// <returns>The local display position converted to local space.</returns>
    public Matrix3x2 DisplayLocalToLocal(Matrix3x2 position, Actor localActor)
        => localActor.Transform.DisplayToLocal(position);

    /// <summary>
    /// Convert a local display position to local space on the same actor.
    /// <br/> <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// <br/>
    /// Local Space - Positions relative to the actor's location.
    /// </summary>
    /// <param name="position">The local display position to transform.</param>
    /// <param name="localActor">The actor the position is local to.</param>
    /// <returns>The local display position converted to local space.</returns>
    public Vector2 DisplayLocalToLocal(Vector2 position, Actor localActor)
        => localActor.Transform.DisplayToLocal(position);

    /// <summary>
    /// Convert a local display position to local space on a different actor.
    /// <br/> <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// <br/>
    /// Local Space - Positions relative to the actor's location.
    /// </summary>
    /// <param name="position">The local display position to transform.</param>
    /// <param name="localActor">The actor the position is local to.</param>
    /// <param name="displayActor">The actor the position should be local to.</param>
    /// <returns>The local display position converted to local space on the target actor.</returns>
    public Matrix3x2 DisplayLocalToLocal(Matrix3x2 position, Actor localActor, Actor displayActor)
        => GlobalToLocal(DisplayLocalToGlobal(position, displayActor), localActor);

    /// <summary>
    /// Convert a local display position to local space on a different actor.
    /// <br/> <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// <br/>
    /// Local Space - Positions relative to the actor's location.
    /// </summary>
    /// <param name="position">The local display position to transform.</param>
    /// <param name="localActor">The actor the position is local to.</param>
    /// <param name="displayActor">The actor the position should be local to.</param>
    /// <returns>The local display position converted to local space on the target actor.</returns>
    public Vector2 DisplayLocalToLocal(Vector2 position, Actor localActor, Actor displayActor)
        => GlobalToLocal(DisplayLocalToGlobal(position, displayActor), localActor);

    /// <summary>
    /// Convert a local display position to local display space on a different actor.
    /// <br/> <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// </summary>
    /// <param name="position">The local display position to transform.</param>
    /// <param name="originLocalActor">The actor the position is local to.</param>
    /// <param name="targetLocalActor"The actor the position should be local to.></param>
    /// <returns>The local display position converted to local display space on the target actor.</returns>
    public Matrix3x2 DisplayLocalToDisplayLocal(Matrix3x2 position, Actor originLocalActor, Actor targetLocalActor)
        => targetLocalActor.Transform.GlobalToDisplay(originLocalActor.Transform.DisplayToGlobal(position));

    /// <summary>
    /// Convert a local display position to local display space on a different actor.
    /// <br/> <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// </summary>
    /// <param name="position">The local display position to transform.</param>
    /// <param name="originLocalActor">The actor the position is local to.</param>
    /// <param name="targetLocalActor"The actor the position should be local to.></param>
    /// <returns>The local display position converted to local display space on the target actor.</returns>
    public Vector2 DisplayLocalToDisplayLocal(Vector2 position, Actor originLocalActor, Actor targetLocalActor)
        => targetLocalActor.Transform.GlobalToDisplay(originLocalActor.Transform.DisplayToGlobal(position));

    /// <summary>
    /// Convert a local display position to tile space on the same actor.
    /// <br/> <br/>
    /// Display Local Space - Positions relative to the actor's location, with any enter actions applied from the actor or its parents.
    /// <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// </summary>
    /// <param name="position">The local display position to transform.</param>
    /// <param name="localActor">The actor the position is local to.</param>
    /// <param name="grid">The spatial grid.</param>
    /// <returns>The local display position converted to tile space.</returns>
    public Matrix3x2 DisplayLocalToTile<Tin, Tout>(Matrix3x2 position, Actor localActor, ISpatialGrid<Tin, Tout> grid)
        => grid.LocalToTileCoord(localActor.Transform.DisplayToLocal(position));

    /// <summary>
    /// Convert a local display position to tile space on the same actor.
    /// <br/> <br/>
    /// Display Local Space - Positions relative to the actor's location, with any enter actions applied from the actor or its parents.
    /// <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// </summary>
    /// <param name="position">The local display position to transform.</param>
    /// <param name="localActor">The actor the position is local to.</param>
    /// <param name="grid">The spatial grid.</param>
    /// <returns>The local display position converted to tile space.</returns>
    public Vector2 DisplayLocalToTile<Tin, Tout>(Vector2 position, Actor localActor, ISpatialGrid<Tin, Tout> grid)
        => grid.LocalToTileCoord(localActor.Transform.DisplayToLocal(position));

    /// <summary>
    /// Convert a local display position to tile space on a different actor.
    /// <br/> <br/>
    /// Display Local Space - Positions relative to the actor's location, with any enter actions applied from the actor or its parents.
    /// <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// </summary>
    /// <param name="position">The local display position to transform.</param>
    /// <param name="originLocalActor">The actor the position is local to.</param>
    /// <param name="targetLocalActor"The actor the position should be local to.></param>
    /// <param name="grid">The spatial grid.</param>
    /// <returns>The local display position converted to tile space on the target actor.</returns>
    public Matrix3x2 DisplayLocalToTile<Tin, Tout>(Matrix3x2 position, Actor originLocalActor, Actor targetLocalActor, ISpatialGrid<Tin, Tout> grid)
        => DisplayLocalToTile(LocalToLocal(position, originLocalActor, targetLocalActor), targetLocalActor, grid);

    /// <summary>
    /// Convert a local display position to tile space on a different actor.
    /// <br/> <br/>
    /// Display Local Space - Positions relative to the actor's location, with any enter actions applied from the actor or its parents.
    /// <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// </summary>
    /// <param name="position">The local display position to transform.</param>
    /// <param name="originLocalActor">The actor the position is local to.</param>
    /// <param name="targetLocalActor"The actor the position should be local to.></param>
    /// <param name="grid">The spatial grid.</param>
    /// <returns>The local display position converted to tile space on the target actor.</returns>
    public Vector2 DisplayLocalToTile<Tin, Tout>(Vector2 position, Actor originLocalActor, Actor targetLocalActor, ISpatialGrid<Tin, Tout> grid)
        => DisplayLocalToTile(LocalToLocal(position, originLocalActor, targetLocalActor), targetLocalActor, grid);

    /// <summary>
    /// Convert a local display position to tile display space on the same actor.
    /// <br/> <br/>
    /// Display Local Space - Positions relative to the actor's location, with any enter actions applied from the actor or its parents.
    /// <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// </summary>
    /// <param name="position">The local display position to transform.</param>
    /// <param name="grid">The spatial grid.</param>
    /// <returns>The local display position converted to tile display space</returns>
    public Matrix3x2 DisplayLocalToDisplayTile<Tin, Tout>(Matrix3x2 position, ISpatialGrid<Tin, Tout> grid)
        => LocalToTile(position, grid);

    /// <summary>
    /// Convert a local display position to tile display space on the same actor.
    /// <br/> <br/>
    /// Display Local Space - Positions relative to the actor's location, with any enter actions applied from the actor or its parents.
    /// <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// </summary>
    /// <param name="position">The local display position to transform.</param>
    /// <param name="grid">The spatial grid.</param>
    /// <returns>The local display position converted to tile display space</returns>
    public Vector2 DisplayLocalToDisplayTile<Tin, Tout>(Vector2 position, ISpatialGrid<Tin, Tout> grid)
        => LocalToTile(position, grid);

    /// <summary>
    /// Convert a local display position to tile display space on a different actor.
    /// <br/> <br/>
    /// Display Local Space - Positions relative to the actor's location, with any enter actions applied from the actor or its parents.
    /// <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// </summary>
    /// <param name="position">The local display position to transform.</param>
    /// <param name="originDisplayActor">The actor the position is local to.</param>
    /// <param name="targetDisplayActor"The actor the position should be local to.></param>
    /// <param name="grid">The spatial grid.</param>
    /// <returns>The local display position converted to local display space on the target actor.</returns>
    public Matrix3x2 DisplayLocalToDisplayTile<Tin, Tout>(Matrix3x2 position, Actor originDisplayActor, Actor targetDisplayActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToTile(DisplayLocalToDisplayLocal(position, originDisplayActor, targetDisplayActor), grid);

    /// <summary>
    /// Convert a local display position to tile display space on a different actor.
    /// <br/> <br/>
    /// Display Local Space - Positions relative to the actor's location, with any enter actions applied from the actor or its parents.
    /// <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// </summary>
    /// <param name="position">The local display position to transform.</param>
    /// <param name="originDisplayActor">The actor the position is local to.</param>
    /// <param name="targetDisplayActor"The actor the position should be local to.></param>
    /// <param name="grid">The spatial grid.</param>
    /// <returns>The local display position converted to local display space on the target actor.</returns>
    public Vector2 DisplayLocalToDisplayTile<Tin, Tout>(Vector2 position, Actor originDisplayActor, Actor targetDisplayActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToTile(DisplayLocalToDisplayLocal(position, originDisplayActor, targetDisplayActor), grid);
    #endregion

    #region Tile
    /// <summary>
    /// Convert a tile position to local space on the same actor.
    /// <br/> <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// <br/>
    /// Local Space - Positions relative to the actor's location.
    /// </summary>
    /// <param name="position">The tile position to transform.</param>
    /// <param name="grid">The spatial grid.</param>
    /// <returns>The tile position in local space.</returns>
    public Matrix3x2 TileToLocal<Tin, Tout>(Matrix3x2 position, ISpatialGrid<Tin, Tout> grid)
        => grid.TileCoordToLocal(position);

    /// <summary>
    /// Convert a tile position to local space on the same actor.
    /// <br/> <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// <br/>
    /// Local Space - Positions relative to the actor's location.
    /// </summary>
    /// <param name="position">The tile position to transform.</param>
    /// <param name="grid">The spatial grid.</param>
    /// <returns>The tile position in local space.</returns>
    public Vector2 TileToLocal<Tin, Tout>(Vector2 position, ISpatialGrid<Tin, Tout> grid)
        => grid.TileCoordToLocal(position);

    /// <summary>
    /// Convert a tile position to local space on a different actor.
    /// <br/> <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// <br/>
    /// Local Space - Positions relative to the actor's location.
    /// </summary>
    /// <param name="position">The tile position to transform.</param>
    /// <param name="originLocalActor">The actor the position is local to.</param>
    /// <param name="targetLocalActor">The actor the position should be local to.</param>
    /// <param name="grid">The spatial grid.</param>
    /// <returns>The tile position in local space on the target actor.</returns>
    public Matrix3x2 TileToLocal<Tin, Tout>(Matrix3x2 position, Actor originLocalActor, Actor targetLocalActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToLocal(TileToLocal(position, grid), originLocalActor, targetLocalActor);

    /// <summary>
    /// Convert a tile position to local space on a different actor.
    /// <br/> <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// <br/>
    /// Local Space - Positions relative to the actor's location.
    /// </summary>
    /// <param name="position">The tile position to transform.</param>
    /// <param name="originLocalActor">The actor the position is local to.</param>
    /// <param name="targetLocalActor">The actor the position should be local to.</param>
    /// <param name="grid">The spatial grid.</param>
    /// <returns>The tile position in local space on the target actor.</returns>
    public Vector2 TileToLocal<Tin, Tout>(Vector2 position, Actor originLocalActor, Actor targetLocalActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToLocal(TileToLocal(position, grid), originLocalActor, targetLocalActor);

    /// <summary>
    /// Convert a tile position to global space
    /// <br/> <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// <br/>
    /// Global Space - Positions relative to the world origin.
    /// </summary>
    /// <param name="position">The tile position to transform.</param>
    /// <param name="localActor">the actor the grid is local to.</param>
    /// <param name="grid">The spatial grid.</param>
    /// <returns>The tile position in global space.</returns>
    public Matrix3x2 TileToGlobal<Tin, Tout>(Matrix3x2 position, Actor localActor, ISpatialGrid<Tin, Tout> grid)
        => localActor.Transform.LocalToGlobal(grid.TileCoordToLocal(position));

    /// <summary>
    /// Convert a tile position to global space
    /// <br/> <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// <br/>
    /// Global Space - Positions relative to the world origin.
    /// </summary>
    /// <param name="position">The tile position to transform.</param>
    /// <param name="localActor">the actor the grid is local to.</param>
    /// <param name="grid">The spatial grid.</param>
    /// <returns>The tile position in global space.</returns>
    public Vector2 TileToGlobal<Tin, Tout>(Vector2 position, Actor localActor, ISpatialGrid<Tin, Tout> grid)
        => localActor.Transform.LocalToGlobal(grid.TileCoordToLocal(position));

    /// <summary>
    /// Convert a tile position to another tile space on the same actor.
    /// <br/> <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// </summary>
    /// <param name="position">The tile position to transform.</param>
    /// <param name="originGrid">The spatial grid the position is local to.</param>
    /// <param name="targetGrid">The spatial grid the position should be local to.</param>
    /// <returns>The tile position in tile space on the target grid.</returns>
    public Matrix3x2 TileToTile<Tin, Tout>(Matrix3x2 position, ISpatialGrid<Tin, Tout> originGrid, ISpatialGrid<Tin, Tout> targetGrid)
        => targetGrid.LocalToTileCoord(originGrid.TileCoordToLocal(position));

    /// <summary>
    /// Convert a tile position to another tile space on the same actor.
    /// <br/> <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// </summary>
    /// <param name="position">The tile position to transform.</param>
    /// <param name="originGrid">The spatial grid the position is local to.</param>
    /// <param name="targetGrid">The spatial grid the position should be local to.</param>
    /// <returns>The tile position in tile space on the target grid.</returns>
    public Vector2 TileToTile<Tin, Tout>(Vector2 position, ISpatialGrid<Tin, Tout> originGrid, ISpatialGrid<Tin, Tout> targetGrid)
        => targetGrid.LocalToTileCoord(originGrid.TileCoordToLocal(position));

    /// <summary>
    /// Convert a tile position to another tile space on a different actor.
    /// <br/> <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// </summary>
    /// <param name="position">The tile position to transform.</param>
    /// <param name="originLocalActor">The actor the origin grid is local to.</param>
    /// <param name="targetLocalActor">The actor the target grid is local to.</param>
    /// <param name="originGrid">The spatial grid the position is local to.</param>
    /// <param name="targetGrid">The spatial grid the position should be local to.</param>
    /// <returns>The tile position in tile space on the target grid.</returns>
    public Matrix3x2 TileToTile<Tin, Tout>(Matrix3x2 position, Actor originLocalActor, Actor targetLocalActor, ISpatialGrid<Tin, Tout> originGrid, ISpatialGrid<Tin, Tout> targetGrid)
        => GlobalToTile(TileToGlobal(position, originLocalActor, originGrid), targetLocalActor, targetGrid);

    /// <summary>
    /// Convert a tile position to another tile space on a different actor.
    /// <br/> <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// </summary>
    /// <param name="position">The tile position to transform.</param>
    /// <param name="originLocalActor">The actor the origin grid is local to.</param>
    /// <param name="targetLocalActor">The actor the target grid is local to.</param>
    /// <param name="originGrid">The spatial grid the position is local to.</param>
    /// <param name="targetGrid">The spatial grid the position should be local to.</param>
    /// <returns>The tile position in tile space on the target grid.</returns>
    public Vector2 TileToTile<Tin, Tout>(Vector2 position, Actor originLocalActor, Actor targetLocalActor, ISpatialGrid<Tin, Tout> originGrid, ISpatialGrid<Tin, Tout> targetGrid)
         => GlobalToTile(TileToGlobal(position, originLocalActor, originGrid), targetLocalActor, targetGrid);

    /// <summary>
    /// Convert a tile position to local display space on the same actor.
    /// <br/> <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// <br/>
    /// Display Local Space - Positions relative to the actor's location, with any enter actions applied from the actor or its parents.
    /// </summary>
    /// <param name="position">The tile position to transform.</param>
    /// <param name="localActor">The actor the grid is local to.</param>
    /// <param name="grid">The spatial grid.</param>
    /// <returns>The tile position in local display space.</returns>
    public Matrix3x2 TileToDisplayLocal<Tin, Tout>(Matrix3x2 position, Actor localActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToDisplayLocal(grid.TileCoordToLocal(position), localActor);

    /// <summary>
    /// Convert a tile position to local display space on the same actor.
    /// <br/> <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// <br/>
    /// Display Local Space - Positions relative to the actor's location, with any enter actions applied from the actor or its parents.
    /// </summary>
    /// <param name="position">The tile position to transform.</param>
    /// <param name="localActor">The actor the grid is local to.</param>
    /// <param name="grid">The spatial grid.</param>
    /// <returns>The tile position in local display space.</returns>
    public Vector2 TileToDisplayLocal<Tin, Tout>(Vector2 position, Actor localActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToDisplayLocal(grid.TileCoordToLocal(position), localActor);

    /// <summary>
    /// Convert a tile position to local display space on a different actor.
    /// <br/> <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// <br/>
    /// Display Local Space - Positions relative to the actor's location, with any enter actions applied from the actor or its parents.
    /// </summary>
    /// <param name="position">The tile position to transform.</param>
    /// <param name="originLocalActor">The actor the grid is local to.</param>
    /// <param name="targetLocalActor">The actor the position should be relative to.</param>
    /// <param name="grid">The spatial grid.</param>
    /// <returns>The tile position in local display space on the target actor.</returns>
    public Matrix3x2 TileToDisplayLocal<Tin, Tout>(Matrix3x2 position, Actor originLocalActor, Actor targetLocalActor, ISpatialGrid<Tin, Tout> grid)
        => GlobalToDisplayLocal(TileToGlobal(position, originLocalActor, grid), targetLocalActor);

    /// <summary>
    /// Convert a tile position to local display space on a different actor.
    /// <br/> <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// <br/>
    /// Display Local Space - Positions relative to the actor's location, with any enter actions applied from the actor or its parents.
    /// </summary>
    /// <param name="position">The tile position to transform.</param>
    /// <param name="originLocalActor">The actor the grid is local to.</param>
    /// <param name="targetLocalActor">The actor the position should be relative to.</param>
    /// <param name="grid">The spatial grid.</param>
    /// <returns>The tile position in local display space on the target actor.</returns>
    public Vector2 TileToDisplayLocal<Tin, Tout>(Vector2 position, Actor originLocalActor, Actor targetLocalActor, ISpatialGrid<Tin, Tout> grid)
        => GlobalToDisplayLocal(TileToGlobal(position, originLocalActor, grid), targetLocalActor);

    /// <summary>
    /// Convert a tile position to tile display space on the same grid.
    /// <br/> <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// </summary>
    /// <param name="position">The tile position to transform.</param>
    /// <param name="localActor">The actor the grid is local to.</param>
    /// <param name="grid">The spatial grid.</param>
    /// <returns>The tile position in tile display space.</returns>
    public Matrix3x2 TileToDisplayTile<Tin, Tout>(Matrix3x2 position, Actor localActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToTile(TileToDisplayLocal(position, localActor, grid), grid);

    /// <summary>
    /// Convert a tile position to tile display space on the same grid.
    /// <br/> <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// </summary>
    /// <param name="position">The tile position to transform.</param>
    /// <param name="localActor">The actor the grid is local to.</param>
    /// <param name="grid">The spatial grid.</param>
    /// <returns>The tile position in tile display space.</returns>
    public Vector2 TileToDisplayTile<Tin, Tout>(Vector2 position, Actor localActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToTile(TileToDisplayLocal(position, localActor, grid), grid);

    /// <summary>
    /// Convert a tile position to tile display space on a different grid on the same actor.
    /// <br/> <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// </summary>
    /// <param name="position">The tile position to transform.</param>
    /// <param name="localActor">The actor the grids are local to.</param>
    /// <param name="localGrid">The spatial grid the position is local to.</param>
    /// <param name="displayGrid">The spatial grid the position should be local to.</param>
    /// <returns>The tile position in tile display space on the target grid..</returns>
    public Matrix3x2 TileToDisplayTile<Tin, Tout>(Matrix3x2 position, Actor localActor, ISpatialGrid<Tin, Tout> localGrid, ISpatialGrid<Tin, Tout> displayGrid)
        => LocalToTile(TileToDisplayLocal(position, localActor, localGrid), displayGrid);

    /// <summary>
    /// Convert a tile position to tile display space on a different grid on the same actor.
    /// <br/> <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// </summary>
    /// <param name="position">The tile position to transform.</param>
    /// <param name="localActor">The actor the grids are local to.</param>
    /// <param name="localGrid">The spatial grid the position is local to.</param>
    /// <param name="displayGrid">The spatial grid the position should be local to.</param>
    /// <returns>The tile position in tile display space on the target grid.</returns>
    public Vector2 TileToDisplayTile<Tin, Tout>(Vector2 position, Actor localActor, ISpatialGrid<Tin, Tout> localGrid, ISpatialGrid<Tin, Tout> displayGrid)
        => LocalToTile(TileToDisplayLocal(position, localActor, localGrid), displayGrid);

    /// <summary>
    /// Convert a tile position to tile display space on a different actor.
    /// <br/> <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// </summary>
    /// <param name="position">The tile position to transform.</param>
    /// <param name="localActor">The actor the local grid is relative to.</param>
    /// <param name="displayActor">The actor the display grid is relative to.</param>
    /// <param name="localGrid">The spatial grid the position is local to.</param>
    /// <param name="displayGrid">The spatial grid the position should be local to.</param>
    /// <returns>The tile position in tile display space on the target grid.</returns>
    public Matrix3x2 TileToDisplayTile<Tin, Tout>(Matrix3x2 position, Actor localActor, Actor displayActor, ISpatialGrid<Tin, Tout> localGrid, ISpatialGrid<Tin, Tout> displayGrid)
        => LocalToTile(TileToDisplayLocal(position, localActor, displayActor, localGrid), displayGrid);

    /// <summary>
    /// Convert a tile position to tile display space on a different actor.
    /// <br/> <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// </summary>
    /// <param name="position">The tile position to transform.</param>
    /// <param name="localActor">The actor the local grid is relative to.</param>
    /// <param name="displayActor">The actor the display grid is relative to.</param>
    /// <param name="localGrid">The spatial grid the position is local to.</param>
    /// <param name="displayGrid">The spatial grid the position should be local to.</param>
    /// <returns>The tile position in tile display space on the target grid.</returns>
    public Vector2 TileToDisplayTile<Tin, Tout>(Vector2 position, Actor localActor, Actor displayActor, ISpatialGrid<Tin, Tout> localGrid, ISpatialGrid<Tin, Tout> displayGrid)
        => LocalToTile(TileToDisplayLocal(position, localActor, displayActor, localGrid), displayGrid);
    #endregion

    #region DisplayTile
    /// <summary>
    /// Convert a tile display position to global space.
    /// <br/> <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// <br/>
    /// Global Space - Positions relative to the world origin.
    /// </summary>
    /// <param name="position">The tile display position to transform.</param>
    /// <param name="displayActor">The actor the grid is relative to.</param>
    /// <param name="grid">the spatial grid.</param>
    /// <returns>The tile display position in global space.</returns>
    public Matrix3x2 DisplayTileToGlobal<Tin, Tout>(Matrix3x2 position, Actor displayActor, ISpatialGrid<Tin, Tout> grid)
        => DisplayLocalToGlobal(DisplayTileToDisplayLocal(position, grid), displayActor);
    /// <summary>
    /// Convert a tile display position to global space.
    /// <br/> <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// <br/>
    /// Global Space - Positions relative to the world origin.
    /// </summary>
    /// <param name="position">The tile display position to transform.</param>
    /// <param name="displayActor">The actor the grid is relative to.</param>
    /// <param name="grid">the spatial grid.</param>
    /// <returns>The tile display position in global space.</returns>
    public Vector2 DisplayTileToGlobal<Tin, Tout>(Vector2 position, Actor displayActor, ISpatialGrid<Tin, Tout> grid)
        => DisplayLocalToGlobal(DisplayTileToDisplayLocal(position, grid), displayActor);

    /// <summary>
    /// Convert a tile display position to local space on the same actor.
    /// <br/> <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// <br/>
    /// Local Space - Positions relative to the actor's location.
    /// </summary>
    /// <param name="position">The tile display position to transform.</param>
    /// <param name="displayActor">The actor the grid is relative to.</param>
    /// <param name="grid">the spatial grid.</param>
    /// <returns>The tile display position in local space.</returns>
    public Matrix3x2 DisplayTileToLocal<Tin, Tout>(Matrix3x2 position, Actor displayActor, ISpatialGrid<Tin, Tout> grid)
        => GlobalToLocal(DisplayTileToGlobal(position, displayActor, grid), displayActor);
    /// <summary>
    /// Convert a tile display position to local space on the same actor.
    /// <br/> <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// <br/>
    /// Local Space - Positions relative to the actor's location.
    /// </summary>
    /// <param name="position">The tile display position to transform.</param>
    /// <param name="displayActor">The actor the grid is relative to.</param>
    /// <param name="grid">the spatial grid.</param>
    /// <returns>The tile display position in local space.</returns>
    public Vector2 DisplayTileToLocal<Tin, Tout>(Vector2 position, Actor displayActor, ISpatialGrid<Tin, Tout> grid)
        => GlobalToLocal(DisplayTileToGlobal(position, displayActor, grid), displayActor);

    /// <summary>
    /// Convert a tile display position to local space on a different actor.
    /// <br/> <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// <br/>
    /// Local Space - Positions relative to the actor's location.
    /// </summary>
    /// <param name="position">The tile display position to transform.</param>
    /// <param name="displayActor">The actor the grid is relative to.</param>
    /// <param name="localActor">The actor the position should be local to.</param>
    /// <param name="grid">the spatial grid.</param>
    /// <returns>The tile display position in local space on the target actor.</returns>
    public Matrix3x2 DisplayTileToLocal<Tin, Tout>(Matrix3x2 position, Actor displayActor, Actor localActor, ISpatialGrid<Tin, Tout> grid)
        => GlobalToLocal(DisplayTileToGlobal(position, displayActor, grid), localActor);

    /// <summary>
    /// Convert a tile display position to local space on a different actor.
    /// <br/> <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// <br/>
    /// Local Space - Positions relative to the actor's location.
    /// </summary>
    /// <param name="position">The tile display position to transform.</param>
    /// <param name="displayActor">The actor the grid is relative to.</param>
    /// <param name="localActor">The actor the position should be local to.</param>
    /// <param name="grid">the spatial grid.</param>
    /// <returns>The tile display position in local space on the target actor.</returns>
    public Vector2 DisplayTileToLocal<Tin, Tout>(Vector2 position, Actor displayActor, Actor localActor, ISpatialGrid<Tin, Tout> grid)
        => GlobalToLocal(DisplayTileToGlobal(position, displayActor, grid), localActor);

    /// <summary>
    /// Convert a tile display position to local display space on the same actor.
    /// <br/> <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// <br/>
    /// Display Local Space - Positions relative to the actor's location, with any enter actions applied from the actor or its parents.
    /// </summary>
    /// <param name="position">The tile display position to transform.</param>
    /// <param name="grid">the spatial grid.</param>
    /// <returns>The tile display position in local display space.</returns>
    public Matrix3x2 DisplayTileToDisplayLocal<Tin, Tout>(Matrix3x2 position, ISpatialGrid<Tin, Tout> grid)
        => TileToLocal(position, grid);

    /// <summary>
    /// Convert a tile display position to local display space on the same actor.
    /// <br/> <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// <br/>
    /// Display Local Space - Positions relative to the actor's location, with any enter actions applied from the actor or its parents.
    /// </summary>
    /// <param name="position">The tile display position to transform.</param>
    /// <param name="grid">the spatial grid.</param>
    /// <returns>The tile display position in local display space.</returns>
    public Vector2 DisplayTileToDisplayLocal<Tin, Tout>(Vector2 position, ISpatialGrid<Tin, Tout> grid)
        => TileToLocal(position, grid);

    /// <summary>
    /// Convert a tile display position to tile space on the same grid.
    /// <br/> <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// </summary>
    /// <param name="position">The tile display position to transform.</param>
    /// <param name="displayActor">The actor the grid is relative to.</param>
    /// <param name="grid">the spatial grid.</param>
    /// <returns>The tile display position in tile space.</returns>
    public Matrix3x2 DisplayTileToTile<Tin, Tout>(Matrix3x2 position, Actor displayActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToTile(DisplayTileToLocal(position, displayActor, grid), grid);

    /// <summary>
    /// Convert a tile display position to tile space on the same grid.
    /// <br/> <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// </summary>
    /// <param name="position">The tile display position to transform.</param>
    /// <param name="displayActor">The actor the grid is relative to.</param>
    /// <param name="grid">the spatial grid.</param>
    /// <returns>The tile display position in tile space.</returns>
    public Vector2 DisplayTileToTile<Tin, Tout>(Vector2 position, Actor displayActor, ISpatialGrid<Tin, Tout> grid)
        => LocalToTile(DisplayTileToLocal(position, displayActor, grid), grid);

    /// <summary>
    /// Convert a tile display position to tile space on a different actor.
    /// <br/> <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// </summary>
    /// <param name="position">The tile display position to transform.</param>
    /// <param name="displayActor">The actor the display grid is local to.</param>
    /// <param name="localActor">The actor the local grid is local to.</param>
    /// <param name="displayGrid">The grid the position is local to.</param>
    /// <param name="localGrid">The grid the position should be local to.</param>
    /// <returns>The tile display position in tile space on the target actor.</returns>
    public Matrix3x2 DisplayTileToTile<Tin, Tout>(Matrix3x2 position, Actor displayActor, Actor localActor, ISpatialGrid<Tin, Tout> displayGrid, ISpatialGrid<Tin, Tout> localGrid)
        => LocalToTile(DisplayTileToLocal(position, displayActor, localActor, displayGrid), localGrid);

    /// <summary>
    /// Convert a tile display position to tile space on a different actor.
    /// <br/> <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// <br/>
    /// Tile Space - Positions relative to a spatial grid with one unit per tile.
    /// </summary>
    /// <param name="position">The tile display position to transform.</param>
    /// <param name="displayActor">The actor the display grid is local to.</param>
    /// <param name="localActor">The actor the local grid is local to.</param>
    /// <param name="displayGrid">The grid the position is local to.</param>
    /// <param name="localGrid">The grid the position should be local to.</param>
    /// <returns>The tile display position in tile space on the target actor.</returns>
    public Vector2 DisplayTileToTile<Tin, Tout>(Vector2 position, Actor displayActor, ISpatialGrid<Tin, Tout> displayGrid, Actor localActor, ISpatialGrid<Tin, Tout> localGrid)
        => LocalToTile(DisplayTileToLocal(position, displayActor, localActor, displayGrid), localGrid);

    /// <summary>
    /// Convert a tile display position to tile display space on a different actor.
    /// <br/> <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// </summary>
    /// <param name="position">The tile display position to transform.</param>
    /// <param name="originActor">The actor the origin grid is local to.</param>
    /// <param name="targetActor">The actor the target grid is local to.</param>
    /// <param name="originGrid">The grid the position is local to.</param>
    /// <param name="targetGrid">The grid the position should be local to.</param>
    /// <returns>The tile display position in tile display space on the target actor.</returns>
    public Matrix3x2 DisplayTileToDisplayTile<Tin, Tout>(Matrix3x2 position, Actor originActor, Actor targetActor, ISpatialGrid<Tin, Tout> originGrid, ISpatialGrid<Tin, Tout> targetGrid)
        => GlobalToDisplayTile(DisplayTileToGlobal(position, originActor, originGrid), targetActor, targetGrid);

    /// <summary>
    /// Convert a tile display position to tile display space on a different actor.
    /// <br/> <br/>
    /// Display Tile Space - Positions relative to a spatial grid with one unit per tile, transformed by an actor's display position.
    /// </summary>
    /// <param name="position">The tile display position to transform.</param>
    /// <param name="originActor">The actor the origin grid is local to.</param>
    /// <param name="targetActor">The actor the target grid is local to.</param>
    /// <param name="originGrid">The grid the position is local to.</param>
    /// <param name="targetGrid">The grid the position should be local to.</param>
    /// <returns>The tile display position in tile display space on the target actor.</returns>
    public Vector2 DisplayTileToDisplayTile<Tin, Tout>(Vector2 position, Actor originActor, Actor targetActor, ISpatialGrid<Tin, Tout> originGrid, ISpatialGrid<Tin, Tout> targetGrid)
        => GlobalToDisplayTile(DisplayTileToGlobal(position, originActor, originGrid), targetActor, targetGrid);
    #endregion
}