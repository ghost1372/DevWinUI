namespace DevWinUI;
public partial class DepthLayerViewEventArgs : EventArgs
{
    public int OldIndex { get; set; }
    public int NewIndex { get; set; }

    public DepthLayerViewEventArgs(int oldIndex, int newIndex)
    {
        this.OldIndex = oldIndex;
        this.NewIndex = newIndex;
    }
}
