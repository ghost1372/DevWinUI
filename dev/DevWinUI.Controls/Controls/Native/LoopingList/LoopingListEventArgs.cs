namespace DevWinUI;

public partial class LoopingListEventArgs : EventArgs
{
    public LoopingListInfo PrimaryInfo { get; set; }
    public LoopingListInfo SecondaryInfo { get; set; }
    public LoopingListInfo TertiaryInfo { get; set; }

    public LoopingListEventArgs(LoopingListInfo primaryInfo, LoopingListInfo secondaryInfo, LoopingListInfo tertiaryInfo)
    {
        this.PrimaryInfo = primaryInfo;
        this.SecondaryInfo = secondaryInfo;
        this.TertiaryInfo = tertiaryInfo;
    }
}

public partial class LoopingListInfo
{
    public bool LoopingSelectorHasValue { get; set; }
    public string SelectedItem { get; set; }
    public bool SelectedItemHasValue => !string.IsNullOrEmpty(SelectedItem);
}
