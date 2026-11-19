namespace DevWinUI;

public partial class StoreCarouselItem
{
    public event EventHandler<StoreCarouselEventArgs>? ActionButtonClick;
    public string ImageSource { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public object? Parameter { get; set; }
    public string ActionButtonText { get; set; } = "See details";
    public bool ShowActionButton { get; set; } = true;

    internal void RaiseAction(object sender, StoreCarouselEventArgs args)
    {
        ActionButtonClick?.Invoke(sender, args);
    }
}
