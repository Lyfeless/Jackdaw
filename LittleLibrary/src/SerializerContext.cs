using System.Text.Json.Serialization;

namespace LittleLib;

[JsonSourceGenerationOptions(
    AllowTrailingCommas = true,
    ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip,
    PropertyNameCaseInsensitive = true
)]
[JsonSerializable(typeof(LittleGameConfig))]
[JsonSerializable(typeof(FontConfig))]
[JsonSerializable(typeof(ShaderConfig))]
[JsonSerializable(typeof(AnimationConfig))]
[JsonSerializable(typeof(AnimationGroupConfig))]
[JsonSerializable(typeof(SoundConfig))]
[JsonSerializable(typeof(AsepriteConfig))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(float))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(int[]))]
[JsonSerializable(typeof(bool[]))]
[JsonSerializable(typeof(float[]))]
[JsonSerializable(typeof(string[]))]
internal partial class SourceGenerationContext : JsonSerializerContext { }