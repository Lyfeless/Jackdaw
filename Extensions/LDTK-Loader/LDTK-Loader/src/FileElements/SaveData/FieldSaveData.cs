using System.Text.Json.Serialization;

namespace Jackdaw.Loader.LDTK;

public class FieldSaveData {
    [JsonPropertyName("__identifier")]
    public string NameID { get; set; } = string.Empty;

    [JsonPropertyName("__type")]
    public string type { get; set; } = string.Empty;

    [JsonPropertyName("__value")]
    public object Value { get; set; } = 0;
}