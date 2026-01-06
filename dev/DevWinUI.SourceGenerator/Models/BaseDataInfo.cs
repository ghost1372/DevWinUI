using System.Collections.ObjectModel;

namespace DevWinUI;
internal partial class BaseDataInfo
{
    public string UniqueId { get; set; }
}
internal partial class DataGroup : BaseDataInfo
{
    public ObservableCollection<DataItem> Items { get; set; } = new();
}
internal partial class DataItem : BaseDataInfo
{

}
