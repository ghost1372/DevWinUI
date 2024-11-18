using System.Text.Json.Serialization;

namespace DevWinUI;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(AppConfig))]
internal partial class GlobalDataJsonContext : JsonSerializerContext
{
}
