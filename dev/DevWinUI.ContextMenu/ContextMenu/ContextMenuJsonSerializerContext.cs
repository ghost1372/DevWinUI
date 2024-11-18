using System.Text.Json.Serialization;

namespace DevWinUI;

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(ContextMenuItem))]
internal partial class ContextMenuJsonSerializerContext : JsonSerializerContext
{
}
