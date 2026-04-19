namespace DevWinUI;

public interface ICarouselView2
{
    bool AreItemsLoaded { get; set; }
    CarouselView2CarouselTypes CarouselType { get; set; }
    CarouselView2WheelAlignments WheelAlignment { get; set; }

    void ChangeSelection(bool reverse);
    void AnimateSelection();
}
