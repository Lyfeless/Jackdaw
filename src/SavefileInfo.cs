using Foster.Framework;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LittleLib;

class WorldSaveData {
    [JsonPropertyName("levels")]
    public LevelSaveReference[] Levels { get; set; } = [];

    [JsonPropertyName("defs")]
    public WorldSaveDefinitions Definitions { get; set; }
}

class WorldSaveDefinitions {
    [JsonPropertyName("entities")]
    public EntitySaveDefinition[] Entities { get; set; } = [];

    [JsonPropertyName("enums")]
    public EnumSaveDefinition[] Enums { get; set; } = [];

    [JsonPropertyName("layers")]
    public LayerSaveDefinition[] Layers { get; set; } = [];

    [JsonPropertyName("levelFields")]
    public FieldSaveDefinition[] Fields { get; set; } = [];

    [JsonPropertyName("tilesets")]
    public TilesetSaveDefinition[] Tilesets { get; set; } = [];
}

class EntitySaveDefinition { }

class EnumSaveDefinition { }

class LayerSaveDefinition { }

class FieldSaveDefinition { }

public class TilesetSaveDefinition {
    [JsonPropertyName("uid")]
    public int NameID { get; set; }

    [JsonPropertyName("relPath")]
    public string TexturePath { get; set; } = string.Empty;

    public string TextureID {
        get {
            string newPath = Path.Join(
                //! FIXME (Alex): This is a bit jank, look into a better way to handle this
                Path.GetDirectoryName(TexturePath.Remove(0, Assets.TextureFolder.Length + 1)),
                Path.GetFileNameWithoutExtension(TexturePath)
            ).Replace("\\", "/");

            return newPath;
        }
    }

    [JsonPropertyName("__cWid")]
    public int TileCountX { get; set; }

    [JsonPropertyName("__cHei")]
    public int TileCountY { get; set; }

    [JsonPropertyName("tileGridSize")]
    public int GridSize { get; set; }

    [JsonPropertyName("padding")]
    public int Padding { get; set; }

    [JsonPropertyName("enumTags")]
    public TileTypeSaveTag[] TileTypes { get; set; } = [];

    public bool HasAnyTags(int tileID, string[] tags) {
        foreach (TileTypeSaveTag tag in TileTypes) {
            if (tags.Contains(tag.Value) && tag.tileIDs.Contains(tileID)) {
                return true;
            }
        }

        return false;
    }
}

public class TileTypeSaveTag {
    [JsonPropertyName("enumValueId")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("tileIds")]
    public int[] tileIDs { get; set; } = [];
}

public class LevelSaveReference {
    [JsonPropertyName("identifier")]
    public string NameID { get; set; } = string.Empty;

    [JsonPropertyName("iid")]
    public string InstanceID { get; set; } = string.Empty;

    [JsonPropertyName("worldX")]
    public int X { get; set; }

    [JsonPropertyName("worldY")]
    public int Y { get; set; }

    [JsonPropertyName("pxWid")]
    public int Width { get; set; }

    [JsonPropertyName("pxHei")]
    public int Height { get; set; }

    [JsonPropertyName("__bgColor")]
    public string BackgroundColor { get; set; } = string.Empty;

    [JsonPropertyName("fieldInstances")]
    public FieldSaveData[] Fields { get; set; } = [];
}

public class LevelSaveData {
    [JsonPropertyName("layerInstances")]
    public LayerSaveData[] Layers { get; set; } = [];
}

public class LayerSaveData {
    [JsonPropertyName("__type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("__identifier")]
    public string NameID { get; set; } = string.Empty;

    [JsonPropertyName("iid")]
    public string InstanceID { get; set; } = string.Empty;

    [JsonPropertyName("__cWid")]
    public int Width { get; set; }

    [JsonPropertyName("__cHei")]
    public int Height { get; set; }

    [JsonPropertyName("__gridSize")]
    public int TileSize { get; set; }

    [JsonPropertyName("__tilesetDefUid")]
    public int? Tileset { get; set; }

    [JsonPropertyName("__pxTotalOffsetX")]
    public int OffsetX { get; set; }

    [JsonPropertyName("__pxTotalOffsetY")]
    public int OffsetY { get; set; }

    [JsonPropertyName("autoLayerTiles")]
    public TileSaveData[] AutoTiles { get; set; } = [];

    [JsonPropertyName("gridTiles")]
    public TileSaveData[] Tiles { get; set; } = [];

    [JsonPropertyName("entityInstances")]
    public EntitySaveData[] Entities { get; set; } = [];
}

public class EntitySaveData {
    [JsonPropertyName("__identifier")]
    public string NameID { get; set; } = string.Empty;

    [JsonPropertyName("iid")]
    public string InstanceID { get; set; } = string.Empty;

    [JsonPropertyName("px")]
    public int[] Position { get; set; } = [0, 0];

    [JsonPropertyName("__worldX")]
    public int? WorldX { get; set; }

    [JsonPropertyName("__worldY")]
    public int? WorldY { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("fieldInstances")]
    public FieldSaveData[] Fields { get; set; } = [];
}

public class TileSaveData {
    [JsonPropertyName("t")]
    public int TileID { get; set; }

    [JsonPropertyName("a")]
    public float Alpha { get; set; }

    [JsonPropertyName("f")]
    public int Flip { get; set; }

    [JsonPropertyName("px")]
    public int[] Position { get; set; } = [0, 0];

    [JsonPropertyName("src")]
    public int[] Source { get; set; } = [0, 0];
}

public class FieldSaveData {
    [JsonPropertyName("__identifier")]
    public string NameID { get; set; } = string.Empty;

    [JsonPropertyName("__type")]
    public string type { get; set; } = string.Empty;

    [JsonPropertyName("__value")]
    public object Value { get; set; } = 0;

    #region Reference Types

    //! FIXME (Alex): I don't like how similar this is to other tile types, possibly try combining this with other tile management systems
    /// <summary>
    /// Displayable tile reference, stores the position and size of a texture source on a tileset.
    /// </summary>
    public struct TileReference {
        public int Tileset;
        public int X;
        public int Y;
        public int Width;
        public int Height;
    }

    /// <summary>
    /// Reference to an entity and containers, loaded or not.
    /// </summary>
    public struct EntityReference {
        public string Level;
        public string Layer;
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

        Console.WriteLine($"Failed to find id {id}");
        return default;
    }

    #endregion
}