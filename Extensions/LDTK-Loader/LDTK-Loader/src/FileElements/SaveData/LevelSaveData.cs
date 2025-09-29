using System.Text.Json.Serialization;

namespace Jackdaw.Loader.LDTK;

class LevelSaveData {
    [JsonPropertyName("layerInstances")]
    public LayerSaveData[] Layers { get; set; } = [];
}