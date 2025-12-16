namespace DevWinUIGallery.Views;

public sealed partial class FlipCardsPage : Page
{
    public ObservableCollection<FlipCardsSourceType> FlipCardsSourceItems { get; set; } = new ObservableCollection<FlipCardsSourceType>(Enum.GetValues<FlipCardsSourceType>());

    public FlipCardsPage()
    {
        InitializeComponent();
    }
}
