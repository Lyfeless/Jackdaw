using System.Numerics;
using Foster.Framework;

namespace LittleLib;

/// <summary>
/// Inheritable class for storing level layers.
/// </summary>
/// <param name="data">Level save data.</param>
public abstract class Layer(LayerSaveData data, Level level) {
    public readonly Level Level = level;
    public readonly string NameID = data.NameID;
    public readonly string InstanceID = data.InstanceID;
    public readonly Point2 Size = new(data.Width, data.Height);
    public readonly Point2 GridSize = new(data.Width, data.Height);
    public readonly int TileSize = data.TileSize;
    public readonly Point2 Offset = new(data.OffsetX, data.OffsetY);

    /// <summary>
    /// Render the layer and all objects/tiles present.
    /// </summary>
    /// <param name="position">The position to render at.</param>
    /// <returns>Output target of rendered layer</returns>
    public abstract void Render(Batcher batcher);

    public Vector2 WorldCoordsToLayerCoords(Vector2 worldCoords) {
        return worldCoords - Level.Position - Offset;
    }

    public Vector2 WorldCoordsToTileCoords(Vector2 worldCoords) {
        return LayerCoordsToTileCoords(WorldCoordsToLayerCoords(worldCoords));
    }


    public Vector2 TileCoordsToLayerCoords(Vector2 tileCoords) {
        return tileCoords * TileSize;
    }

    public Vector2 TileCoordsToWorldCoords(Vector2 tileCoords) {
        return LayerCoordsToWorldCoords(TileCoordsToLayerCoords(tileCoords));
    }

    public Vector2 LayerCoordsToTileCoords(Vector2 levelCoords) {
        return levelCoords / TileSize;
    }
    public Vector2 LayerCoordsToWorldCoords(Vector2 levelCoords) {
        return levelCoords + Level.Position + Offset;
    }
}