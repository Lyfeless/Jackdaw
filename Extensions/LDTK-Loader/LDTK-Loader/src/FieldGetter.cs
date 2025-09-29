using System.Text.Json;
using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

/// <summary>
/// Utility class for getting custom values out of level savedata.
/// </summary>
public static class LDTKFieldGetter {
    #region Reference Types
    /// <summary>
    /// Displayable tile reference.
    /// </summary>
    public struct TileReference {
        /// <summary>
        /// The tileset the tile is using.
        /// </summary>
        public int Tileset;

        /// <summary>
        /// The tile's x position in the tileset texture.
        /// </summary>
        public int X;

        /// <summary>
        /// The tile's y position in the tileset texture.
        /// </summary>
        public int Y;

        /// <summary>
        /// The tile's width in the tileset texture.
        /// </summary>
        public int Width;

        /// <summary>
        /// The tile's height in the tileset texture.
        /// </summary>
        public int Height;
    }

    /// <summary>
    /// Reference to an entity and containers, loaded or not.
    /// </summary>
    public struct EntityReference {
        /// <summary>
        /// The level name the entity is part of.
        /// </summary>
        public string Level;

        /// <summary>
        /// The layer name the entity is part of.
        /// </summary>
        public string Layer;

        /// <summary>
        /// The entity's uuid.
        /// </summary>
        public string Entity;
    }

    #endregion

    #region Value Getters

    /// <summary>
    /// Gets a single integer value from field data by identifier.
    /// </summary>
    /// <param name="id">Field Identifier.</param>
    /// <param name="fields">Field data from save data..</param>
    /// <returns>Int value of field or default.</returns>
    public static int GetInt(string id, FieldSaveData[] fields) {
        JsonElement element = GetFieldElement(id, fields);
        if (element.ValueKind == JsonValueKind.Undefined) { return 0; }
        return element.TryGetInt32(out int value) ? value : 0;
    }

    /// <summary>
    /// Gets a single float value from field data by identifier.
    /// </summary>
    /// <param name="id">Field Identifier.</param>
    /// <param name="fields">Field data from save data.</param>
    /// <returns>Float value of field or default.</returns>
    public static float GetFloat(string id, FieldSaveData[] fields) {
        JsonElement element = GetFieldElement(id, fields);
        if (element.ValueKind == JsonValueKind.Undefined) { return 0; }
        return element.TryGetDouble(out double value) ? (float)value : 0;
    }

    /// <summary>
    /// Gets a single boolean value from field data by identifier.
    /// </summary>
    /// <param name="id">Field Identifier.</param>
    /// <param name="fields">Field data from save data.</param>
    /// <returns>Boolean value of field or default</returns>
    public static bool GetBool(string id, FieldSaveData[] fields) {
        JsonElement element = GetFieldElement(id, fields);
        if (element.ValueKind == JsonValueKind.Undefined) { return false; }
        return element.GetBoolean();
    }

    /// <summary>
    /// Gets a single string value from field data by identifier.
    /// </summary>
    /// <param name="id">Field Identifier.</param>
    /// <param name="fields">Field data from save data.</param>
    /// <returns>String value of field or default.</returns>
    public static string GetString(string id, FieldSaveData[] fields) {
        JsonElement element = GetFieldElement(id, fields);
        if (element.ValueKind == JsonValueKind.Undefined) { return string.Empty; }
        return element.GetString() ?? string.Empty;
    }

    /// <summary>
    /// Gets a single enum value from field data by identifier and type.
    /// </summary>
    /// <typeparam name="T">Target enum for field.</typeparam>
    /// <param name="id">Field Identifier.</param>
    /// <param name="fields">Field data from save data.</param>
    /// <returns>Enum value of field or default.</returns>
    public static T GetEnum<T>(string id, FieldSaveData[] fields) where T : struct {
        return Enum.TryParse(GetString(id, fields), out T output) ? output : default;
    }

