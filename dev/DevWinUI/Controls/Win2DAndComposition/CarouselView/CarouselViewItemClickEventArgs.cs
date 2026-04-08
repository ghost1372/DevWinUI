namespace DevWinUI;

public partial class CarouselViewItemClickEventArgs : EventArgs
{
    public object ClickItem { get; set; }
    public CarouselViewItemClickEventArgs(object clickItem)
    {
        this.ClickItem = clickItem;
    }
}
