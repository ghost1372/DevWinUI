using System.Text.Json;

namespace DevWinUI;
public static partial class JsonUtil
{
    public static string Serialize(object obj)
    {
        return JsonSerializer.Serialize(obj, ContextMenuJsonSerializerContext.Default.ContextMenuItem);
    }

    public static ContextMenuItem Deserialize(string json)
    {
        return JsonSerializer.Deserialize<ContextMenuItem>(json, ContextMenuJsonSerializerContext.Default.ContextMenuItem);
    }
}