    /// <summary>
    /// Gets a single Color value from field data by identifier.
    /// </summary>
    /// <param name="id">Field Identifier.</param>
    /// <param name="fields">Field data from save data.</param>
    /// <returns>Color value of field or default.</returns>
    public static Color GetColor(string id, FieldSaveData[] fields) {
        return Color.FromHexStringRGB(GetString(id, fields));
    }

    /// <summary>
    /// Gets a single Point2 value from field data by identifier.
    /// </summary>
    /// <param name="id">Field Identifier.</param>
    /// <param name="fields">Field data from save data.</param>
    /// <returns>Point2 value of field in grid space or null.</returns>
    public static Point2 GetPoint(string id, FieldSaveData[] fields) {
        return GetPoint2(GetFieldElement(id, fields));
    }

    /// <summary>
    /// Gets a single TileReference value from field data by identifier.
    /// </summary>
    /// <param name="id">Field Identifier.</param>
    /// <param name="fields">Field data from save data.</param>
    /// <returns>TileReference value of field or null.</returns>
    public static TileReference GetTile(string id, FieldSaveData[] fields) {
        return GetTileReference(GetFieldElement(id, fields));
    }

    /// <summary>
    /// Gets a single EntityReference value from field data by identifier.
    /// </summary>
    /// <param name="id">Field Identifier.</param>
    /// <param name="fields">Field data from save data.</param>
    /// <returns>EntityReference value of field or null.</returns>
    public static EntityReference GetEntity(string id, FieldSaveData[] fields) {
        return GetEntityReference(GetFieldElement(id, fields));
    }

    #endregion

    #region List Getters

    /// <summary>
    /// Gets a list of integer values from field data by identifier.
    /// </summary>
    /// <param name="id">Field Identifier.</param>
    /// <param name="fields">Field data from save data.</param>
    /// <returns>Integer value list, empty if no entries found.</returns>
    public static int[] GetIntList(string id, FieldSaveData[] fields) {
        JsonElement obj = GetFieldElement(id, fields);
        if (obj.ValueKind == JsonValueKind.Undefined) { return []; }
        return [.. obj.EnumerateArray().Select(e => e.TryGetInt32(out int value) ? value : 0)];
    }

    /// <summary>
    /// Gets a list of float values from field data by identifier.
    /// </summary>
    /// <param name="id">Field Identifier.</param>
    /// <param name="fields">Field data from save data.</param>
    /// <returns>Float value list, empty if no entries found.</returns>
    public static float[] GetFloatList(string id, FieldSaveData[] fields) {
        JsonElement obj = GetFieldElement(id, fields);
        if (obj.ValueKind == JsonValueKind.Undefined) { return []; }
        return [.. obj.EnumerateArray().Select(e => e.TryGetDouble(out double value) ? (float)value : 0)];
    }

    /// <summary>
    /// Gets a list of boolean values from field data by identifier.
    /// </summary>
    /// <param name="id">Field Identifier.</param>
    /// <param name="fields">Field data from save data.</param>
    /// <returns>Boolean value list, empty if no entries found.</returns>
    public static bool[] GetBoolList(string id, FieldSaveData[] fields) {
        JsonElement obj = GetFieldElement(id, fields);
        if (obj.ValueKind == JsonValueKind.Undefined) { return []; }
        return [.. obj.EnumerateArray().Select(e => e.GetBoolean())];
    }

    /// <summary>
    /// Gets a list of string values from field data by identifier.
    /// </summary>
    /// <param name="id">Field Identifier.</param>
    /// <param name="fields">Field data from save data.</param>
    /// <returns>String value list, empty if no entries found.</returns>
    public static string[] GetStringList(string id, FieldSaveData[] fields) {
        JsonElement obj = GetFieldElement(id, fields);
        if (obj.ValueKind == JsonValueKind.Undefined) { return []; }
        return [.. obj.EnumerateArray().Select(e => e.GetString())];
    }

