using System.Numerics;
using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

/// <summary>
/// Loaded instance of an LDTK tile grid layer.
/// </summary>
public class LDTKTileLayer : Component, ISpatialGrid<Point2, LDTKTile> {
    readonly GridCollider colliderGrid;
    readonly CollisionComponent collider;
    readonly GridRendererComponent renderer;

    readonly Grid<LDTKTile> Tiles;
    readonly LDTKTileset Tileset;

    Vector2 position;

    /// <summary>
    /// The offset from the actor origin.
    /// </summary>
    public Vector2 Position {
        get => position;
        set => position = value;
    }

    Vector2 tileSize;

    /// <summary>
    /// The width and height of a single tile.
    /// </summary>
    public Vector2 TileSize {
        get => tileSize;
        set => tileSize = value;
    }

    /// <summary>
    /// The x and y tilecount of the grid.
    /// </summary>
    public Point2 GridSize => Tiles.Size;

    /// <summary>
    /// The total size the layer covers.
    /// </summary>
    public Vector2 Size => Tiles.Size * tileSize;

    /// <summary>
    /// Loaded instance of an LDTK tile grid layer.
    /// </summary>
    /// <param name="game">The game instance.</param>
    /// <param name="tileset">The tileset the layer is using.</param>
    /// <param name="gridSize">The grid dimensions.</param>
    /// <param name="tileSize">The size of an individual grid tile.</param>
    /// <param name="position">The grid's position relative to the level position.</param>
    public LDTKTileLayer(Game game, LDTKTileset tileset, Point2 gridSize, int tileSize, Vector2 position) : base(game) {
        this.tileSize = new(tileSize);
        Tiles = new(gridSize);
        Tileset = tileset;
        this.position = position;

        colliderGrid = new GridCollider(gridSize, this.tileSize);
        collider = new(game, colliderGrid);
        renderer = new(game, (Point2)position, gridSize, (Point2)this.tileSize);
    }

    protected override void Added() {
        if (collider.ActorValid) { collider.Actor.Components.Remove(collider); }
        if (renderer.ActorValid) { renderer.Actor.Components.Remove(renderer); }
        Actor.Components.Add(collider);
        Actor.Components.Add(renderer);
    }

    protected override void Removed() {
        Actor.Components.Remove(collider);
        Actor.Components.Remove(renderer);
    }

    /// <summary>
    /// Set a tile to a new value.
    /// </summary>
    /// <param name="tilesetCoord">The tile coordinate from the tileset.</param>
    /// <param name="gridCoord">The location on the grid to set.</param>
    public void SetTile(Point2 tilesetCoord, Point2 gridCoord) {
        LDTKTileElement? tileElement = Tileset?.Get(tilesetCoord);
        if (tileElement == null) { return; }
        LDTKTile tile = new(tileElement);
        Tiles.Set(tile, gridCoord);
        UpdateGrids(tile, gridCoord);
    }

    /// <summary>
    /// Stack another tile onto an existing tile.
    /// </summary>
    /// <param name="tilesetCoord">The tile coordinate from the tileset.</param>
    /// <param name="gridCoord">The location on the grid to stack.</param>
    public void AddTileStack(Point2 tilesetCoord, Point2 gridCoord) {
        LDTKTileElement? tileElement = Tileset?.Get(tilesetCoord);
        if (tileElement == null) { return; }
        LDTKTile? tile = Tiles.Get(gridCoord);
        if (tile == null) {
            tile = new();
            Tiles.Set(tile, gridCoord);
        }
        tile.Add(tileElement);
        UpdateGrids(tile, gridCoord);
    }

    /// <summary>
    /// Remove the top element of a stack of tiles.
    /// </summary>
    /// <param name="gridCoord">The location to remove from.</param>
    public void RemoveTileStack(Point2 gridCoord) {
        LDTKTile? tile = Tiles.Get(gridCoord);
        if (tile?.Remove() ?? false) {
            if (tile.Empty) {
                tile = null;
                Tiles.Set(null, gridCoord);
            }
            UpdateGrids(tile, gridCoord);
        }
    }

    /// <summary>
    /// Reset a tile to empty.
    /// </summary>
    /// <param name="gridCoord">The location to reset.</param>
    public void ClearTile(Point2 gridCoord) {
        Tiles.Set(null, gridCoord);
        UpdateGrids(null, gridCoord);
    }

    /// <summary>
    /// Get the current data of a tile.
    /// </summary>
    /// <param name="gridCoord">The tile location to read from.</param>
    /// <returns>The tile instance at the given location, null if out of bounds or not assigned.</returns>
    public LDTKTile? GetTile(Point2 gridCoord) {
        return Tiles.Get(gridCoord);
    }

    void UpdateGrids(LDTKTile? tile, Point2 gridCoord) {
        colliderGrid.SetTile(tile?.Collider, gridCoord);
        renderer.SetTile(tile?.Sprite, gridCoord);
    }
}