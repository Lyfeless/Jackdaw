using System.Text.Json.Serialization;

namespace LittleLib.Loader.LDTK;

class WorldSaveData {
    [JsonPropertyName("levels")]
    public LevelSaveReference[] Levels { get; set; } = [];

    [JsonPropertyName("defs")]
    public WorldSaveDefinitions Definitions { get; set; } = new();
}