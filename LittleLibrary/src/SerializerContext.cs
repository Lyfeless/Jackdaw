using System.Text.Json.Serialization;

namespace LittleLib;

[JsonSourceGenerationOptions(
    AllowTrailingCommas = true,
    ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip,
    PropertyNameCaseInsensitive = true
)]
//! FIXME (Alex): Disabled for now
// [JsonSerializable(typeof(WorldSaveData))]
// [JsonSerializable(typeof(LevelSaveData))]
[JsonSerializable(typeof(LittleGameConfig))]
[JsonSerializable(typeof(FontConfig))]
[JsonSerializable(typeof(ShaderConfig))]
[JsonSerializable(typeof(AnimationConfig))]
[JsonSerializable(typeof(SoundConfig))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(float))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(int[]))]
[JsonSerializable(typeof(bool[]))]
[JsonSerializable(typeof(float[]))]
[JsonSerializable(typeof(string[]))]
internal partial class SourceGenerationContext : JsonSerializerContext { }