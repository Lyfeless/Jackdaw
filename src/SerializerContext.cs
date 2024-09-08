using System.Text.Json.Serialization;

namespace LittleLib;

[JsonSourceGenerationOptions(
    AllowTrailingCommas = true,
    ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip,
    PropertyNameCaseInsensitive = true
)]
[JsonSerializable(typeof(WorldSaveData))]
[JsonSerializable(typeof(LevelSaveData))]
[JsonSerializable(typeof(Assets.FontConfig))]
[JsonSerializable(typeof(Assets.ShaderConfig))]
[JsonSerializable(typeof(Assets.AnimationConfig))]
[JsonSerializable(typeof(Assets.SoundConfig))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(float))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(int[]))]
[JsonSerializable(typeof(bool[]))]
[JsonSerializable(typeof(float[]))]
[JsonSerializable(typeof(string[]))]
internal partial class SourceGenerationContext : JsonSerializerContext { }