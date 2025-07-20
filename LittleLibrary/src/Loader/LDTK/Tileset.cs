using System.Drawing;
using System.Numerics;
using System.Xml.Linq;
using Foster.Framework;

namespace LittleLib.Loader.LDTK;

//! FIXME (Alex): Include data and extract collision info
public class LDTKTileset {
    public readonly string Identifier;
    public readonly Subtexture Atlas;
    readonly Grid<LDTKTileElement> tileElements;
    public readonly int TileSize;

    readonly Collider DefaultCollider;

    public Point2 Size => tileElements.Size;
    public LDTKTileElement? Get(int id) => tileElements.Get(GetTileCoord(id));
    public LDTKTileElement? Get(Point2 tileCoord) => tileElements.Get(tileCoord);
    public LDTKTileElement? GetLocal(Vector2 localPosition) => tileElements.Get(GetTileCoord(localPosition));
    public Point2 GetTileCoord(Vector2 localPosition) => (Point2)(localPosition / TileSize);
    public Point2 GetTileCoord(int id) => new(id % tileElements.Size.X, id / tileElements.Size.X);

    public LDTKTileset(
        string identifier,
        Subtexture atlas,
        Point2 tileCount,
        int tileSize,
        TileTypeSaveTag[] enumTags,
        TileTypeCustomData[] customData,
        Func<string, int?> collisionTagFunc
    ) {
        Identifier = identifier;
        Atlas = atlas;
        TileSize = tileSize;
        tileElements = new(tileCount);

        DefaultCollider = new RectangleCollider(new Rect(0, 0, TileSize, TileSize));
        Console.WriteLine(DefaultCollider.Tags);

        for (int x = 0; x < tileElements.Size.X; ++x) {
            for (int y = 0; y < tileElements.Size.Y; ++y) {
                Point2 gridCoord = new(x, y);
                //! FIXME (Alex): Make into helper function
                int id = (y * tileCount.X) + x;
                tileElements.Set(new() {
                    ID = id,
                    Sprite = new SpriteSingle(Atlas.Clip(new(gridCoord * TileSize, TileSize, TileSize))),
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
                    Console.WriteLine($"LDTK Tileset: Failed to load custom data {line} for tile {entry.ID}, missing identifier");
                    continue;
                }
                string dataID = line[..index];
                string dataValue = line[(index + 1)..].Trim();
                element.CustomData.Add(dataID, dataValue);

                // Handle built-in tile information
                switch (dataID) {
                    case "collider":
                        element.Collider = ColliderFromData(dataValue);
                        if (entry.ID == 33) { Console.WriteLine($"{element.Collider.Tags}"); }
                        if (element.Collider != null) {
                            foreach (string tag in element.EnumValues) {
                                int? collisionTag = collisionTagFunc(tag);
                                if (collisionTag != null) {
                                    element.Collider.Tags.Add((int)collisionTag);
                                    if (entry.ID == 33) { Console.WriteLine($"add {tag} {collisionTag} {element.Collider.Tags}"); }
                                }
                            }
                        }
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
            case "POLYGON": {
                    if (args.Length % 2 != 0) { return null; }
                    Vector2[] points = new Vector2[args.Length / 2];
                    for (int i = 0; i < points.Length; ++i) {
                        if (
                            !float.TryParse(args[(i * 2) + 1], out float x) ||
                            !float.TryParse(args[(i * 2) + 2], out float y)
                        ) { return null; }
                        points[i] = new(x, y);
                    }
                    //! FIXME (Alex): I'm trusting that this polygon is convex, maybe a bad idea
                    return new ConvexPolygonCollider(new ConvexPolygon() { Vertices = [.. points] });
                }
        }
        return null;
    }
}