using System.Text.Json.Serialization;

namespace LittleLib.Loader.LDTK;

[JsonSourceGenerationOptions(
    AllowTrailingCommas = true,
    ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip,
    PropertyNameCaseInsensitive = true
)]
[JsonSerializable(typeof(WorldSaveData))]
[JsonSerializable(typeof(LevelSaveData))]
internal partial class LDTKSourceGenerationContext : JsonSerializerContext { }