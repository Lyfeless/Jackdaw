using System.Numerics;
using Foster.Framework;

namespace LittleLib;

/// <summary>
/// Display layer for a single tile, used for storing texture position and display information
/// </summary>
/// <param name="source">Source position of texture in a tilesheet, in pixels from the top left of texture</param>
/// <param name="flip">Mirroring information, 1 for normal and -1 for flip on each axis</param>
public class TileSource(int tileID, Point2 source, Point2 flip) {
    public int tileID = tileID;
    public Point2 Source = source;
    public Point2 Flip = flip;
}

/// <summary>
/// Displayable tile, handles storing and drawing a stack of texture references
/// </summary>
/// <param name="sourceStack">List of texture sources, sorted in display order from bottom to top</param>
public class Tile {
    public static string[] CollisionTags = [];

    public TilesetSaveDefinition Tileset;

    readonly TileSource[] SourceStack;
    public bool IsSolid => SourceStack.Any(e => Tileset.HasAnyTags(e.tileID, CollisionTags));
    public bool HasAnyTag(string[] tags) => SourceStack.Any(e => Tileset.HasAnyTags(e.tileID, tags));

    public Tile(TileSource[] sourceStack, TilesetSaveDefinition tileset) {
        Tileset = tileset;

        SourceStack = sourceStack;
    }

    public Tile(TileSource[] sourceStack, int tileset) {
        Tileset = LevelManager.GetTileset(tileset);

        SourceStack = sourceStack;
    }

    /// <summary>
    /// Draw tile stack
    /// </summary>
    /// <param name="batcher">Current rendering batcher</param>
    /// <param name="layer">Tile layer this tile is part of</param>
    public void Render(Batcher batcher, TileLayer layer, bool collisionOnly = false) {
        foreach (TileSource tile in SourceStack) {
            // Ignore rendering for non-collidable objects if rendering coll a collision mask
            if (collisionOnly && layer.Tileset.HasAnyTags(tile.tileID, CollisionTags)) { continue; }

            Subtexture texture = Assets.GetTexture(layer.Tileset.TextureID);

            Vector2 origin = new(layer.TileSize / 2);

            batcher.Image(
                new Subtexture(
                    texture.Texture,
                    new(
                        tile.Source.X + texture.Source.X,
                        tile.Source.Y + texture.Source.Y,
                        layer.TileSize,
                        layer.TileSize
                    )
                ),
                origin,
                origin,
                tile.Flip,
                0,
                Color.White
            );
        }
    }
}