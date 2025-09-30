using System.Numerics;
using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

/// <summary>
/// Data of a single grid tileset.
/// </summary>
public class LDTKTileset {
    readonly Game Game;

    /// <summary>
    /// The tileset unique identifier.
    /// </summary>
    public readonly string Identifier;

    /// <summary>
    /// The tileset texture.
    /// </summary>
    public readonly Subtexture Atlas;

    /// <summary>
    /// The tile element data in the tileset.
    /// </summary>
    readonly Grid<LDTKTileElement> tileElements;

    /// <summary>
    /// The width and height of each tile.
    /// </summary>
    public readonly int TileSize;

    readonly Collider DefaultCollider;

    /// <summary>
    /// The gird size of the tileset.
    /// </summary>
    public Point2 Size => tileElements.Size;

    /// <summary>
    /// Get a tile element from the tileset.
    /// </summary>
    /// <param name="id">The tile element's unique identifier.</param>
    /// <returns>The tile element, null if no tile element matches the id.</returns>
    public LDTKTileElement? Get(int id) => tileElements.Get(GetTileCoord(id));

    /// <summary>
    /// Get a tile element from the tileset in grid coordinates.
    /// </summary>
    /// <param name="tileCoord">The grid coordinate of the tile element.</param>
    /// <returns>The tile element, null if the coords are out of range.</returns>
    public LDTKTileElement? Get(Point2 tileCoord) => tileElements.Get(tileCoord);

    /// <summary>
    /// Get a tile element from the tileset in pixel coordinates.
    /// </summary>
    /// <param name="localPosition">>The pixel coordinate of the tile element.</param>
    /// <returns>The tile element, null if the coords are out of range.</returns>
    public LDTKTileElement? GetLocal(Vector2 localPosition) => tileElements.Get(GetTileCoord(localPosition));

    /// <summary>
    /// Convert a pixel position into a tile coordinate.
    /// </summary>
    /// <param name="localPosition">The pixel coordinate.</param>
    /// <returns>The position as a tile coordinate.</returns>
    public Point2 GetTileCoord(Vector2 localPosition) => (Point2)(localPosition / TileSize);

    /// <summary>
    /// Convert a tile identifier into a tile coordinate.
    /// </summary>
    /// <param name="id">The tile id.</param>
    /// <returns>The position as a tile coordinate.</returns>
    public Point2 GetTileCoord(int id) => new(id % tileElements.Size.X, id / tileElements.Size.X);

    /// <summary>
    /// Data of a single grid tileset.
    /// </summary>
    /// <param name="game">The game instance.</param>
    /// <param name="identifier">The tileset identifier.</param>
    /// <param name="atlas">The tileset texture.</param>
    /// <param name="tileCount">The number of tiles in the tileset.</param>
    /// <param name="tileSize">The individual tile size.</param>
    /// <param name="enumTags">The per-tile enum tag data.</param>
    /// <param name="customData">The per-tile custom string data.</param>
    /// <param name="collisionTagFunc">The function for converting enum tag data into enums.</param>
    internal LDTKTileset(
        Game game,
        string identifier,
        Subtexture atlas,
        Point2 tileCount,
        int tileSize,
        TileTypeSaveTag[] enumTags,
        TileTypeCustomData[] customData,
        Func<string, int?> collisionTagFunc
    ) {
        Game = game;

        Identifier = identifier;
        Atlas = atlas;
        TileSize = tileSize;
        tileElements = new(tileCount);

        DefaultCollider = new RectangleCollider(new Rect(0, 0, TileSize, TileSize));

        for (int x = 0; x < tileElements.Size.X; ++x) {
            for (int y = 0; y < tileElements.Size.Y; ++y) {
                Point2 gridCoord = new(x, y);
                int id = (y * tileCount.X) + x;
                tileElements.Set(new() {
                    ID = id,
                    Sprite = new SpriteSingle(Atlas.GetClipSubtexture(new(gridCoord * TileSize, TileSize, TileSize))),
                    EnumValues = [.. enumTags.Where(e => e.tileIDs.Contains(id)).Select(e => e.Value)]
                }, gridCoord);
            }
        }

        foreach (TileTypeCustomData entry in customData) {
            LDTKTileElement? element = Get(entry.ID);
            if (element == null) { continue; }
            string[] lines = entry.Data.Split("\n");
            foreach (string line in lines) {
                int index = line.IndexOf(':');
                if (index == -1) {
                    Log.Warning($"LDTK Tileset: Failed to load custom data {line} for tile {entry.ID}, missing identifier");
                    continue;
                }
                string dataID = line[..index];
                string dataValue = line[(index + 1)..].Trim();
                element.CustomData.Add(dataID, dataValue);

                // Handle built-in tile information
                switch (dataID) {
                    case "collider":
                        /*
                            Collider custom data format:
                                identifier: collider
                                formats:
                                    Full tile: FULL
                                        ex: collider: FULL
                                    Rectangle: RECT [x] [y] [width] [height]
                                        ex: collider: RECT 4 4 8 8
                                    Circle: CIRCLE [x] [y] [radius]
                                        ex: collider: CIRCLE 8 8 4
                                    Polygon: POLY [x1] [y1] [x2] [y2] ...
                                        Expects a convex collider but doesn't verify
                                        ex: collider: POLY 0 8 16 8 8 16 0 8
                        */

                        element.Collider = ColliderFromData(dataValue);
                        if (element.Collider != null) {
                            foreach (string tag in element.EnumValues) {
                                int? collisionTag = collisionTagFunc(tag);
                                if (collisionTag != null) {
                                    element.Collider.Tags.Add((int)collisionTag);
                                }
                            }
                        }
                        break;
                    case "anim":
                        /*
                            Animated sprite custom data format:
                                identifier: anim
                                format: [name]
                                ex: anim: testAnim
                        */

                        if (dataValue == string.Empty) { continue; }
                        element.Sprite = new SpriteAnimated(Game, Game.Assets.GetAnimation(dataValue));
                        break;
                }
            }
        }
    }

    Collider? ColliderFromData(string data) {
        if (data == string.Empty) { return null; }
        string[] args = data.Split(' ');
        switch (args[0]) {
            case "FULL": { return new RectangleCollider(DefaultCollider.Bounds); }
            case "RECT": {
                    if (
                        args.Length != 5 ||
                        !float.TryParse(args[1], out float x) ||
                        !float.TryParse(args[2], out float y) ||
                        !float.TryParse(args[3], out float w) ||
                        !float.TryParse(args[4], out float h)
                    ) { return null; }
                    return new RectangleCollider(new Rect(x, y, w, h));
                }
            case "CIRCLE": {
                    if (
                        args.Length != 4 ||
                        !float.TryParse(args[1], out float x) ||
                        !float.TryParse(args[2], out float y) ||
                        !float.TryParse(args[3], out float r)
                    ) { return null; }
                    return new CircleCollider(new Circle(x, y, r));
                }
            case "POLY": {
                    if (args.Length % 2 != 0) { return null; }
                    Vector2[] points = new Vector2[args.Length / 2];
                    for (int i = 0; i < points.Length; ++i) {
                        if (
                            !float.TryParse(args[(i * 2) + 1], out float x) ||
                            !float.TryParse(args[(i * 2) + 2], out float y)
                        ) { return null; }
                        points[i] = new(x, y);
                    }
                    return new ConvexPolygonCollider(new ConvexPolygon() { Vertices = [.. points] });
                }
        }
        return null;
    }
}