namespace DevWinUIGallery.Models;

public partial class CarouselItemSource : ICarouselViewItemSource
{
    public string ImageSource { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
}
