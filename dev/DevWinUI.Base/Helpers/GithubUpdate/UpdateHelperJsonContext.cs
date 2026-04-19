using System.Text.Json.Serialization;

namespace DevWinUI;

[JsonSourceGenerationOptions()]
[JsonSerializable(typeof(List<UpdateInfo>))]
internal partial class UpdateHelperJsonContext : JsonSerializerContext
{
}
