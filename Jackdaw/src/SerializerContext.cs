using System.Text.Json.Serialization;

namespace Jackdaw;

[JsonSourceGenerationOptions(
    AllowTrailingCommas = true,
    ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip,
    PropertyNameCaseInsensitive = true
)]
[JsonSerializable(typeof(GameConfig))]
[JsonSerializable(typeof(FontConfig))]
[JsonSerializable(typeof(ShaderConfig))]
[JsonSerializable(typeof(AnimationConfig))]
[JsonSerializable(typeof(AnimationGroupConfig))]
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