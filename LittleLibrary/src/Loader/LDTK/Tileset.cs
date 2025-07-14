using System.Numerics;
using Foster.Framework;

namespace LittleLib.Loader.LDTK;

//! FIXME (Alex): Include data and extract collision info
public class LDTKTileset {
    public readonly string Identifier;
    public readonly Subtexture Atlas;
    readonly Grid<LDTKTileElement> tileElements;
    public readonly int TileSize;

    public Point2 Size => tileElements.Size;
    public LDTKTileElement? Get(Point2 tileCoord) => tileElements.Get(tileCoord);
    public LDTKTileElement? GetLocal(Vector2 localPosition) => tileElements.Get(GetLocalPosition(localPosition));
    public Point2 GetLocalPosition(Vector2 localPosition) => (Point2)(localPosition / TileSize);

    public LDTKTileset(string identifier, Subtexture atlas, Point2 tileCount, int tileSize) {
        Identifier = identifier;
        Atlas = atlas;
        TileSize = tileSize;
        tileElements = new(tileCount);
        for (int x = 0; x < tileElements.Size.X; ++x) {
            for (int y = 0; y < tileElements.Size.Y; ++y) {
                Point2 gridCoord = new(x, y);
                tileElements.Set(new() {
                    Sprite = new SpriteSingle(Atlas.Clip(new(gridCoord * TileSize, TileSize, TileSize)))
                }, gridCoord);
            }
        }
    }
}