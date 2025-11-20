namespace DevWinUI;
public class StepBarItemClickEventArgs : EventArgs
{
    public int Index { get; set; }
    public StepBarItem? ClickedItem { get; }

    public StepBarItemClickEventArgs(int index, StepBarItem? clickedItem)
    {
        Index = index;
        ClickedItem = clickedItem;
    }
}
