using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Utilities for converting between tile and local positions in a grid.
/// </summary>
public static class SpatialGridConverter {
    extension(ISpatialGrid grid) {
        /// <summary>
        /// Convert a position from a spatial position to a tile coordinate.
        /// </summary>
        /// <param name="localPosition">The local coord to be converted.</param>
        /// <returns>The local coordinate in tile space.</returns>
        public Matrix3x2 LocalToTileCoord(Matrix3x2 localPosition) {
            return localPosition * grid.InMatrix();
        }

        /// <summary>
        /// Convert a position from a spatial position to a tile coordinate.
        /// </summary>
        /// <param name="localPosition">The local coord to be converted.</param>
        /// <returns>The local coordinate in tile space.</returns>
        public Vector2 LocalToTileCoord(Vector2 localPosition) {
            return Vector2.Transform(localPosition, grid.InMatrix());
        }

        /// <summary>
        /// Convert a position from a tile coordinate to a spatial position.
        /// </summary>
        /// <param name="tileCoords">The tile coord to be converted.</param>
        /// <returns>The tile coordinate in local space.</returns>
        public Matrix3x2 TileCoordToLocal(Matrix3x2 tileCoords) {
            return tileCoords * grid.OutMatrix();
        }

        /// <summary>
        /// Convert a position from a tile coordinate to a spatial position.
        /// </summary>
        /// <param name="tileCoords">The tile coord to be converted.</param>
        /// <returns>The tile coordinate in local space.</returns>
        public Vector2 TileCoordToLocal(Point2 tileCoords)
            => TileCoordToLocal(grid, (Vector2)tileCoords);

        /// <summary>
        /// Convert a position from a tile coordinate to a spatial position.
        /// </summary>
        /// <param name="tileCoords">The tile coord to be converted.</param>
        /// <returns>The tile coordinate in local space.</returns>
        public Vector2 TileCoordToLocal(Vector2 tileCoords) {
            return Vector2.Transform(tileCoords, grid.OutMatrix());
        }

        Matrix3x2 OutMatrix()
            => Transform.CreateMatrix(grid.Position, Vector2.Zero, grid.TileSize, 0);

        Matrix3x2 InMatrix()
            => Transform.CreateMatrix(-grid.Position, Vector2.Zero, Vector2.One / grid.TileSize, 0);
    }
}