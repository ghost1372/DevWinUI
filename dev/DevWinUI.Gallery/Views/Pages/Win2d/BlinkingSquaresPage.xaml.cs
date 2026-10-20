namespace DevWinUIGallery.Views;

public sealed partial class BlinkingSquaresPage : Page
{
    public ObservableCollection<BlinkingSquaresDirection> BlinkingSquaresDirectionItems { get; set; } = new ObservableCollection<BlinkingSquaresDirection>(Enum.GetValues<BlinkingSquaresDirection>());

    public BlinkingSquaresPage()
    {
        InitializeComponent();
    }
}
