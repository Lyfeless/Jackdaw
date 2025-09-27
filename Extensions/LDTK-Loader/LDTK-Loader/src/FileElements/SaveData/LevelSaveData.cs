using System.Text.Json.Serialization;

namespace Jackdaw.Loader.LDTK;

public class LevelSaveData {
    [JsonPropertyName("layerInstances")]
    public LayerSaveData[] Layers { get; set; } = [];
}