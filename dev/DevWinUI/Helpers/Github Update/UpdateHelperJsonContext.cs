using System.Text.Json.Serialization;

namespace DevWinUI;

[JsonSourceGenerationOptions()]
[JsonSerializable(typeof(UpdateInfo))]
internal partial class UpdateHelperJsonContext : JsonSerializerContext
{
}
