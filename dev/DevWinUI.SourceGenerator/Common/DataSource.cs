using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace DevWinUI;

internal partial class Root
{
    public ObservableCollection<DataGroup> Groups { get; set; }
}
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(Root))]
internal partial class RootContext : JsonSerializerContext
{
}
