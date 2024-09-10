using System.Text.Json;
using Foster.Framework;

namespace LittleLib;

/// <summary>
/// Responsible for storing and handling all values related to level data such as level id and tilemap info.
/// </summary>
public static class LevelManager {
    static readonly Dictionary<int, TilesetSaveDefinition> tilesets = [];
    public static TilesetSaveDefinition GetTileset(int NameID) => tilesets.TryGetValue(NameID, out TilesetSaveDefinition output) ? output : throw new Exception("Attempt to access undefined tileset");

    static readonly Dictionary<string, LevelSaveReference> levels = [];
    public static LevelSaveReference GetLevelReference(string NameID) => levels.TryGetValue(NameID, out LevelSaveReference output) ? output : throw new Exception("Attempt to access undefined level identifier");
    public static string[] LevelNames => [.. levels.Keys];

    public static Level ActiveLevel { get; private set; } = Level.Empty;

    /// <summary>
    /// Load preliminary information from global save data. Not responsible for loading any specific level, which should be done later on.
    /// </summary>
    public static void Init() {
        if (!File.Exists("./Content/Levels.ldtk")) { return; }
        WorldSaveData? data = JsonSerializer.Deserialize(File.ReadAllText("./Content/Levels.ldtk"), SourceGenerationContext.Default.WorldSaveData);
        if (data == null) { return; }

        foreach (TilesetSaveDefinition tileset in data.Definitions.Tilesets) {
            tilesets.Add(tileset.NameID, tileset);
        }

        foreach (LevelSaveReference levelRef in data.Levels) {
            levels.Add(levelRef.NameID, levelRef);
        }
    }

    public static void SetActiveLevel(Level level) {
        ActiveLevel.Deactivate();
        level.Activate();
        ActiveLevel = level;
    }

    public static void SetActiveLevel(string levelName) {
        ActiveLevel.Deactivate();
        ActiveLevel = LoadLevel(levelName, false);
        ActiveLevel.Activate();

        LittleLibMain.LevelChange();
    }

    /// <summary>
    /// Load a level from file.
    /// </summary>
    /// <param name="levelName">Name identifier for the current level.</param>
    /// <param name="skipRegister">Avoid adding any entities to the EntityManager if level is being preloaded.</param>
    /// <returns>The current loaded level. Returns an empty level if requested level is invalid.</returns>
    public static Level LoadLevel(string levelName, bool skipRegister) {
        if (!levels.TryGetValue(levelName, out LevelSaveReference? levelRef)) { throw new Exception("Attempt to access unidentified level"); }
        LevelSaveData levelData = JsonSerializer.Deserialize(File.ReadAllText($"./Content/Levels/{levelRef.NameID}.ldtkl"), SourceGenerationContext.Default.LevelSaveData) ?? throw new Exception("Failed to load level");

        Point2 levelPosition = new(levelRef.X, levelRef.Y);
        Point2 levelSize = new(levelRef.Width, levelRef.Height);

        Level level = new(levelName, levelSize, levelPosition, levelRef.Fields);

        List<Layer> layers = [];
        foreach (LayerSaveData layerData in levelData.Layers) {
            switch (layerData.Type) {
                case "Entities":
                    layers.Add(new EntityLayer(layerData, level, skipRegister));
                    break;
                case "IntGrid":
                case "Tiles":
                case "AutoLayer":
                    layers.Add(new TileLayer(layerData, level));
                    break;
            }
        }

        level.SetLayers([.. layers]);

        return level;
    }
}