using System.Numerics;
using Foster.Framework;

namespace LittleLib.Loader.LDTK;

public class LDTKTileLayer : Component, ISpatialGrid<Point2, LDTKTile> {
    readonly GridCollider colliderGrid;
    readonly CollisionComponent collider;
    readonly GridRendererComponent renderer;

    readonly Grid<LDTKTile> Tiles;
    readonly LDTKTileset Tileset;

    Vector2 position;
    public Vector2 Position {
        get => position;
        set => position = value;
    }

    Vector2 tileSize;
    public Vector2 TileSize {
        get => tileSize;
        set => tileSize = value;
    }

    public Point2 GridSize => Tiles.Size;
    public Vector2 Size => Tiles.Size * tileSize;

    public LDTKTileLayer(LittleGame game, LDTKTileset tileset, Point2 gridSize, int tileSize, Vector2 position) : base(game) {
        this.tileSize = new(tileSize);
        Tiles = new(gridSize);
        Tileset = tileset;
        this.position = position;

        colliderGrid = new GridCollider(gridSize, this.tileSize);
        collider = new(game, colliderGrid);
        renderer = new(game, (Point2)position, gridSize, (Point2)this.tileSize);
    }

    public override void Added() {
        if (collider.ActorValid) { collider.Actor.Components.Remove(collider); }
        if (renderer.ActorValid) { renderer.Actor.Components.Remove(renderer); }
        Actor.Components.Add(collider);
        Actor.Components.Add(renderer);
    }

    public override void Removed() {
        Actor.Components.Remove(collider);
        Actor.Components.Remove(renderer);
    }

    public void SetTile(Point2 tilesetCoord, Point2 gridCoord) {
        LDTKTileElement? tileElement = Tileset?.Get(tilesetCoord);
        if (tileElement == null) { return; }
        LDTKTile tile = new(tileElement);
        Tiles.Set(tile, gridCoord);
        UpdateGrids(tile, gridCoord);
    }
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

    public void ClearTile(Point2 gridCoord) {
        Tiles.Set(null, gridCoord);
        UpdateGrids(null, gridCoord);
    }

    public LDTKTile? GetTile(Point2 gridCoord) {
        return Tiles.Get(gridCoord);
    }

    void UpdateGrids(LDTKTile? tile, Point2 gridCoord) {
        colliderGrid.SetTile(tile?.Collider, gridCoord);
        renderer.SetTile(tile?.Sprite, gridCoord);
    }
}