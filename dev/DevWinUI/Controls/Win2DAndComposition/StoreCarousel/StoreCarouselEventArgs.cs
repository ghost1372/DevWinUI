namespace DevWinUI;

public partial class StoreCarouselEventArgs : EventArgs
{
    public string ImageSource { get; }
    public string Title { get; }
    public string Description { get; }
    public object Parameter { get; }
    public bool IsThumbnail { get; }
    public StoreCarouselEventArgs(string title, string description, string imageSource, bool isThumbnail, object parameter)
    {
        this.Title = title;
        this.Description = description;
        this.ImageSource = imageSource;
        this.IsThumbnail = isThumbnail;
        this.Parameter = parameter;
    }
}
