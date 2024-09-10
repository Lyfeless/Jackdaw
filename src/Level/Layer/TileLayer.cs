using System.Numerics;
using Foster.Framework;

namespace LittleLib;

/// <summary>
/// A grid layer used for storing tile positions
/// </summary>
public class TileLayer : Layer {
    #region Member variables

    readonly Tile[] Tiles;

    public readonly TilesetSaveDefinition Tileset;

    #endregion

    #region Setup

    public TileLayer(LayerSaveData data, Level level) : base(data, level) {
        //! FIXME (Alex): This error fallback is going to crash regardless
        Tileset = LevelManager.GetTileset(data.Tileset ?? 0);

        Tiles = new Tile[data.Width * data.Height];
        if (data.Type == "Tiles") {
            LoadTiles(data.Tiles);
        }
        else {
            LoadTiles(data.AutoTiles);
        }

    }

    /// <summary>
    /// Sort provided tiles into a grid map and set up to be handled easier by other game systems
    /// </summary>
    /// <param name="tiles">A list of tile data loaded directly from save file</param>
    void LoadTiles(TileSaveData[] tiles) {
        List<TileSaveData>[] tileSort = new List<TileSaveData>[GridSize.X * GridSize.Y];
        foreach (TileSaveData tileData in tiles) {
            int index = GetIndex(tileData.Position[0] / TileSize, tileData.Position[1] / TileSize);
            tileSort[index] ??= [];
            tileSort[index].Add(tileData);
        }

        for (int i = 0; i < tileSort.Length; ++i) {
            if (tileSort[i] == null) { continue; }
            Tiles[i] = new(
                tileSort[i].Select(e => new TileSource(
                    e.TileID,
                    // Texture source rect
                    new(
                        e.Source[0],
                        e.Source[1]
                    ),
                    // X and y flip, stored as 2 bit number
                    //      0b01 = X flip
                    //      0b10 = Y flip
                    new(
                        (e.Flip & 0b01) == 1 ? -1 : 1,
                        ((e.Flip >> 1) & 0b01) == 1 ? -1 : 1
                    )
                )).ToArray(),
                Tileset
            );
        }
    }

    #endregion

    public Tile? GetTile(Point2 tileCoords) {
        return PositionValid(tileCoords) ? Tiles[GetIndex(tileCoords)] : null;
    }

    public void SetTile(Tile tile, Point2 tileCoords) {
        if (!PositionValid(tileCoords)) { return; }
        Tiles[GetIndex(tileCoords)] = tile;
    }

    public TileLayerCollisionInfo CheckCollision(Rect hitbox, string[]? collisionTags) {
        Vector2 adjustedPosition = WorldCoordsToLayerCoords(hitbox.Position);
        Rect adjustedHitbox = new(
            adjustedPosition,
            adjustedPosition + hitbox.Size
        );

        Point2 checkStart = new((int)Math.Floor(adjustedHitbox.Left / TileSize), (int)Math.Floor(adjustedHitbox.Top / TileSize));
        Point2 checkEnd = new((int)Math.Floor((adjustedHitbox.Right - 0.01f) / TileSize), (int)Math.Floor((adjustedHitbox.Bottom - 0.01f) / TileSize));
        Point2 checkSize = checkEnd - checkStart + Point2.One;

        List<Point2> tiles = [];
        for (int x = 0; x < checkSize.X; ++x) {
            int checkX = x + checkStart.X;
            if (checkX < 0 || checkX >= Size.X) { continue; }

            for (int y = 0; y < checkSize.Y; ++y) {
                int checkY = y + checkStart.Y;
                if (checkY < 0 || checkY >= Size.Y) { continue; }

                Tile tile = Tiles[GetIndex(checkX, checkY)];
                if (tile != null && (collisionTags == null ? tile.IsSolid : tile.HasAnyTag(collisionTags))) {
                    tiles.Add(new(checkX, checkY));
                }
            }
        }

        return new(this, [.. tiles]);
    }

    public TileLayerCollisionInfo CheckCollision(Vector2 position, string[]? collisionTags) {
        Point2 tilePosition = (Point2)WorldCoordsToTileCoords(position);
        if (!PositionValid(tilePosition)) { return new(this, []); }
        Tile tile = Tiles[GetIndex(tilePosition)];
        return (tile != null && (collisionTags == null ? tile.IsSolid : tile.HasAnyTag(collisionTags))) ? new(this, [tilePosition]) : new(this, []);
    }

    #region Rendering

    /// <summary>
    /// Render all elements on current layer
    /// </summary>
    /// <param name="batcher">Current rendering batcher</param>
    public override void Render(Batcher batcher) {
        // Only draw the tiles that are currently in the camera's view
        Rect cameraBounds = Camera.ActiveCamera.Bounds;
        Rect checkBounds = new(
            cameraBounds.TopLeft - Level.Position - Offset,
            cameraBounds.BottomRight - Level.Position - Offset
        );

        for (int x = Math.Max(0, (int)(checkBounds.Left / TileSize)); x < Math.Min(Size.X, (int)(checkBounds.Right / TileSize) + 1); ++x) {
            for (int y = Math.Max(0, (int)(checkBounds.Top / TileSize)); y < Math.Min(Size.Y, (int)(checkBounds.Bottom / TileSize) + 1); ++y) {
                batcher.PushMatrix((new Point2(x, y) * TileSize) + Offset);
                Tiles[GetIndex(x, y)]?.Render(batcher, this);
                batcher.PopMatrix();
            }
        }
    }

    #endregion

    #region Private utils

    //! FIXME (Alex): I expect this operation to be very common, might be wise to move this to a global util function if it shows up enough

    int GetIndex(Point2 position) => GetIndex(position.X, position.Y);

    /// <summary>
    /// Get index into tile list given an x and a y
    /// </summary>
    /// <param name="x">Tile x position in grid space</param>
    /// <param name="y">Tile y position in grid space</param>
    /// <returns>List index representing tile</returns>
    int GetIndex(int x, int y) {
        return (y * Size.X) + x;
    }

    /// <summary>
    /// Get x and y position of a tile in pixels given a list index
    /// </summary>
    /// <param name="index">List index of tile</param>
    /// <returns>Point2 representing x and y of tile in grid space</returns>
    Point2 GetPosition(int index) {
        return new(
            index / Size.X,
            index % Size.X
        );
    }

    public bool PositionValid(Point2 position) => PositionValid(position.X, position.Y);
    bool PositionValid(int x, int y) {
        return x >= 0 && x < Size.X && y >= 0 && y < Size.Y;
    }

    #endregion
}