    /// <summary>
    /// Gets a single enum value from field data by identifier and type.
    /// </summary>
    /// <typeparam name="T">Target enum for field.</typeparam>
    /// <param name="id">Field Identifier.</param>
    /// <param name="fields">Field data from save data.</param>
    /// <returns>Enum value list, empty if no entries found.</returns>
    public static T[] GetEnumList<T>(string id, FieldSaveData[] fields) where T : struct {
        JsonElement obj = GetFieldElement(id, fields);
        if (obj.ValueKind == JsonValueKind.Undefined) { return []; }
        return [.. obj.EnumerateArray().Select(e => Enum.TryParse(e.GetString(), out T output) ? output : default)];
    }

    /// <summary>
    /// Gets a list of Color values from field data by identifier.
    /// </summary>
    /// <param name="id">Field Identifier.</param>
    /// <param name="fields">Field data from save data.</param>
    /// <returns>Color value list, empty if no entries found.</returns>
    public static Color[] GetColorList(string id, FieldSaveData[] fields) {
        JsonElement obj = GetFieldElement(id, fields);
        if (obj.ValueKind == JsonValueKind.Undefined) { return []; }
        return [.. obj.EnumerateArray().Select(e => Color.FromHexStringRGB(e.GetString()))];
    }

    /// <summary>
    /// Gets a list of Point2 values from field data by identifier.
    /// </summary>
    /// <param name="id">Field Identifier.</param>
    /// <param name="fields">Field data from save data.</param>
    /// <returns>Point2 value list, empty if no entries found.</returns>
    public static Point2[] GetPointList(string id, FieldSaveData[] fields) {
        JsonElement obj = GetFieldElement(id, fields);
        if (obj.ValueKind == JsonValueKind.Undefined) { return []; }
        return [.. obj.EnumerateArray().Select(GetPoint2)];
    }

    /// <summary>
    /// Gets a list of TileReference values from field data by identifier.
    /// </summary>
    /// <param name="id">Field Identifier.</param>
    /// <param name="fields">Field data from save data.</param>
    /// <returns>TileReference value list, empty if no entries found.</returns>
    public static TileReference[] GetTileList(string id, FieldSaveData[] fields) {
        JsonElement obj = GetFieldElement(id, fields);
        if (obj.ValueKind == JsonValueKind.Undefined) { return []; }
        return [.. obj.EnumerateArray().Select(GetTileReference)];
    }

    /// <summary>
    /// Gets a list of EntityReference values from field data by identifier.
    /// </summary>
    /// <param name="id">Field Identifier.</param>
    /// <param name="fields">Field data from save data.</param>
    /// <returns>EntityReference value list, empty if no entries found.</returns>
    public static EntityReference[] GetEntityList(string id, FieldSaveData[] fields) {
        JsonElement obj = GetFieldElement(id, fields);
        if (obj.ValueKind == JsonValueKind.Undefined) { return []; }
        return [.. obj.EnumerateArray().Select(GetEntityReference)];
    }

    #endregion

    #region Internal Util

    static Point2 GetPoint2(JsonElement obj) {
        return new Point2(obj.GetProperty("cx").GetInt32(), obj.GetProperty("cy").GetInt32());
    }

    static TileReference GetTileReference(JsonElement obj) {
        return new TileReference() {
            Tileset = obj.GetProperty("tilesetUid").GetInt32(),
            X = obj.GetProperty("x").GetInt32(),
            Y = obj.GetProperty("y").GetInt32(),
            Width = obj.GetProperty("w").GetInt32(),
            Height = obj.GetProperty("h").GetInt32()
        };
    }

    static EntityReference GetEntityReference(JsonElement obj) {
        return new EntityReference() {
            Level = obj.GetProperty("levelIid").GetString() ?? string.Empty,
            Layer = obj.GetProperty("layerIid").GetString() ?? string.Empty,
            Entity = obj.GetProperty("entityIid").GetString() ?? string.Empty
        };
    }

    static JsonElement GetFieldElement(string id, FieldSaveData[] fields) {
        foreach (FieldSaveData field in fields) {
            if (field.NameID == id && field.Value != null) { return (JsonElement)field.Value; }
        }

        Log.Warning($"LDTKField: Failed to find id {id}");
        return default;
    }

    #endregion
}