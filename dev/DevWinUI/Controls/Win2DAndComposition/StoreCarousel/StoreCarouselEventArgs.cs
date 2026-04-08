namespace DevWinUI;

public partial class StoreCarouselEventArgs : EventArgs
{
    public string ImageSource { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public object Parameter { get; set; }
    public bool IsThumbnail { get; set; }
    public StoreCarouselEventArgs(string title, string description, string imageSource, bool isThumbnail, object parameter)
    {
        this.Title = title;
        this.Description = description;
        this.ImageSource = imageSource;
        this.IsThumbnail = isThumbnail;
        this.Parameter = parameter;
    }
}
