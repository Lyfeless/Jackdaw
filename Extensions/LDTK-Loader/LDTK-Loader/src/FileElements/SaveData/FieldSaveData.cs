using System.Text.Json.Serialization;

namespace Jackdaw.Loader.LDTK;

public class FieldSaveData {
    /// <summary>
    /// The field name.
    /// </summary>
    [JsonPropertyName("__identifier")]
    public string NameID { get; set; } = string.Empty;

    /// <summary>
    /// The field's data type.
    /// </summary>
    [JsonPropertyName("__type")]
    public string type { get; set; } = string.Empty;

    /// <summary>
    /// The field's value. <br/>
    /// Use <seealso cref="LDTKFieldGetter"/> to convert this back to variable data.
    /// </summary>
    [JsonPropertyName("__value")]
    public object Value { get; set; } = 0;
}