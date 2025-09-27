using System.Text.Json.Serialization;

namespace Jackdaw.Loader.LDTK;

class LayerSaveDefinition {
    [JsonPropertyName("uid")]
    public int DefinitionID { get; set; }

    [JsonPropertyName("identifier")]
    public string Identifier { get; set; } = string.Empty;
}