using System.Text.Json.Serialization;

namespace LittleLib.Loader.LDTK;

public class LevelSaveData {
    [JsonPropertyName("layerInstances")]
    public LayerSaveData[] Layers { get; set; } = [];
}